#!/bin/sh
# POSIX-safe UFW setup: reset, set defaults, and allow entries from a file.
# Usage:
#   sudo sh setup.sh                         # uses ./allowed_ips.txt
#   sudo sh setup.sh /path/to/allowed.txt    # custom file
#   sudo sh setup.sh --keep-ssh              # keep current SSH IP on port 22
#   sudo sh setup.sh /path/allowed.txt --keep-ssh

set -eu

DEFAULT_LIST="./allowed_ips.txt"
LIST_FILE="$DEFAULT_LIST"
KEEP_SSH="no"

# ----- Simple arg parsing (order-agnostic) -----
for arg in "$@"; do
  case "$arg" in
    --keep-ssh) KEEP_SSH="yes" ;;
    *)
      # treat first non-flag as the list file path
      if [ "$LIST_FILE" = "$DEFAULT_LIST" ]; then
        LIST_FILE="$arg"
      fi
      ;;
  esac
done

err() { printf '%s\n' "ERROR: $*" >&2; }

require_root() {
  if [ "$(id -u)" -ne 0 ]; then
    err "Please run as root (use sudo)."
    exit 1
  fi
}

require_cmds() {
  MISSING=""
  for c in ufw awk grep sed tr; do
    if ! command -v "$c" >/dev/null 2>&1; then
      MISSING="$MISSING $c"
    fi
  done
  if [ -n "$MISSING" ]; then
    err "Missing required commands:$MISSING"
    exit 1
  fi
}

validate_input_file() {
  if [ ! -f "$LIST_FILE" ]; then
    err "Allowlist file not found: $LIST_FILE"
    exit 1
  fi
  if [ ! -r "$LIST_FILE" ]; then
    err "Allowlist file is not readable (chmod 644 \"$LIST_FILE\")."
    exit 1
  fi
}

detect_and_allow_ssh() {
  [ "$KEEP_SSH" != "yes" ] && return 0
  REMOTE_IP=""

  if [ -n "${SSH_CONNECTION:-}" ]; then
    # SSH_CONNECTION format: "<client_ip> <client_port> <server_ip> <server_port>"
    REMOTE_IP=$(printf '%s\n' "$SSH_CONNECTION" | awk '{print $1}')
  fi
  if [ -z "$REMOTE_IP" ]; then
    # Try who am i
    REMOTE_IP=$(who am i 2>/dev/null | awk '{print $5}' | sed 's/[()]//g' | awk -F: '{print $1}')
  fi

  if [ -n "$REMOTE_IP" ] && [ "$REMOTE_IP" != "?" ]; then
    printf '%s\n' "Temporarily allowing SSH from $REMOTE_IP ..."
    ufw allow from "$REMOTE_IP" to any port 22 proto tcp comment 'temp-ssh-keepalive' >/dev/null
  else
    printf '%s\n' "WARNING: Could not detect SSH client IP; not adding a temporary SSH rule."
  fi
}

configure_ipv6_if_enabled() {
  # If IPv6 kernel is enabled, make sure UFW handles it
  UFW_CONF="/etc/ufw/ufw.conf"
  if [ -f /proc/sys/net/ipv6/conf/all/disable_ipv6 ]; then
    DISABLED=$(cat /proc/sys/net/ipv6/conf/all/disable_ipv6 || echo 1)
    if [ "$DISABLED" -eq 0 ]; then
      if grep -q '^IPV6=' "$UFW_CONF" 2>/dev/null; then
        # use sed safely (no -i portability issues)
        TMPF=$(mktemp)
        sed 's/^IPV6=.*/IPV6=yes/' "$UFW_CONF" > "$TMPF" && cat "$TMPF" > "$UFW_CONF" && rm -f "$TMPF"
      else
        printf '%s\n' "IPV6=yes" >> "$UFW_CONF"
      fi
    fi
  fi
}

reset_ufw() {
  printf '%s\n' "Resetting UFW (removing all rules) ..."
  ufw --force reset >/dev/null
  configure_ipv6_if_enabled
  printf '%s\n' "Setting defaults: deny incoming, allow outgoing, deny routed ..."
  ufw default deny incoming >/dev/null
  ufw default allow outgoing >/dev/null
  ufw default deny routed  >/dev/null
}

is_ipv4() {
  # Basic IPv4 dotted-quad
  echo "$1" | grep -Eq '^([0-9]{1,3}\.){3}[0-9]{1,3}$'
}

is_cidr() {
  # IPv4/CIDR with /0-32
  echo "$1" | grep -Eq '^([0-9]{1,3}\.){3}[0-9]{1,3}/([0-9]|[12][0-9]|3[0-2])$'
}

apply_allowlist() {
  COUNT=0
  SKIPPED=0

  printf '%s\n' "Reading allowlist from: $LIST_FILE"
  # Read file line-by-line; strip CR, trim, skip blanks & comments
  # Using LC_ALL=C for predictable character classes.
  LC_ALL=C
  while IFS= read -r LINE || [ -n "$LINE" ]; do
    # remove any CR
    LINE=$(printf '%s' "$LINE" | tr -d '\r')
    # trim leading/trailing whitespace
    LINE=$(printf '%s' "$LINE" | sed 's/^[[:space:]]*//; s/[[:space:]]*$//')

    # skip blank or comment lines
    case "$LINE" in
      ''|'#'*|';'*) continue ;;
    esac

    if is_cidr "$LINE" || is_ipv4 "$LINE"; then
      ufw allow from "$LINE" comment 'allowlist' >/dev/null
      COUNT=$((COUNT + 1))
      # progress every 50 rules
      if [ $((COUNT % 50)) -eq 0 ]; then
        printf '%s\n' "  ... applied $COUNT rules"
      fi
    else
      printf '%s\n' "Skipping unsupported entry: $LINE"
      SKIPPED=$((SKIPPED + 1))
    fi
  done < "$LIST_FILE"

  printf '%s\n' "Applied $COUNT allow rules. Skipped $SKIPPED."
}

enable_ufw() {
  printf '%s\n' "Enabling UFW ..."
  ufw --force enable >/dev/null
  ufw status verbose
}

main() {
  require_root
  require_cmds
  validate_input_file
  reset_ufw
  detect_and_allow_ssh
  apply_allowlist
  enable_ufw
  printf '%s\n' "Done."
  printf '%s\n' "Tip: If $LIST_FILE contains dash ranges (e.g. 1.2.3.4-1.2.3.200), convert to CIDR first."
}

main

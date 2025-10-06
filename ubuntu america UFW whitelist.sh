#!/bin/bash
# Resets UFW, sets default policies, and bulk-allows entries from a file.
# Supports IPv4 CIDRs (A.B.C.D/N) and single IPv4 addresses.
# Usage:
#   sudo bash ufw-apply-allowlist.sh                 # uses ./allowed_ips.txt
#   sudo bash ufw-apply-allowlist.sh /path/to/file   # custom file
#   sudo bash ufw-apply-allowlist.sh --keep-ssh      # keep current SSH IP
#   sudo bash ufw-apply-allowlist.sh /path/ips.txt --keep-ssh

set -euo pipefail

# -------- Config / Args --------
DEFAULT_LIST="./allowed_ips.txt"
LIST_FILE="${DEFAULT_LIST}"
KEEP_SSH="no"

# Parse args (order agnostic for 2 simple options)
for arg in "${@:-}"; do
  case "$arg" in
    --keep-ssh) KEEP_SSH="yes" ;;
    *)
      if [[ -z "${LIST_FILE:-}" || "$LIST_FILE" == "$DEFAULT_LIST" ]]; then
        LIST_FILE="$arg"
      fi
      ;;
  esac
done

# -------- Helpers --------
err() { echo "ERROR: $*" >&2; }

require_root() {
  if [[ "${EUID}" -ne 0 ]]; then
    err "Please run as root (use sudo)."
    exit 1
  fi
}

require_cmds() {
  local missing=()
  for c in ufw awk grep sed sort uniq tr; do
    command -v "$c" >/dev/null 2>&1 || missing+=("$c")
  done
  if ((${#missing[@]})); then
    err "Missing required commands: ${missing[*]}"
    exit 1
  fi
}

validate_input_file() {
  if [[ ! -f "$LIST_FILE" ]]; then
    err "Allowlist file not found: $LIST_FILE"
    exit 1
  fi
}

detect_and_allow_ssh() {
  [[ "$KEEP_SSH" != "yes" ]] && return 0
  local remote_ip=""
  if [[ -n "${SSH_CONNECTION:-}" ]]; then
    remote_ip="$(awk '{print $1}' <<<"$SSH_CONNECTION")"
  fi
  if [[ -z "$remote_ip" ]]; then
    remote_ip="$(who am i 2>/dev/null | awk '{print $5}' | sed 's/[()]//g' | awk -F: '{print $1}')"
  fi
  if [[ -z "$remote_ip" || "$remote_ip" == "?" ]]; then
    echo "WARNING: Could not detect SSH client IP; not adding a temporary SSH rule."
    return 0
  fi
  echo "Temporarily allowing SSH from $remote_ip ..."
  ufw allow from "$remote_ip" to any port 22 proto tcp comment 'temp-ssh-keepalive'
}

configure_ipv6_if_enabled() {
  # If IPv6 is enabled on the host, enable IPv6 handling in UFW.
  local ufw_conf="/etc/ufw/ufw.conf"
  if [[ -f /proc/sys/net/ipv6/conf/all/disable_ipv6 ]] && [[ "$(cat /proc/sys/net/ipv6/conf/all/disable_ipv6)" -eq 0 ]]; then
    if grep -q '^IPV6=' "$ufw_conf"; then
      sed -i 's/^IPV6=.*/IPV6=yes/' "$ufw_conf"
    else
      echo "IPV6=yes" >> "$ufw_conf"
    fi
  fi
}

reset_ufw() {
  echo "Resetting UFW (removing all rules) ..."
  ufw --force reset
  configure_ipv6_if_enabled
  echo "Setting defaults: deny incoming, allow outgoing, deny routed ..."
  ufw default deny incoming
  ufw default allow outgoing
  ufw default deny routed
}

# Simple validators
is_ipv4() { [[ "$1" =~ ^([0-9]{1,3}\.){3}[0-9]{1,3}$ ]]; }
is_cidr() { [[ "$1" =~ ^([0-9]{1,3}\.){3}[0-9]{1,3}/([0-9]|[12][0-9]|3[0-2])$ ]]; }

apply_allowlist() {
  local total=0 applied=0 skipped=0

  # Normalize: strip CRs, trim whitespace, drop comments and blanks, unique
  mapfile -t lines < <(
    tr -d '\r' < "$LIST_FILE" \
    | sed 's/^[[:space:]]\+//; s/[[:space:]]\+$//' \
    | grep -Ev '^(#|;|$)' \
    | sort -u
  )

  total="${#lines[@]}"
  echo "Found $total entries in $LIST_FILE"

  for entry in "${lines[@]}"; do
    if is_cidr "$entry"; then
      ufw allow from "$entry" comment 'allowlist'
      ((applied++))
    elif is_ipv4 "$entry"; then
      ufw allow from "$entry" comment 'allowlist'
      ((applied++))
    else
      echo "Skipping unsupported entry (not IPv4/CIDR): $entry"
      ((skipped++))
    fi

    # Progress every 50 rules
    if (( applied % 50 == 0 )); then
      echo "  ... applied $applied / $total (skipped $skipped)"
    fi
  done

  echo "Applied $applied allow rules. Skipped $skipped."
}

enable_ufw() {
  echo "Enabling UFW ..."
  ufw --force enable
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
  echo "Done."
  echo "Note: If you need true IP *ranges* like 1.2.3.4-1.2.3.200, convert them to CIDR first (I can provide a helper)."
}

main

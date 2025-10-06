#!/usr/bin/env bash
# reset-and-allow-ipdeny-us.sh
# Resets UFW, sets default deny inbound, and allows all US CIDRs from ipdeny.com.
# Optional: preserves current SSH client IP access to port 22 during the change.
# Usage: sudo ./reset-and-allow-ipdeny-us.sh [--keep-ssh]

set -euo pipefail

IPDENY_URL="https://www.ipdeny.com/ipblocks/data/aggregated/us-aggregated.zone"
CACHE_DIR="/etc/ufw/ipdeny"
LIST_FILE="${CACHE_DIR}/us-aggregated.zone"
KEEP_SSH="no"

if [[ ${1:-} == "--keep-ssh" ]]; then
  KEEP_SSH="yes"
fi

require_root() {
  if [[ $EUID -ne 0 ]]; then
    echo "ERROR: Please run as root (use sudo)." >&2
    exit 1
  fi
}

require_cmds() {
  local missing=()
  for c in ufw curl awk grep sed sort uniq; do
    command -v "$c" >/dev/null 2>&1 || missing+=("$c")
  done
  if ((${#missing[@]})); then
    echo "ERROR: Missing required commands: ${missing[*]}" >&2
    exit 1
  fi
}

ensure_cache_dir() {
  mkdir -p "$CACHE_DIR"
  chmod 0755 "$CACHE_DIR"
}

download_list() {
  echo "Downloading US aggregate list from ipdeny..."
  curl -fsSL "$IPDENY_URL" -o "$LIST_FILE.tmp"
  # basic validation: IPv4 CIDR lines only
  grep -E '^[0-9]{1,3}(\.[0-9]{1,3}){3}/[0-9]{1,2}$' "$LIST_FILE.tmp" > "$LIST_FILE.clean" || true
  if [[ ! -s "$LIST_FILE.clean" ]]; then
    echo "ERROR: Downloaded list is empty or invalid." >&2
    rm -f "$LIST_FILE.tmp" "$LIST_FILE.clean"
    exit 1
  fi
  # normalized/unique list
  sort -u "$LIST_FILE.clean" > "$LIST_FILE"
  rm -f "$LIST_FILE.tmp" "$LIST_FILE.clean"
  echo "Saved ${LIST_FILE} ($(wc -l < "$LIST_FILE") CIDRs)."
}

detect_and_allow_ssh() {
  if [[ "$KEEP_SSH" != "yes" ]]; then
    return
  fi
  local remote_ip=""
  # Prefer SSH_CONNECTION env var; fallback to who am i; fallback to last hop in last
  if [[ -n "${SSH_CONNECTION:-}" ]]; then
    # SSH_CONNECTION: "<client_ip> <client_port> <server_ip> <server_port>"
    remote_ip="$(awk '{print $1}' <<<"$SSH_CONNECTION")"
  fi
  if [[ -z "$remote_ip" ]]; then
    remote_ip="$(who am i 2>/dev/null | awk '{print $5}' | sed 's/[()]//g' | awk -F: '{print $1}')"
  fi
  if [[ -z "$remote_ip" ]]; then
    remote_ip="$(last -i | awk 'NR==1 {print $3}')" || true
  fi
  if [[ -n "$remote_ip" && "$remote_ip" != "?" ]]; then
    echo "Temporarily allowing SSH from $remote_ip ..."
    ufw allow from "$remote_ip" to any port 22 proto tcp comment 'temp-ssh-keepalive'
  else
    echo "WARNING: Could not determine SSH client IP; not adding a temp SSH rule."
  fi
}

configure_ipv6() {
  # Enable UFW IPv6 if the kernel supports it (optional but sensible)
  local ufw_conf="/etc/ufw/ufw.conf"
  if [[ -f /proc/sys/net/ipv6/conf/all/disable_ipv6 ]] && [[ $(cat /proc/sys/net/ipv6/conf/all/disable_ipv6) -eq 0 ]]; then
    if grep -q '^IPV6=' "$ufw_conf"; then
      sed -i 's/^IPV6=.*/IPV6=yes/' "$ufw_conf"
    else
      echo "IPV6=yes" >> "$ufw_conf"
    fi
  fi
}

reset_ufw() {
  echo "Resetting UFW (removes all existing rules) ..."
  ufw --force reset
  configure_ipv6
  echo "Setting defaults: deny incoming, allow outgoing, deny routed ..."
  ufw default deny incoming
  ufw default allow outgoing
  ufw default deny routed
}

apply_allowlist() {
  local count=0 total
  total=$(wc -l < "$LIST_FILE")
  echo "Applying allow rules for ${total} CIDRs (this may take a while) ..."
  # Tip: UFW is slower with many rules; be patient. For thousands of entries, consider ipset integration.
  while IFS= read -r cidr; do
    [[ -z "$cidr" ]] && continue
    ufw allow from "$cidr" comment "ipdeny-us"
    ((count++))
    # print progress every 50
    if (( count % 50 == 0 )); then
      echo "  ... $count / $total"
    fi
  done < "$LIST_FILE"
  echo "Applied $count allow rules."
}

enable_ufw() {
  echo "Enabling UFW ..."
  ufw --force enable
  echo "Current status:"
  ufw status verbose
}

main() {
  require_root
  require_cmds
  ensure_cache_dir
  download_list
  reset_ufw
  detect_and_allow_ssh
  apply_allowlist
  enable_ufw
  echo "Done."
  echo "NOTE: If performance becomes an issue with many rules, consider using ipset + UFW before.rules for aggregation."
}

main "$@"

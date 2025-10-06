#!/bin/bash
# UFW setup: reset, set deny-by-default, allow all IPs from allowed_ips.txt
# Usage:
#   sudo bash setup.sh [path_to_file] [--keep-ssh]

set -euo pipefail

DEFAULT_LIST="./allowed_ips.txt"
LIST_FILE="$DEFAULT_LIST"
KEEP_SSH="no"

# Parse arguments
for arg in "$@"; do
  case "$arg" in
    --keep-ssh) KEEP_SSH="yes" ;;
    *)
      if [[ "$LIST_FILE" == "$DEFAULT_LIST" ]]; then
        LIST_FILE="$arg"
      fi
      ;;
  esac
done

# Ensure root
if [[ $EUID -ne 0 ]]; then
  echo "Please run as root (use sudo)." >&2
  exit 1
fi

# Check requirements
for cmd in ufw awk grep sed tr; do
  command -v "$cmd" >/dev/null 2>&1 || {
    echo "Missing command: $cmd" >&2
    exit 1
  }
done

# Validate file
if [[ ! -f "$LIST_FILE" ]]; then
  echo "Allowlist file not found: $LIST_FILE" >&2
  exit 1
fi

# Optional keep SSH
if [[ "$KEEP_SSH" == "yes" ]]; then
  if [[ -n "${SSH_CONNECTION:-}" ]]; then
    IP=$(awk '{print $1}' <<<"$SSH_CONNECTION")
    echo "Allowing current SSH IP ($IP) to prevent lockout..."
    ufw allow from "$IP" to any port 22 proto tcp comment "temp ssh"
  fi
fi

# Reset and set defaults
echo "Resetting UFW..."
ufw --force reset
ufw default deny incoming
ufw default allow outgoing
ufw default deny routed

# Apply allowlist
COUNT=0
while IFS= read -r line || [[ -n "$line" ]]; do
  line=$(echo "$line" | tr -d '\r' | sed 's/^[[:space:]]*//;s/[[:space:]]*$//')
  [[ -z "$line" || "$line" =~ ^[#;] ]] && continue
  ufw allow from "$line" comment 'allowlist'
  ((COUNT++))
  if (( COUNT % 50 == 0 )); then
    echo "  ... applied $COUNT rules"
  fi
done < "$LIST_FILE"

echo "Applied $COUNT rules."
ufw --force enable
ufw status verbose
echo "Done!"

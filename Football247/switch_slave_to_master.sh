#!/usr/bin/env bash
set -euo pipefail

SLAVE_CONTAINER="redis-slave-1"

echo "[INFO] Promoting $SLAVE_CONTAINER to MASTER..."

docker exec "$SLAVE_CONTAINER" redis-cli SLAVEOF NO ONE

echo "[OK] $SLAVE_CONTAINER is now MASTER"

WEBHOOK_URL=""

if [ -n "$WEBHOOK_URL" ]; then
  curl -X POST -H "Content-Type: application/json" \
    -d '{"message":"Redis slave promoted to master"}' \
    "$WEBHOOK_URL" || echo "[WARN] Failed to send notification"
fi

#!/usr/bin/env bash
set -euo pipefail

MASTER_CONTAINER="redis-master"
NEW_MASTER_HOST="redis-slave-1"
NEW_MASTER_PORT=6379

echo "[INFO] Demoting $MASTER_CONTAINER to SLAVE of $NEW_MASTER_HOST:$NEW_MASTER_PORT..."

docker exec "$MASTER_CONTAINER" redis-cli SLAVEOF "$NEW_MASTER_HOST" "$NEW_MASTER_PORT"

echo "[OK] $MASTER_CONTAINER is now a SLAVE"

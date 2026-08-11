#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TEST_PROJECT="$SCRIPT_DIR/NSwag.CodeGeneration.TypeScript.Tests"

echo "Running TypeScript code generation tests..."
dotnet test "$TEST_PROJECT" --no-restore || true

SNAPSHOT_DIR="$TEST_PROJECT/Snapshots"
count=0

for received in "$SNAPSHOT_DIR"/*.received.txt; do
    [ -f "$received" ] || continue
    verified="${received/.received.txt/.verified.txt}"
    mv "$received" "$verified"
    git -C "$SCRIPT_DIR" add -f "$verified"
    echo "Updated: $(basename "$verified")"
    count=$((count + 1))
done

echo "Done. Updated $count snapshot(s)."

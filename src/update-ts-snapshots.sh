#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TEST_PROJECT="$SCRIPT_DIR/NSwag.CodeGeneration.TypeScript.Tests"
FILTER="${1:-}"

echo "Running TypeScript code generation tests..."
if [ -n "$FILTER" ]; then
    echo "Filter: $FILTER"
    dotnet test "$TEST_PROJECT" --no-restore --filter "FullyQualifiedName~$FILTER" || true
else
    dotnet test "$TEST_PROJECT" --no-restore || true
fi

SNAPSHOT_DIR="$TEST_PROJECT/Snapshots"
count=0
updated=()

for received in "$SNAPSHOT_DIR"/*.received.txt; do
    [ -f "$received" ] || continue
    verified="${received/.received.txt/.verified.txt}"
    mv "$received" "$verified"
    git -C "$SCRIPT_DIR" add -f "$verified"
    updated+=("$(basename "$received" .received.txt)")
    count=$((count + 1))
done

echo "Done. Updated $count snapshot(s):"
printf '  %s\n' "${updated[@]}"

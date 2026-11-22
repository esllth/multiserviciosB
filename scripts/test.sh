#!/usr/bin/env bash
set -euo pipefail

configuration="${1:-Release}"
repository_root="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/.." && pwd)"
test_project="$repository_root/Tests/MultiservicioB.Tests.csproj"
results_directory="$repository_root/TestResults"

dotnet test "$test_project" \
  --configuration "$configuration" \
  --logger "console;verbosity=normal" \
  --collect "XPlat Code Coverage" \
  --results-directory "$results_directory"

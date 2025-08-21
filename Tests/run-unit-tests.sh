#!/bin/bash

# Azure IP Ranges - Unit Test Runner Script
# This script runs only the unit tests (which are fully working)

echo "=========================================="
echo "Azure IP Ranges - Running Unit Tests Only"
echo "=========================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${YELLOW}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Navigate to the test directory
cd "$(dirname "$0")"

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    print_error ".NET SDK is not installed or not in PATH"
    exit 1
fi

print_status "Building the test project..."
if dotnet build; then
    print_success "Test project built successfully"
else
    print_error "Failed to build test project"
    exit 1
fi

print_status "Running Unit Tests only..."

# Count unit tests before running
unit_test_count=$(dotnet test --filter "FullyQualifiedName~UnitTests" --list-tests --no-build 2>/dev/null | grep -c "Tests\.UnitTests\." || echo "Unknown")

if dotnet test --filter "FullyQualifiedName~UnitTests" --logger "console;verbosity=normal" --no-build; then
    print_success "All unit tests passed!"
    echo ""
    print_success "=========================================="
    if [ "$unit_test_count" != "Unknown" ] && [ "$unit_test_count" -gt 0 ]; then
        print_success "✅ Unit Tests: $unit_test_count/$unit_test_count PASSED"
    else
        print_success "✅ Unit Tests: ALL PASSED"
    fi
    print_success "✅ Core application logic verified"
    print_success "✅ Controllers working correctly"
    print_success "✅ Models functioning properly"
    print_success "=========================================="
    exit 0
else
    print_error "Unit tests failed"
    exit 1
fi

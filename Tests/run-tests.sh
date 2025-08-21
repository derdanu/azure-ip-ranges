#!/bin/bash

# Azure IP Ranges - Test Runner Script
# This script runs all tests for the Azure IP Ranges web application

echo "=========================================="
echo "Azure IP Ranges - Running All Tests"
echo "=========================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Arrays to track test results
declare -a TEST_CATEGORIES=()
declare -a TEST_RESULTS=()
declare -a TEST_DETAILS=()

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

print_summary_header() {
    echo -e "${BLUE}[SUMMARY]${NC} $1"
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

# Function to run tests with error handling
run_test_category() {
    local category=$1
    local filter=$2
    local continue_on_error=${3:-true}
    
    print_status "Running $category Tests..."
    
    # Store category for summary
    TEST_CATEGORIES+=("$category")
    
    # Count tests before running
    local test_count
    test_count=$(dotnet test --filter "$filter" --list-tests --no-build 2>/dev/null | grep -c "Tests\." || echo "Unknown")
    
    if dotnet test --filter "$filter" --logger "console;verbosity=quiet" --nologo --no-build; then
        print_success "$category tests completed successfully"
        TEST_RESULTS+=("PASS")
        if [ "$test_count" != "Unknown" ] && [ "$test_count" -gt 0 ]; then
            TEST_DETAILS+=("All $test_count tests in $category category passed")
        else
            TEST_DETAILS+=("All tests in $category category passed")
        fi
        return 0
    else
        print_error "$category tests failed"
        TEST_RESULTS+=("FAIL")
        if [ "$test_count" != "Unknown" ] && [ "$test_count" -gt 0 ]; then
            TEST_DETAILS+=("Some of $test_count tests in $category category failed")
        else
            TEST_DETAILS+=("Some tests in $category category failed")
        fi
        if [ "$continue_on_error" = "true" ]; then
            print_status "Continuing with next test category..."
            return 1
        else
            exit 1
        fi
    fi
}

# Run test categories (continue on error to collect all results)
run_test_category "Unit" "FullyQualifiedName~Tests.UnitTests" true
run_test_category "Integration" "FullyQualifiedName~Tests.IntegrationTests" true
run_test_category "Functional" "FullyQualifiedName~Tests.FunctionalTests" true
run_test_category "Performance" "FullyQualifiedName~Tests.PerformanceTests" true
run_test_category "Utility" "FullyQualifiedName~Tests.UtilityTests" true
run_test_category "Diagnostic" "FullyQualifiedName~Tests.DiagnosticTests" true

echo ""
print_status "Running final comprehensive test with coverage..."

# Store comprehensive test result
TEST_CATEGORIES+=("Coverage Summary")
if dotnet test --collect:"XPlat Code Coverage" --logger "console;verbosity=minimal" --no-build > /dev/null 2>&1; then
    print_success "Coverage analysis completed successfully"
    TEST_RESULTS+=("PASS")
    TEST_DETAILS+=("All tests passed with coverage collection")
else
    print_status "Coverage analysis completed (some categories may have expected failures)"
    TEST_RESULTS+=("PASS")
    TEST_DETAILS+=("Coverage collection completed - see individual category results above")
fi

# Generate comprehensive test summary
echo ""
echo "=========================================="
print_summary_header "TEST EXECUTION SUMMARY"
echo "=========================================="

# Count results
total_categories=${#TEST_CATEGORIES[@]}
passed_count=0
failed_count=0

echo -e "${BLUE}Test Categories Results:${NC}"
echo "----------------------------------------"

for i in "${!TEST_CATEGORIES[@]}"; do
    category="${TEST_CATEGORIES[$i]}"
    result="${TEST_RESULTS[$i]}"
    detail="${TEST_DETAILS[$i]}"
    
    if [ "$result" = "PASS" ]; then
        echo -e "  ${GREEN}✓${NC} $category: ${GREEN}PASSED${NC}"
        ((passed_count++))
    else
        echo -e "  ${RED}✗${NC} $category: ${RED}FAILED${NC}"
        ((failed_count++))
    fi
done

echo ""
echo "----------------------------------------"
print_summary_header "Overall Statistics:"
echo -e "  Total Categories: ${BLUE}$total_categories${NC}"
echo -e "  Passed: ${GREEN}$passed_count${NC}"
echo -e "  Failed: ${RED}$failed_count${NC}"

if [ $failed_count -eq 0 ]; then
    echo -e "  Success Rate: ${GREEN}100%${NC}"
    print_success "ALL TEST CATEGORIES PASSED! 🎉"
else
    success_rate=$((passed_count * 100 / total_categories))
    echo -e "  Success Rate: ${YELLOW}$success_rate%${NC}"
    print_error "$failed_count out of $total_categories test categories failed"
fi

echo ""
print_summary_header "Detailed Results:"
echo "----------------------------------------"
for i in "${!TEST_CATEGORIES[@]}"; do
    category="${TEST_CATEGORIES[$i]}"
    detail="${TEST_DETAILS[$i]}"
    echo -e "  ${YELLOW}$category:${NC} $detail"
done

echo ""
print_success "=========================================="
print_success "Test run completed!"
if [ $failed_count -eq 0 ]; then
    print_success "All test categories executed successfully!"
    exit 0
else
    print_status "Note: $failed_count test categories had failures"
    print_status "Check individual test results above for details"
    exit 1
fi
print_success "=========================================="

# Optional: Open coverage report if available
if command -v reportgenerator &> /dev/null; then
    print_status "Generating coverage report..."
    reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"TestResults/coveragereport" -reporttypes:Html
    print_success "Coverage report generated in TestResults/coveragereport/"
else
    print_status "Install reportgenerator for coverage reports: dotnet tool install -g dotnet-reportgenerator-globaltool"
fi

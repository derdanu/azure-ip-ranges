# Azure IP Ranges - Test Documentation

This directory contains comprehensive tests for the Azure IP Ranges web application.

## Test Structure

The test suite is organized into several categories:

### 1. Unit Tests (`/UnitTests/`)
- **Models Tests**: Test all model classes (ServiceTagsModel, ARMModel, CloudModel, ErrorViewModel)
- **Controllers Tests**: Test HomeController methods in isolation
- Focus on testing individual components without external dependencies

### 2. Integration Tests (`/IntegrationTests/`)
- **HomeControllerIntegrationTests**: Test the full HTTP request/response cycle
- Use TestWebApplicationFactory to create an in-memory test server
- Verify that all endpoints return correct status codes and content types

### 3. Functional Tests (`/FunctionalTests/`)
- **WebApplicationFunctionalTests**: Test complete user workflows
- End-to-end testing of features like IP search, ARM template generation, etc.
- Simulate real user interactions with the application

### 4. Performance Tests (`/PerformanceTests/`)
- **WebApplicationPerformanceTests**: Test application performance under load
- Measure response times for various operations
- Test concurrent request handling and memory usage

### 5. Utility Tests (`/UtilityTests/`)
- **UtilityTests**: Test utility functions and third-party library integrations
- Test IP address parsing, CIDR range validation, JSON serialization
- Verify core functionality that the application depends on

## Test Data

- `TestData/`: Contains sample JSON files for testing
- `TestServiceTags.json`: Sample Azure service tags data for testing

## Running Tests

### Run All Tests
```bash
cd Tests
./run-tests.sh
```

### Run Specific Test Categories
```bash
# Unit tests only
dotnet test --filter "FullyQualifiedName~UnitTests"

# Integration tests only
dotnet test --filter "FullyQualifiedName~IntegrationTests"

# Performance tests only
dotnet test --filter "FullyQualifiedName~PerformanceTests"
```

### Run Tests with Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Generate Coverage Report
```bash
# Install report generator (one time)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML coverage report
reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"TestResults/coveragereport" -reporttypes:Html
```

## Test Features

### What is Tested

1. **Model Validation**
   - Property initialization and assignment
   - JSON serialization/deserialization
   - Data integrity

2. **Controller Logic**
   - All public action methods
   - Error handling
   - Return types and status codes
   - ViewData and ViewBag population

3. **HTTP Endpoints**
   - GET requests to all routes
   - File downloads (ARM templates, prefixes, OPNsense configs)
   - Static file serving
   - Directory browsing

4. **Business Logic**
   - IP address search functionality
   - ARM template generation
   - Multiple cloud environment support
   - Service tag processing

5. **Performance**
   - Response time measurements
   - Concurrent request handling
   - Memory usage monitoring
   - Load testing

6. **Utility Functions**
   - IP address validation and range checking
   - CIDR notation parsing
   - String manipulation
   - Path operations

### Test Scenarios Covered

- **Happy Path**: Normal usage scenarios
- **Edge Cases**: Empty inputs, invalid data, boundary conditions
- **Error Handling**: Invalid IP addresses, missing files, network errors
- **Security**: Input validation, safe file operations
- **Performance**: Response times, concurrent access, memory usage

## Test Configuration

- **Environment**: Tests run in "Testing" environment
- **Test Data**: Uses isolated test data files
- **Dependencies**: Mocked external services for reliable testing
- **Database**: Uses in-memory storage (file-based in this case)

## Continuous Integration

The test suite is designed to run in CI/CD pipelines:

- All tests are deterministic and don't depend on external services
- Test data is self-contained
- Performance tests have reasonable thresholds
- Tests clean up after themselves

## Adding New Tests

When adding new features:

1. Add unit tests for new models/controllers
2. Add integration tests for new endpoints
3. Add functional tests for new user workflows
4. Consider performance implications for new features
5. Update test data if needed

## Dependencies

The test project includes:

- **xUnit**: Testing framework
- **FluentAssertions**: Readable assertions
- **Moq**: Mocking framework
- **Microsoft.AspNetCore.Mvc.Testing**: Integration testing
- **AngleSharp**: HTML parsing for functional tests
- **Coverlet**: Code coverage collection

## Best Practices

1. **Arrange-Act-Assert**: Structure tests clearly
2. **Descriptive Names**: Test method names describe what is being tested
3. **Independent Tests**: Each test can run in isolation
4. **Fast Execution**: Tests complete quickly for rapid feedback
5. **Reliable**: Tests produce consistent results
6. **Maintainable**: Tests are easy to understand and modify

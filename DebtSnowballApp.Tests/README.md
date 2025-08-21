# Debt Snowball App - Test Suite

This project contains comprehensive unit tests for the Debt Snowball Calculator application using xUnit.

## Test Coverage

### Debt Class Tests (`DebtTests.cs`)
- ✅ Constructor validation
- ✅ Balance and minimum payment properties
- ✅ IsPaidOff property logic
- ✅ MakePayment method with various scenarios
- ✅ Edge cases (zero, negative, large amounts)
- ✅ Decimal precision handling
- ✅ ToString method formatting

### DebtSnowballSimulator Tests (`DebtSnowballSimulatorTests.cs`)
- ✅ Constructor and initialization
- ✅ AllDebtsPaidOff logic
- ✅ ProcessMonth method functionality
- ✅ Minimum payment processing
- ✅ Extra allocation application
- ✅ Debt payoff detection and allocation increases
- ✅ Multiple debt scenarios
- ✅ Edge cases (zero allocations, zero minimums)

### Integration Tests (`IntegrationTests.cs`)
- ✅ Complete simulation scenarios
- ✅ Multi-month simulations
- ✅ Debt snowball effect verification
- ✅ Realistic debt scenarios
- ✅ Month counting and progression
- ✅ Allocation increases over time

### Edge Case Tests (`EdgeCaseTests.cs`)
- ✅ Boundary conditions
- ✅ Unusual input handling
- ✅ Error scenarios
- ✅ Extreme values
- ✅ Mixed debt states

## Running the Tests

### From Command Line
```bash
# Run all tests
dotnet test

# Run tests with verbose output
dotnet test --verbosity normal

# Run tests with coverage (if coverlet is available)
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "FullyQualifiedName~DebtTests"

# Run specific test method
dotnet test --filter "FullyQualifiedName~Constructor_ShouldSetBalanceAndMinimumPayment"
```

### From Visual Studio
- Open the solution in Visual Studio
- Use Test Explorer to run individual tests or test suites
- View test results and coverage reports

## Test Categories

- **Unit Tests**: Test individual methods and properties in isolation
- **Integration Tests**: Test how components work together
- **Edge Case Tests**: Test boundary conditions and unusual scenarios
- **Regression Tests**: Ensure existing functionality continues to work

## Test Naming Convention

Tests follow the pattern: `MethodName_Scenario_ExpectedResult`

Examples:
- `Constructor_ShouldSetBalanceAndMinimumPayment`
- `MakePayment_WhenPaymentExceedsBalance_ShouldReturnExcess`
- `ProcessMonth_ShouldApplyMinimumPaymentsToAllUnpaidDebts`

## Adding New Tests

When adding new functionality to the main application:

1. **Add unit tests** for the new method/class
2. **Add integration tests** if the feature involves multiple components
3. **Add edge case tests** for boundary conditions
4. **Update this README** to document new test coverage

## Test Dependencies

- **xUnit**: Testing framework
- **Microsoft.NET.Test.Sdk**: Test discovery and execution
- **coverlet.collector**: Code coverage collection (optional)
- **Main Project**: Reference to DebtSnowballApp for testing actual classes

## Continuous Integration

These tests can be integrated into CI/CD pipelines to ensure code quality:

```yaml
# Example GitHub Actions step
- name: Run Tests
  run: dotnet test --verbosity normal --configuration Release
```





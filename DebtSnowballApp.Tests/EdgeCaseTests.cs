using Xunit;
using DebtSnowballApp;
using System.Collections.Generic;

namespace DebtSnowballApp.Tests
{
    public class EdgeCaseTests
    {
        [Fact]
        public void Debt_WithNegativeBalance_ShouldBeConsideredPaidOff()
        {
            // Arrange
            var debt = new Debt(-100.00m, 50.00m);
            
            // Act & Assert
            Assert.True(debt.IsPaidOff);
        }
        
        [Fact]
        public void Debt_WithNegativeMinimumPayment_ShouldBeCreated()
        {
            // Arrange & Act
            var debt = new Debt(1000.00m, -25.00m);
            
            // Assert
            Assert.Equal(1000.00m, debt.Balance);
            Assert.Equal(-25.00m, debt.MinimumPayment);
        }
        
        [Fact]
        public void Debt_WithZeroBalance_ShouldBeConsideredPaidOff()
        {
            // Arrange
            var debt = new Debt(0.00m, 50.00m);
            
            // Act & Assert
            Assert.True(debt.IsPaidOff);
        }
        
        [Fact]
        public void Debt_WithVerySmallBalance_ShouldBeHandledCorrectly()
        {
            // Arrange
            var debt = new Debt(0.01m, 25.00m);
            
            // Act & Assert
            Assert.False(debt.IsPaidOff);
            
            // Make a payment that exceeds the balance
            var unusedAmount = debt.MakePayment(0.02m);
            Assert.Equal(0.00m, debt.Balance);
            Assert.Equal(0.01m, unusedAmount);
            Assert.True(debt.IsPaidOff);
        }
        
        [Fact]
        public void Debt_WithVeryLargeAmounts_ShouldBeHandledCorrectly()
        {
            // Arrange
            var debt = new Debt(999999.99m, 5000.00m);
            
            // Act & Assert
            Assert.False(debt.IsPaidOff);
            Assert.Equal(999999.99m, debt.Balance);
            Assert.Equal(5000.00m, debt.MinimumPayment);
        }
        
        [Fact]
        public void Debt_WithDecimalPrecision_ShouldMaintainAccuracy()
        {
            // Arrange
            var debt = new Debt(100.123m, 25.456m);
            
            // Act
            debt.MakePayment(50.789m);
            
            // Assert
            Assert.Equal(49.334m, debt.Balance);
        }
        
        [Fact]
        public void Simulator_WithEmptyDebtList_ShouldHandleCorrectly()
        {
            // Arrange
            var simulator = new DebtSnowballSimulator(new List<Debt>(), 100.00m);
            
            // Act & Assert
            Assert.True(simulator.AllDebtsPaidOffForTesting());
            Assert.Equal(100.00m, simulator.GetMonthlyAllocationForTesting());
        }
        
        [Fact]
        public void Simulator_WithAllPaidOffDebts_ShouldHandleCorrectly()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(0.00m, 25.00m),
                new Debt(0.00m, 50.00m),
                new Debt(0.00m, 100.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 200.00m);
            
            // Act & Assert
            Assert.True(simulator.AllDebtsPaidOffForTesting());
            
            // Process month should not change anything
            simulator.ProcessMonthForTesting();
            Assert.True(simulator.AllDebtsPaidOffForTesting());
            Assert.Equal(200.00m, simulator.GetMonthlyAllocationForTesting());
        }
        
        [Fact]
        public void Simulator_WithZeroExtraAllocation_ShouldStillProcessMinimumPayments()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(100.00m, 25.00m),
                new Debt(200.00m, 50.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 0.00m);
            
            // Act
            simulator.ProcessMonthForTesting();
            
            // Assert
            Assert.Equal(75.00m, debts[0].Balance); // 100 - 25
            Assert.Equal(150.00m, debts[1].Balance); // 200 - 50
        }
        
        [Fact]
        public void Simulator_WithZeroMinimumPayments_ShouldOnlyApplyExtraAllocation()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(100.00m, 0.00m),
                new Debt(200.00m, 0.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 50.00m);
            
            // Act
            simulator.ProcessMonthForTesting();
            
            // Assert
            Assert.Equal(50.00m, debts[0].Balance); // 100 - 50
            Assert.Equal(200.00m, debts[1].Balance); // No change
        }
        
        [Fact]
        public void Simulator_WithMinimumPaymentExceedingBalance_ShouldPayOffDebt()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(25.00m, 100.00m),  // Minimum payment exceeds balance
                new Debt(1000.00m, 50.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 200.00m);
            
            // Act
            simulator.ProcessMonthForTesting();
            
            // Assert
            Assert.Equal(0.00m, debts[0].Balance); // Paid off
            Assert.Equal(650.00m, debts[1].Balance); // 1000 - 50 - (200 + 100)
            Assert.Equal(300.00m, simulator.GetMonthlyAllocationForTesting()); // 200 + 100
        }
        
        [Fact]
        public void Simulator_WithVeryLargeExtraAllocation_ShouldPayOffAllDebtsQuickly()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(100.00m, 25.00m),
                new Debt(1000.00m, 50.00m),
                new Debt(10000.00m, 200.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 50000.00m);
            
            // Act
            simulator.ProcessMonthForTesting();
            
            // Assert
            Assert.True(simulator.AllDebtsPaidOffForTesting());
            Assert.Equal(50275.00m, simulator.GetMonthlyAllocationForTesting()); // 50000 + 25 + 50 + 200
        }
        
        [Fact]
        public void Simulator_WithMixedPaidOffAndUnpaidDebts_ShouldHandleCorrectly()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(0.00m, 25.00m),    // Already paid off
                new Debt(100.00m, 25.00m),   // Unpaid
                new Debt(0.00m, 50.00m)     // Already paid off
            };
            var simulator = new DebtSnowballSimulator(debts, 100.00m);
            
            // Act
            simulator.ProcessMonthForTesting();
            
            // Assert
            Assert.Equal(0.00m, debts[0].Balance); // Still paid off
            Assert.Equal(0.00m, debts[1].Balance); // Now paid off
            Assert.Equal(0.00m, debts[2].Balance); // Still paid off
            Assert.Equal(125.00m, simulator.GetMonthlyAllocationForTesting()); // 100 + 25 (only one new payoff this month)
        }
        
        [Fact]
        public void Simulator_WithSingleDebtEqualToExtraAllocation_ShouldPayOffCorrectly()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(100.00m, 25.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 100.00m);
            
            // Act
            simulator.ProcessMonthForTesting();
            
            // Assert
            Assert.Equal(0.00m, debts[0].Balance); // 100 - 25 - 100 = 0
            Assert.True(simulator.AllDebtsPaidOffForTesting());
            Assert.Equal(125.00m, simulator.GetMonthlyAllocationForTesting()); // 100 + 25
        }
        
        [Fact]
        public void Simulator_WithDebtSmallerThanExtraAllocation_ShouldPayOffAndReturnExcess()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(50.00m, 25.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 100.00m);
            
            // Act
            simulator.ProcessMonthForTesting();
            
            // Assert
            Assert.Equal(0.00m, debts[0].Balance);
            Assert.True(simulator.AllDebtsPaidOffForTesting());
            Assert.Equal(125.00m, simulator.GetMonthlyAllocationForTesting()); // 100 + 25
        }
    }
}




using Xunit;
using DebtSnowballApp;
using System.Collections.Generic;
using System.Linq;

namespace DebtSnowballApp.Tests
{
    public class DebtSnowballSimulatorTests
    {
        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(1000.00m, 50.00m),
                new Debt(2000.00m, 100.00m)
            };
            var monthlyAllocation = 200.00m;
            
            // Act
            var simulator = new DebtSnowballSimulator(debts, monthlyAllocation);
            
            // Assert
            Assert.Equal(debts, simulator.GetDebtsForTesting());
            Assert.Equal(monthlyAllocation, simulator.GetMonthlyAllocationForTesting());
            Assert.Equal(0, simulator.GetCurrentMonthForTesting());
        }
        
        [Fact]
        public void AllDebtsPaidOff_WhenNoDebts_ShouldReturnTrue()
        {
            // Arrange
            var simulator = new DebtSnowballSimulator(new List<Debt>(), 100.00m);
            
            // Act & Assert
            Assert.True(simulator.AllDebtsPaidOffForTesting());
        }
        
        [Fact]
        public void AllDebtsPaidOff_WhenAllDebtsHaveZeroBalance_ShouldReturnTrue()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(0.00m, 50.00m),
                new Debt(0.00m, 100.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 100.00m);
            
            // Act & Assert
            Assert.True(simulator.AllDebtsPaidOffForTesting());
        }
        
        [Fact]
        public void AllDebtsPaidOff_WhenSomeDebtsHavePositiveBalance_ShouldReturnFalse()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(0.00m, 50.00m),
                new Debt(500.00m, 100.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 100.00m);
            
            // Act & Assert
            Assert.False(simulator.AllDebtsPaidOffForTesting());
        }
        
        [Fact]
        public void ProcessMonth_ShouldApplyMinimumPaymentsToAllUnpaidDebts()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(1000.00m, 50.00m),
                new Debt(2000.00m, 100.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 200.00m);
            
            // Act
            simulator.ProcessMonthForTesting();
            
            // Assert (min payments + cascading extra on smallest)
            Assert.Equal(750.00m, debts[0].Balance); // 1000 - 50 - 200
            Assert.Equal(1900.00m, debts[1].Balance); // 2000 - 100
        }
        
        [Fact]
        public void ProcessMonth_ShouldApplyExtraAllocationToSmallestDebt()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(100.00m, 25.00m),  // Smallest debt
                new Debt(1000.00m, 50.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 200.00m);
            
            // Act
            simulator.ProcessMonthForTesting();
            
            // Assert (75 of extra rolls over to next debt)
            Assert.Equal(0.00m, debts[0].Balance);
            Assert.Equal(825.00m, debts[1].Balance); // 1000 - 50 - 125
        }
        
        [Fact]
        public void ProcessMonth_WhenDebtIsPaidOffByMinimumPayment_ShouldIncreaseAllocation()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(25.00m, 25.00m),  // Will be paid off by minimum payment
                new Debt(1000.00m, 50.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 100.00m);
            
            // Act
            simulator.ProcessMonthForTesting();
            
            // Assert
            Assert.Equal(0.00m, debts[0].Balance);
            Assert.Equal(125.00m, simulator.GetMonthlyAllocationForTesting()); // 100 + 25
        }
        
        [Fact]
        public void ProcessMonth_WhenDebtIsPaidOffByExtraAllocation_ShouldIncreaseAllocation()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(100.00m, 25.00m),  // Will be paid off by extra allocation
                new Debt(1000.00m, 50.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 200.00m);
            
            // Act
            simulator.ProcessMonthForTesting();
            
            // Assert
            Assert.Equal(0.00m, debts[0].Balance);
            Assert.Equal(225.00m, simulator.GetMonthlyAllocationForTesting()); // 200 + 25
        }
        
        [Fact]
        public void ProcessMonth_ShouldHandleMultipleDebtsPaidOffInSameMonth()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(20.00m, 25.00m),  // Will be paid off by minimum payment
                new Debt(50.00m, 25.00m),  // Will be paid off by extra allocation
                new Debt(1000.00m, 50.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 100.00m);
            
            // Act
            simulator.ProcessMonthForTesting();
            
            // Assert
            Assert.Equal(0.00m, debts[0].Balance);
            Assert.Equal(0.00m, debts[1].Balance);
            Assert.Equal(150.00m, simulator.GetMonthlyAllocationForTesting()); // 100 + 25 + 25
        }
        
        [Fact]
        public void ProcessMonth_ShouldNotExceedDebtBalanceWhenMakingPayments()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(50.00m, 100.00m),  // Minimum payment exceeds balance
                new Debt(1000.00m, 50.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 200.00m);
            
            // Act
            simulator.ProcessMonthForTesting();
            
            // Assert
            Assert.Equal(0.00m, debts[0].Balance); // Paid off by minimum payment
            Assert.Equal(650.00m, debts[1].Balance); // 1000 - 50 - (200 + 100)
        }
        
        [Fact]
        public void ProcessMonth_ShouldHandleZeroMinimumPayments()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(1000.00m, 0.00m),
                new Debt(2000.00m, 0.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 100.00m);
            
            // Act
            simulator.ProcessMonthForTesting();
            
            // Assert
            Assert.Equal(900.00m, debts[0].Balance); // 1000 - 100 (only extra allocation)
            Assert.Equal(2000.00m, debts[1].Balance); // No change
        }
        
        [Fact]
        public void ProcessMonth_ShouldHandleZeroExtraAllocation()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(1000.00m, 50.00m),
                new Debt(2000.00m, 100.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 0.00m);
            
            // Act
            simulator.ProcessMonthForTesting();
            
            // Assert
            Assert.Equal(950.00m, debts[0].Balance); // 1000 - 50
            Assert.Equal(1900.00m, debts[1].Balance); // 2000 - 100
        }
        
        [Fact]
        public void ProcessMonth_ShouldTrackMonthlyAllocationsCorrectly()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(100.00m, 25.00m),
                new Debt(1000.00m, 50.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 100.00m);
            
            // Act
            simulator.ProcessMonthForTesting();
            
            // Assert (month counter only increments in RunSimulation)
            Assert.Equal(0, simulator.GetCurrentMonthForTesting());
        }
        
        [Theory]
        [InlineData("100.00", "25.00", "125.00")]
        [InlineData("200.00", "50.00", "250.00")]
        [InlineData("0.00", "25.00", "25.00")]
        public void ProcessMonth_ShouldIncreaseAllocationByCorrectAmount(string originalAllocationStr, string minPaymentStr, string expectedAllocationStr)
        {
            // Arrange
            var originalAllocation = decimal.Parse(originalAllocationStr, System.Globalization.CultureInfo.InvariantCulture);
            var minPayment = decimal.Parse(minPaymentStr, System.Globalization.CultureInfo.InvariantCulture);
            var expectedAllocation = decimal.Parse(expectedAllocationStr, System.Globalization.CultureInfo.InvariantCulture);
            var debts = new List<Debt>
            {
                new Debt(25.00m, minPayment),  // Will be paid off
                new Debt(1000.00m, 50.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, originalAllocation);
            
            // Act
            simulator.ProcessMonthForTesting();
            
            // Assert
            Assert.Equal(expectedAllocation, simulator.GetMonthlyAllocationForTesting());
        }
        
        [Fact]
        public void ProcessMonth_ShouldHandleDebtsInCorrectOrder()
        {
            // Arrange - Note: Program.cs sorts debts by balance, so smallest comes first
            var debts = new List<Debt>
            {
                new Debt(50.00m, 25.00m),   // Smallest - should get extra allocation
                new Debt(100.00m, 25.00m),  // Medium
                new Debt(1000.00m, 50.00m)  // Largest
            };
            var simulator = new DebtSnowballSimulator(debts, 100.00m);
            
            // Act
            simulator.ProcessMonthForTesting();
            
            // Assert (cascading extra pays off first two)
            Assert.Equal(0.00m, debts[0].Balance);
            Assert.Equal(0.00m, debts[1].Balance);
            Assert.Equal(950.00m, debts[2].Balance); // 1000 - 50
        }
    }
}




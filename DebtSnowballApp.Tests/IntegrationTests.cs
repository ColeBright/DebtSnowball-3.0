using Xunit;
using DebtSnowballApp;
using System.Collections.Generic;
using System.Linq;

namespace DebtSnowballApp.Tests
{
    public class IntegrationTests
    {
        [Fact]
        public void FullSimulation_SingleDebt_ShouldPayOffInOneMonth()
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
            Assert.True(simulator.AllDebtsPaidOffForTesting());
            Assert.Equal(125.00m, simulator.GetMonthlyAllocationForTesting()); // 100 + 25
        }
        
        [Fact]
        public void FullSimulation_TwoDebts_ShouldPayOffSmallestFirst()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(50.00m, 25.00m),   // Smallest
                new Debt(1000.00m, 50.00m)  // Largest
            };
            var simulator = new DebtSnowballSimulator(debts, 100.00m);
            
            // Act - Month 1
            simulator.ProcessMonthForTesting();
            
            // Assert - First debt should be paid off, leftover extra reduces second
            Assert.Equal(0.00m, debts[0].Balance);
            Assert.Equal(875.00m, debts[1].Balance); // 1000 - 50 - 75
            Assert.Equal(125.00m, simulator.GetMonthlyAllocationForTesting()); // 100 + 25
            
            // Act - Month 2
            simulator.ProcessMonthForTesting();
            
            // Assert - Second debt reduced by min+extra
            Assert.Equal(700.00m, debts[1].Balance); // 875 - 50 - 125
        }
        
        [Fact]
        public void FullSimulation_ThreeDebts_ShouldShowSnowballEffect()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(25.00m, 25.00m),   // Will be paid off by minimum payment
                new Debt(100.00m, 25.00m),  // Will be paid off by extra allocation
                new Debt(1000.00m, 50.00m)  // Largest debt
            };
            var simulator = new DebtSnowballSimulator(debts, 100.00m);
            
            // Act - Month 1
            simulator.ProcessMonthForTesting();
            
            // Assert - First two debts should be paid off; leftover reduces third by 50
            Assert.Equal(0.00m, debts[0].Balance);
            Assert.Equal(0.00m, debts[1].Balance);
            Assert.Equal(900.00m, debts[2].Balance); // 1000 - 50 - 50
            
            // Act - Month 2
            simulator.ProcessMonthForTesting();
            
            // Assert - Third debt should be reduced
            Assert.Equal(700.00m, debts[2].Balance); // 900 - (50 + 150)
        }
        
        [Fact]
        public void FullSimulation_ZeroExtraAllocation_ShouldStillPayOffDebtsEventually()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(100.00m, 25.00m),
                new Debt(200.00m, 25.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 0.00m);
            
            // Act - Month 1
            simulator.ProcessMonthForTesting();
            
            // Assert - Only minimum payments applied
            Assert.Equal(75.00m, debts[0].Balance); // 100 - 25
            Assert.Equal(175.00m, debts[1].Balance); // 200 - 25
            
            // Act - Month 2
            simulator.ProcessMonthForTesting();
            
            // Assert - First debt not yet paid off (no extra allocation yet)
            Assert.Equal(50.00m, debts[0].Balance); // 75 - 25
            Assert.Equal(150.00m, debts[1].Balance); // 175 - 25
            Assert.Equal(0.00m, simulator.GetMonthlyAllocationForTesting());
        }
        
        [Fact]
        public void FullSimulation_LargeExtraAllocation_ShouldPayOffDebtsQuickly()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(100.00m, 25.00m),
                new Debt(200.00m, 25.00m),
                new Debt(500.00m, 50.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 1000.00m);
            
            // Act - Month 1
            simulator.ProcessMonthForTesting();
            
            // Assert - All debts should be paid off in one month
            Assert.True(simulator.AllDebtsPaidOffForTesting());
            Assert.Equal(1100.00m, simulator.GetMonthlyAllocationForTesting()); // 1000 + 25 + 25 + 50
        }
        
        [Fact]
        public void FullSimulation_EdgeCase_AllDebtsHaveZeroBalance()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(0.00m, 25.00m),
                new Debt(0.00m, 50.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 100.00m);
            
            // Act & Assert
            Assert.True(simulator.AllDebtsPaidOffForTesting());
            
            // Process month should not change anything
            simulator.ProcessMonthForTesting();
            Assert.True(simulator.AllDebtsPaidOffForTesting());
        }
        
        [Fact]
        public void FullSimulation_EdgeCase_AllDebtsHaveZeroMinimumPayments()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(100.00m, 0.00m),
                new Debt(200.00m, 0.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 50.00m);
            
            // Act - Month 1
            simulator.ProcessMonthForTesting();
            
            // Assert - Only extra allocation applied to smallest debt
            Assert.Equal(50.00m, debts[0].Balance); // 100 - 50
            Assert.Equal(200.00m, debts[1].Balance); // No change
        }
        
        [Fact]
        public void FullSimulation_RealisticScenario_ShouldCompleteSuccessfully()
        {
            // Arrange - Realistic debt scenario
            var debts = new List<Debt>
            {
                new Debt(500.00m, 25.00m),   // Credit card
                new Debt(2000.00m, 100.00m), // Car loan
                new Debt(10000.00m, 200.00m) // Student loan
            };
            var simulator = new DebtSnowballSimulator(debts, 300.00m);
            
            // Act - Month 1
            simulator.ProcessMonthForTesting();
            
            // Assert - First debt partially paid by extra; no payoff yet
            Assert.Equal(175.00m, debts[0].Balance); // 500 - 25 - 300
            Assert.Equal(1900.00m, debts[1].Balance); // 2000 - 100
            Assert.Equal(9800.00m, debts[2].Balance); // 10000 - 200
            Assert.Equal(300.00m, simulator.GetMonthlyAllocationForTesting());
            
            // Act - Month 2
            simulator.ProcessMonthForTesting();
            
            // Assert - Debt1 paid off; extra now 325, reduces debt2
            Assert.Equal(1650.00m, debts[1].Balance); // 1900 - 100 - 150
            Assert.Equal(9600.00m, debts[2].Balance); // 9800 - 200
            Assert.Equal(325.00m, simulator.GetMonthlyAllocationForTesting());
        }
        
        [Fact]
        public void FullSimulation_ShouldHandleMinimumPaymentExceedingBalance()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(20.00m, 50.00m),   // Minimum payment exceeds balance
                new Debt(1000.00m, 25.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 100.00m);
            
            // Act - Month 1
            simulator.ProcessMonthForTesting();
            
            // Assert - First debt paid off by minimum payment, allocation increased; extra cascades
            Assert.Equal(0.00m, debts[0].Balance);
            Assert.Equal(825.00m, debts[1].Balance); // 1000 - 25 - (100 + 50)
            Assert.Equal(150.00m, simulator.GetMonthlyAllocationForTesting()); // 100 + 50
        }
        
        [Fact]
        public void FullSimulation_ShouldTrackMonthCountCorrectly()
        {
            // Arrange
            var debts = new List<Debt>
            {
                new Debt(1000.00m, 25.00m)
            };
            var simulator = new DebtSnowballSimulator(debts, 100.00m);
            
            // Act & Assert
            Assert.Equal(0, simulator.GetCurrentMonthForTesting());
            
            simulator.ProcessMonthForTesting();
            Assert.Equal(0, simulator.GetCurrentMonthForTesting());
            
            simulator.RunSimulation();
            Assert.True(simulator.AllDebtsPaidOffForTesting());
        }
    }
}




using System;
using System.Collections.Generic;
using System.Linq;

namespace DebtSnowballApp
{
    public class DebtSnowballSimulator
    {
        private readonly List<Debt> _debts;
        private decimal _monthlyAllocation;
        private int _currentMonth;
        private decimal _originalAllocation;
        private List<decimal> _monthlyAllocations;
        
        public DebtSnowballSimulator(List<Debt> debts, decimal monthlyAllocation)
        {
            _debts = debts;
            _monthlyAllocation = monthlyAllocation;
            _originalAllocation = monthlyAllocation;
            _currentMonth = 0;
            _monthlyAllocations = new List<decimal>();
        }
        
        public void RunSimulation()
        {
            Console.WriteLine($"Starting debt snowball with ${_monthlyAllocation:F2} extra monthly allocation (on top of minimum payments).\n");
            Console.WriteLine("Initial debt balances:");
            PrintDebtStatus();
            Console.WriteLine();
            
            while (!AllDebtsPaidOff())
            {
                _currentMonth++;
                ProcessMonth();
                PrintMonthSummary();
                
                if (_currentMonth >= 600) // Prevent infinite loops (50 years)
                {
                    Console.WriteLine("Simulation stopped after 50 years to prevent infinite loop.");
                    break;
                }
            }
            
            PrintFinalSummary();
        }
        
        private void ProcessMonth()
        {
            // Track which debts were paid off this month to increase allocation (avoid double counting)
            var debtsBeforePayment = _debts.Where(d => !d.IsPaidOff).ToList();
            var processedThisMonth = new HashSet<Debt>();
            
            // First, make minimum payments to all unpaid debts (these don't count against monthly allocation)
            foreach (var debt in _debts.Where(d => !d.IsPaidOff))
            {
                decimal minPayment = Math.Min(debt.MinimumPayment, debt.Balance);
                if (minPayment > 0)
                {
                    debt.MakePayment(minPayment);
                }
            }
            
            // Check if any debts were paid off by minimum payments and increase allocation
            CheckForNewlyPaidDebts(debtsBeforePayment, processedThisMonth);
            
            // Then, apply the monthly allocation to debts in snowball order, cascading leftover within the same month
            decimal remainingExtra = _monthlyAllocation;
            while (remainingExtra > 0)
            {
                var nextDebt = _debts.FirstOrDefault(d => !d.IsPaidOff);
                if (nextDebt == null)
                {
                    break;
                }
                decimal beforeBalance = nextDebt.Balance;
                decimal leftover = nextDebt.MakePayment(remainingExtra);
                if (nextDebt.IsPaidOff && beforeBalance > 0)
                {
                    // Increase allocation once for this debt
                    processedThisMonth.Add(nextDebt);
                    _monthlyAllocation += nextDebt.MinimumPayment;
                    Console.WriteLine($"🎉 Debt paid off! Monthly allocation increased by ${nextDebt.MinimumPayment:F2} to ${_monthlyAllocation:F2}");
                }
                // Prevent infinite loop if nothing was applied
                if (leftover == remainingExtra)
                {
                    break;
                }
                remainingExtra = leftover;
            }
            
            // Record this month's allocation for accurate total calculation
            _monthlyAllocations.Add(_monthlyAllocation);
        }
        
        private void PrintMonthSummary()
        {
            Console.WriteLine($"=== Month {_currentMonth} ===");
            
            var unpaidDebts = _debts.Where(d => !d.IsPaidOff).ToList();
            if (unpaidDebts.Count == 0)
            {
                Console.WriteLine("🎉 ALL DEBTS PAID OFF! 🎉");
                return;
            }
            
            Console.WriteLine($"Remaining debts: {unpaidDebts.Count}");
            Console.WriteLine($"Total remaining balance: ${unpaidDebts.Sum(d => d.Balance):F2}");
            
            foreach (var debt in unpaidDebts)
            {
                Console.WriteLine($"  - {debt}");
            }
            Console.WriteLine();
        }
        
        private void PrintDebtStatus()
        {
            for (int i = 0; i < _debts.Count; i++)
            {
                var debt = _debts[i];
                string status = debt.IsPaidOff ? "✅ PAID OFF" : $"Balance: ${debt.Balance:F2}";
                Console.WriteLine($"Debt #{i + 1}: {status}");
            }
        }
        
        private void PrintFinalSummary()
        {
            decimal totalMinPayments = _debts.Sum(d => d.MinimumPayment) * _currentMonth;
            decimal totalExtraPayments = CalculateTotalExtraPayments();
            decimal totalAmountPaid = totalMinPayments + totalExtraPayments;
            
            Console.WriteLine("\n=== FINAL SUMMARY ===");
            Console.WriteLine($"Total months to pay off all debts: {_currentMonth}");
            Console.WriteLine($"Total years: {_currentMonth / 12.0:F1}");
            Console.WriteLine($"Total minimum payments: ${totalMinPayments:F2}");
            Console.WriteLine($"Total extra payments: ${totalExtraPayments:F2}");
            Console.WriteLine($"Total amount paid: ${totalAmountPaid:F2}");
            Console.WriteLine($"Final monthly allocation: ${_monthlyAllocation:F2} (started at ${_originalAllocation:F2})");
            Console.WriteLine("\n🎉 Congratulations! You're debt-free! 🎉");
        }
        
        private void CheckForNewlyPaidDebts(List<Debt> debtsBeforePayment, HashSet<Debt> processedThisMonth)
        {
            var newlyPaidDebts = debtsBeforePayment.Where(d => d.IsPaidOff && !processedThisMonth.Contains(d)).ToList();
            foreach (var debt in newlyPaidDebts)
            {
                processedThisMonth.Add(debt);
                _monthlyAllocation += debt.MinimumPayment;
                Console.WriteLine($"🎉 Debt paid off! Monthly allocation increased by ${debt.MinimumPayment:F2} to ${_monthlyAllocation:F2}");
            }
        }
        
        private decimal CalculateTotalExtraPayments()
        {
            return _monthlyAllocations.Sum();
        }
        
        private bool AllDebtsPaidOff()
        {
            return _debts.All(d => d.IsPaidOff);
        }
        
        // Test helper methods - only for unit testing
        #if DEBUG
        public List<Debt> GetDebtsForTesting() => _debts;
        public decimal GetMonthlyAllocationForTesting() => _monthlyAllocation;
        public int GetCurrentMonthForTesting() => _currentMonth;
        public bool AllDebtsPaidOffForTesting() => AllDebtsPaidOff();
        public void ProcessMonthForTesting() => ProcessMonth();
        #endif
    }
} 
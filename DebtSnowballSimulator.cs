using System;
using System.Collections.Generic;
using System.Linq;

namespace DebtSnowballApp
{
    public class DebtSnowballSimulator
    {
        private readonly List<Debt> _debts;
        private readonly decimal _monthlyAllocation;
        private int _currentMonth;
        
        public DebtSnowballSimulator(List<Debt> debts, decimal monthlyAllocation)
        {
            _debts = debts;
            _monthlyAllocation = monthlyAllocation;
            _currentMonth = 0;
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
            // First, make minimum payments to all unpaid debts (these don't count against monthly allocation)
            foreach (var debt in _debts.Where(d => !d.IsPaidOff))
            {
                decimal minPayment = Math.Min(debt.MinimumPayment, debt.Balance);
                if (minPayment > 0)
                {
                    debt.MakePayment(minPayment);
                }
            }
            
            // Then, apply the full monthly allocation to the smallest debt (debt snowball method)
            var smallestDebt = _debts.FirstOrDefault(d => !d.IsPaidOff);
            if (smallestDebt != null)
            {
                smallestDebt.MakePayment(_monthlyAllocation);
            }
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
            decimal totalExtraPayments = _monthlyAllocation * _currentMonth;
            decimal totalAmountPaid = totalMinPayments + totalExtraPayments;
            
            Console.WriteLine("\n=== FINAL SUMMARY ===");
            Console.WriteLine($"Total months to pay off all debts: {_currentMonth}");
            Console.WriteLine($"Total years: {_currentMonth / 12.0:F1}");
            Console.WriteLine($"Total minimum payments: ${totalMinPayments:F2}");
            Console.WriteLine($"Total extra payments: ${totalExtraPayments:F2}");
            Console.WriteLine($"Total amount paid: ${totalAmountPaid:F2}");
            Console.WriteLine("\n🎉 Congratulations! You're debt-free! 🎉");
        }
        
        private bool AllDebtsPaidOff()
        {
            return _debts.All(d => d.IsPaidOff);
        }
    }
} 
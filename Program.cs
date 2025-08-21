using System;
using System.Collections.Generic;
using System.Linq;

namespace DebtSnowballApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Debt Snowball Calculator ===\n");
            
            // Get monthly allocation amount
            decimal monthlyAllocation = GetMonthlyAllocation();
            
            // Get debt information
            List<Debt> debts = GetDebts();
            
            if (debts.Count == 0)
            {
                Console.WriteLine("No debts entered. Exiting...");
                return;
            }
            
            // Sort debts by balance (smallest first for debt snowball method)
            debts = debts.OrderBy(d => d.Balance).ToList();
            
            Console.WriteLine("\n=== Debt Snowball Simulation ===\n");
            
            // Run the simulation
            DebtSnowballSimulator simulator = new DebtSnowballSimulator(debts, monthlyAllocation);
            simulator.RunSimulation();
        }
        
        static decimal GetMonthlyAllocation()
        {
            while (true)
            {
                Console.Write("Enter the EXTRA amount you can pay toward debt each month (on top of minimum payments): $");
                if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
                {
                    return amount;
                }
                Console.WriteLine("Please enter a valid positive amount.");
            }
        }
        
        static List<Debt> GetDebts()
        {
            List<Debt> debts = new List<Debt>();
            
            Console.WriteLine("\nEnter your debts (one per line).");
            Console.WriteLine("Format: Balance,MinimumPayment (e.g., 5000.00,150.00)");
            Console.WriteLine("Enter an empty line when finished.\n");
            
            int debtNumber = 1;
            while (true)
            {
                Console.Write($"Debt #{debtNumber}: ");
                string? input = Console.ReadLine();
                
                if (string.IsNullOrWhiteSpace(input))
                {
                    break;
                }
                
                string[] parts = input.Split(',');
                if (parts.Length == 2 && 
                    decimal.TryParse(parts[0], out decimal balance) && 
                    decimal.TryParse(parts[1], out decimal minPayment) &&
                    balance > 0 && minPayment > 0)
                {
                    debts.Add(new Debt(balance, minPayment));
                    debtNumber++;
                }
                else
                {
                    Console.WriteLine("Invalid format. Please use: Balance,MinimumPayment");
                }
            }
            
            return debts;
        }
    }
} 
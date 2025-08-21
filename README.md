# Debt Snowball Calculator

A C# console application that simulates debt payoff using the debt snowball method.

## What is the Debt Snowball Method?

The debt snowball method is a debt reduction strategy where you pay off debts in order of smallest to largest balance, gaining momentum as each balance is paid off. You continue to make minimum payments on all debts, but put any extra money toward the debt with the smallest balance.

## Features

- Input monthly allocation amount
- Add multiple debts with balances and minimum payments
- Simulate monthly payments until all debts are paid off
- View month-by-month progress
- See total time and amount needed to become debt-free

## How to Use

1. **Build and run the application:**
   ```bash
   dotnet build
   dotnet run
   ```

2. **Enter your monthly allocation:**
   - Input the total amount you can afford to pay toward debt each month

3. **Add your debts:**
   - Enter each debt on a separate line
   - Format: `Balance,MinimumPayment`
   - Example: `5000.00,150.00`
   - Press Enter on an empty line when finished

4. **Watch the simulation:**
   - The app will show month-by-month progress
   - See which debts get paid off first
   - View the final summary with total time and cost

## Example Input

```
Enter the amount you can allocate to debt payments each month: $1000

Enter your debts (one per line).
Format: Balance,MinimumPayment (e.g., 5000.00,150.00)
Enter an empty line when finished.

Debt #1: 2500.00,75.00
Debt #2: 8000.00,200.00
Debt #3: 15000.00,300.00
Debt #4: [Enter]
```

## Requirements

- .NET 8.0 or later
- Windows, macOS, or Linux

## How It Works

1. **Sort debts** by balance (smallest first)
2. **Each month:**
   - Make minimum payments to all unpaid debts
   - Apply remaining allocation to the smallest unpaid debt
3. **Continue** until all debts are paid off
4. **Track progress** month by month

## Benefits of the Debt Snowball Method

- **Psychological wins:** Paying off smaller debts first provides motivation
- **Simplified focus:** Concentrate extra payments on one debt at a time
- **Faster progress:** Each paid-off debt frees up more money for the next
- **Reduced stress:** Fewer creditors to manage as debts are eliminated

## Notes

- The simulation assumes no interest accrual (for simplicity)
- All payments are applied at the beginning of each month
- The app prevents infinite loops by limiting to 50 years maximum
- Use this as a planning tool alongside your actual debt management strategy 
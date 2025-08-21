namespace DebtSnowballApp
{
    public class Debt
    {
        public decimal Balance { get; set; }
        public decimal MinimumPayment { get; set; }
        public bool IsPaidOff => Balance <= 0;
        
        public Debt(decimal balance, decimal minimumPayment)
        {
            Balance = balance;
            MinimumPayment = minimumPayment;
        }
        
        public decimal MakePayment(decimal paymentAmount)
        {
            if (IsPaidOff)
                return paymentAmount;
                
            decimal actualPayment = Math.Min(paymentAmount, Balance);
            Balance -= actualPayment;
            
            return paymentAmount - actualPayment; // Return any unused portion
        }
        
        public override string ToString()
        {
            return $"Balance: ${Balance:F2}, Min Payment: ${MinimumPayment:F2}";
        }
    }
} 
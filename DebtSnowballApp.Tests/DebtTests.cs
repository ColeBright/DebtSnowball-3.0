using Xunit;
using DebtSnowballApp;

namespace DebtSnowballApp.Tests
{
    public class DebtTests
    {
        [Fact]
        public void Constructor_ShouldSetBalanceAndMinimumPayment()
        {
            // Arrange & Act
            var debt = new Debt(1000.00m, 50.00m);
            
            // Assert
            Assert.Equal(1000.00m, debt.Balance);
            Assert.Equal(50.00m, debt.MinimumPayment);
        }
        
        [Fact]
        public void IsPaidOff_WhenBalanceIsZero_ShouldReturnTrue()
        {
            // Arrange
            var debt = new Debt(0.00m, 50.00m);
            
            // Act & Assert
            Assert.True(debt.IsPaidOff);
        }
        
        [Fact]
        public void IsPaidOff_WhenBalanceIsNegative_ShouldReturnTrue()
        {
            // Arrange
            var debt = new Debt(-10.00m, 50.00m);
            
            // Act & Assert
            Assert.True(debt.IsPaidOff);
        }
        
        [Fact]
        public void IsPaidOff_WhenBalanceIsPositive_ShouldReturnFalse()
        {
            // Arrange
            var debt = new Debt(100.00m, 50.00m);
            
            // Act & Assert
            Assert.False(debt.IsPaidOff);
        }
        
        [Fact]
        public void MakePayment_WhenPaymentIsLessThanBalance_ShouldReduceBalanceAndReturnZero()
        {
            // Arrange
            var debt = new Debt(1000.00m, 50.00m);
            var paymentAmount = 300.00m;
            
            // Act
            var unusedAmount = debt.MakePayment(paymentAmount);
            
            // Assert
            Assert.Equal(700.00m, debt.Balance);
            Assert.Equal(0.00m, unusedAmount);
        }
        
        [Fact]
        public void MakePayment_WhenPaymentEqualsBalance_ShouldReduceBalanceToZeroAndReturnZero()
        {
            // Arrange
            var debt = new Debt(1000.00m, 50.00m);
            var paymentAmount = 1000.00m;
            
            // Act
            var unusedAmount = debt.MakePayment(paymentAmount);
            
            // Assert
            Assert.Equal(0.00m, debt.Balance);
            Assert.Equal(0.00m, unusedAmount);
            Assert.True(debt.IsPaidOff);
        }
        
        [Fact]
        public void MakePayment_WhenPaymentExceedsBalance_ShouldReduceBalanceToZeroAndReturnExcess()
        {
            // Arrange
            var debt = new Debt(1000.00m, 50.00m);
            var paymentAmount = 1500.00m;
            
            // Act
            var unusedAmount = debt.MakePayment(paymentAmount);
            
            // Assert
            Assert.Equal(0.00m, debt.Balance);
            Assert.Equal(500.00m, unusedAmount);
            Assert.True(debt.IsPaidOff);
        }
        
        [Fact]
        public void MakePayment_WhenDebtIsAlreadyPaidOff_ShouldReturnFullPaymentAmount()
        {
            // Arrange
            var debt = new Debt(0.00m, 50.00m);
            var paymentAmount = 100.00m;
            
            // Act
            var unusedAmount = debt.MakePayment(paymentAmount);
            
            // Assert
            Assert.Equal(0.00m, debt.Balance);
            Assert.Equal(100.00m, unusedAmount);
        }
        
        [Fact]
        public void MakePayment_WhenPaymentIsZero_ShouldNotChangeBalance()
        {
            // Arrange
            var debt = new Debt(1000.00m, 50.00m);
            var originalBalance = debt.Balance;
            
            // Act
            var unusedAmount = debt.MakePayment(0.00m);
            
            // Assert
            Assert.Equal(originalBalance, debt.Balance);
            Assert.Equal(0.00m, unusedAmount);
        }
        
        [Fact]
        public void ToString_ShouldReturnFormattedString()
        {
            // Arrange
            var debt = new Debt(1234.56m, 78.90m);
            
            // Act
            var result = debt.ToString();
            
            // Assert
            Assert.Equal("Balance: $1234.56, Min Payment: $78.90", result);
        }
        
        [Theory]
        [InlineData("0.01", "0.01")]
        [InlineData("100.00", "25.00")]
        [InlineData("9999.99", "500.00")]
        public void Constructor_WithValidAmounts_ShouldCreateDebt(string balanceStr, string minPaymentStr)
        {
            // Arrange
            var balance = decimal.Parse(balanceStr, System.Globalization.CultureInfo.InvariantCulture);
            var minPayment = decimal.Parse(minPaymentStr, System.Globalization.CultureInfo.InvariantCulture);
            
            // Act
            var debt = new Debt(balance, minPayment);
            
            // Assert
            Assert.Equal(balance, debt.Balance);
            Assert.Equal(minPayment, debt.MinimumPayment);
        }
        
        [Theory]
        [InlineData("1000.00", "100.00", "100.00", "900.00")]
        [InlineData("500.00", "50.00", "75.00", "425.00")]
        [InlineData("200.00", "25.00", "200.00", "0.00")]
        public void MakePayment_WithVariousAmounts_ShouldCalculateCorrectly(string initialBalanceStr, string minPaymentStr, string paymentAmountStr, string expectedBalanceStr)
        {
            // Arrange
            var initialBalance = decimal.Parse(initialBalanceStr, System.Globalization.CultureInfo.InvariantCulture);
            var minPayment = decimal.Parse(minPaymentStr, System.Globalization.CultureInfo.InvariantCulture);
            var paymentAmount = decimal.Parse(paymentAmountStr, System.Globalization.CultureInfo.InvariantCulture);
            var expectedBalance = decimal.Parse(expectedBalanceStr, System.Globalization.CultureInfo.InvariantCulture);
            
            var debt = new Debt(initialBalance, minPayment);
            
            // Act
            debt.MakePayment(paymentAmount);
            
            // Assert
            Assert.Equal(expectedBalance, debt.Balance);
        }
    }
}




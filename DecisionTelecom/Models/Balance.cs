namespace DecisionTelecom.Models
{
    /// <summary>
    /// Represents user money balance
    /// </summary>
    public class Balance
    {
        /// <summary>
        /// Current balance amount
        /// </summary>
        public double BalanceAmount { get; set; }
        
        /// <summary>
        /// Current credit line (if opened)
        /// </summary>
        public double CreditAmount { get; set; }
        
        /// <summary>
        /// Balance currency
        /// </summary>
        public string Currency { get; set; }
    }
}
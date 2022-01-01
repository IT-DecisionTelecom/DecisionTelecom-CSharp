using System.Text.Json.Serialization;

namespace DecisionTelecom.Models
{
    /// <summary>
    /// Represents user money balance
    /// </summary>
    public class SmsBalance
    {
        /// <summary>
        /// Current balance amount
        /// </summary>
        [JsonPropertyName("balance")]
        public double BalanceAmount { get; set; }
        
        /// <summary>
        /// Current credit line (if opened)
        /// </summary>
        [JsonPropertyName("credit")]
        public double CreditAmount { get; set; }
        
        /// <summary>
        /// Balance currency
        /// </summary>
        [JsonPropertyName("currency")]
        public string Currency { get; set; }
    }
}
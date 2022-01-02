using System.Text.Json.Serialization;

namespace DecisionTelecom.Models
{
    /// <summary>
    /// Represents Viber plus SMS message
    /// </summary>
    public class ViberPlusSmsMessage : ViberMessage
    {
        /// <summary>
        /// Alternative SMS message text for cases when Viber message is not sent.
        /// Supported only for transactional messages.
        /// <remarks> Should be less or equal than 70 characters for UCS-2 and less or equal 160 characters for Latin.
        /// This value should be set if it's necessary to resend transactional message via SMS
        /// in case of message via Viber was not delivered.
        /// </remarks>
        /// </summary>
        [JsonPropertyName("text_sms")]
        public string SmsText { get; set; }
    }
}
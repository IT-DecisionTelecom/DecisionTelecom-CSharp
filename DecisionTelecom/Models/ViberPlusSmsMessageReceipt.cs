using System.Text.Json.Serialization;

namespace DecisionTelecom.Models
{
    /// <summary>
    /// Contains information about Viber and SMS messages status
    /// </summary>
    public class ViberPlusSmsMessageReceipt : ViberMessageReceipt
    {
        /// <summary>
        /// SMS message Id (if available, only for transactional messages)
        /// </summary>
        [JsonPropertyName("sms_message_id")]
        public long SmsMessageId { get; set; }

        /// <summary>
        /// SMS message status (if available, only for transactional messages) 
        /// </summary>
        [JsonPropertyName("sms_message_status")]
        public SmsMessageStatus SmsMessageStatus { get; set; }
    }
}
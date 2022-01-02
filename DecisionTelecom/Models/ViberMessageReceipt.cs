using System.Text.Json.Serialization;

namespace DecisionTelecom.Models
{
    /// <summary>
    /// Represents Id and status of the particular Viber message
    /// </summary>
    public class ViberMessageReceipt
    {
        /// <summary>
        /// Id of the Viber message which status should be got (sent in the last 5 days)
        /// </summary>
        [JsonPropertyName("message_id")]
        public long ViberMessageId { get; set; }

        /// <summary>
        /// Viber message status
        /// </summary>
        [JsonPropertyName("status")]
        public ViberMessageStatus ViberMessageStatus { get; set; }
    }
}
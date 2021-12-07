using System.Text.Json.Serialization;

namespace DecisionTelecom.Models
{
    public class ViberMessageResult
    {
        /// <summary>
        /// Id of the Viber message which status should be got (sent in the last 5 days)
        /// </summary>
        [JsonPropertyName("message_id")]
        public int ViberMessageId { get; set; }

        /// <summary>
        /// Viber message status
        /// </summary>
        [JsonPropertyName("status")]
        public ViberMessageStatus ViberMessageStatus { get; set; }
    }
}
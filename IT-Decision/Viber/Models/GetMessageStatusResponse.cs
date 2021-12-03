using System.Text.Json.Serialization;

namespace ITDecision.Viber.Models
{
    public class GetMessageStatusResponse
    {
        [JsonPropertyName("message_id")]
        public int MessageId { get; set; }
        
        [JsonPropertyName("status")]
        public MessageStatus Status { get; set; }
    }
}
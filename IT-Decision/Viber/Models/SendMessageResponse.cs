using System.Text.Json.Serialization;

namespace ITDecision.Viber.Models
{
    public class SendMessageResponse
    {
        [JsonPropertyName("message_id")]
        public int MessageId { get; set; }
    }
}
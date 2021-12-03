using System.Text.Json.Serialization;

namespace ITDecision.Viber.Models
{
    public class GetMessageStatusRequest
    {
        [JsonPropertyName("message_id")]
        public int MessageId { get; set; }
    }
}
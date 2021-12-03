using System.Text.Json.Serialization;

namespace ITDecision.Viber.Models
{
    public class Error
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }
    }
}
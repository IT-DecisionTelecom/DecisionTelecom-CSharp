using System.Text.Json.Serialization;

namespace ITDecision.Models
{
    public class ViberError
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
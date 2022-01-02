using System.Text.Json.Serialization;

namespace DecisionTelecom.Models
{
    /// <summary>
    /// Represents error which may occur while working with Viber messages
    /// </summary>
    public class ViberError
    {
        /// <summary>
        /// Error name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// Error code
        /// </summary>
        [JsonPropertyName("code")]
        public int Code { get; set; }

        /// <summary>
        /// Error status
        /// </summary>
        [JsonPropertyName("status")]
        public int Status { get; set; }
    }
}
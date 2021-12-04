using System.Text.Json.Serialization;

namespace DecisionTelecom.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ViberMessage
    {
        /// <summary>
        /// Message sender (from whom message is sent)
        /// <remarks>Should be less or equal than 20 characters</remarks>
        /// </summary>
        [JsonPropertyName("source_addr")]
        public string Sender { get; set; }

        /// <summary>
        /// Message receiver (to whom message is sent)
        /// <remarks>Should be less or equal than 20 characters</remarks>
        /// </summary>
        [JsonPropertyName("destination_addr")]
        public string Receiver { get; set; }

        /// <summary>
        /// Message type
        /// </summary>
        [JsonPropertyName("message_type")]
        public ViberMessageType MessageType { get; set; }

        /// <summary>
        /// Message in the UTF8 format
        /// <remarks>Should be less or equal than 1000 characters</remarks>
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }

        /// <summary>
        /// Image URL for promotional message with button caption and button action
        /// <remarks>
        /// jpg or jpeg (mime type is image/jpeg), maximum resolution 400x400 pixels
        /// png (mime type is image/png), maximum resolution 400x400 pixels
        /// </remarks>
        /// </summary>
        [JsonPropertyName("image")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Button caption in the UTF8 format
        /// <remarks>Should be less or equal than 30 characters</remarks>
        /// </summary>
        [JsonPropertyName("button_caption")]
        public string ButtonCaption { get; set; }

        /// <summary>
        /// URL for transition when the button is pressed
        /// </summary>
        [JsonPropertyName("button_action")]
        public string ButtonAction { get; set; }

        /// <summary>
        /// Message sending procedure
        /// </summary>
        [JsonPropertyName("source_type")]
        public ViberMessageSourceType SourceType { get; set; }

        /// <summary>
        /// URL for message status callback
        /// </summary>
        [JsonPropertyName("callback_url")]
        public string CallbackUrl { get; set; }

        /// <summary>
        /// Allows sender to limit the life time of a message (in seconds)
        /// <remarks>
        /// In case the message did not get the status "delivered" before the time ended,
        /// the message will not be charged and will not be delivered to the user.
        /// In case no TTL was provided (no "ttl" parameter), Viber will try to deliver the message for up to 1 day.
        /// Minimum TTL value is 40 seconds. Maximum TTL value is 432000 seconds (5 days)
        /// </remarks>
        /// </summary>
        [JsonPropertyName("validity_period")]
        public int ValidityPeriod { get; set; }
    }
}
namespace DecisionTelecom.Models
{
    /// <summary>
    /// Represents SMS message
    /// </summary>
    public class SmsMessage
    {
        /// <summary>
        /// Message receiver phone number (MSISDN Destination)
        /// </summary>
        public string ReceiverPhone { get; set; }

        /// <summary>
        /// Message sender. Could be a mobile phone number (including a country code) or an alphanumeric string.
        /// <remarks>maximum length of alphanumeric string is 11 characters.</remarks>
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// Message body
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// True if a caller needs to obtain the delivery receipt in the future (by message id)
        /// </summary>
        public bool Delivery { get; set; }
    }
}
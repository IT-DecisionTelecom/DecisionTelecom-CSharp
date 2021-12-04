namespace ITDecision.Sms.Models
{
    public enum SmsMessageStatus
    {
        Unknown = 0,
        Delivered = 2,
        Expired = 3,
        Undeliverable = 5,
        Accepted = 6,
    }
}
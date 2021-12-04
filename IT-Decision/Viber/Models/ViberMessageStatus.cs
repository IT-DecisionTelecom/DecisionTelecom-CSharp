namespace ITDecision.Viber.Models
{
    public enum ViberMessageStatus
    {
        Sent = 0,
        Delivered = 1,
        Error = 2,
        Rejected = 3,
        Undelivered = 4,
        Pending = 5,
        Unknown = 20,
    }
}
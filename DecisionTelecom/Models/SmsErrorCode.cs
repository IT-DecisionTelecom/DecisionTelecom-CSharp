namespace DecisionTelecom.Models
{
    public enum SmsErrorCode
    {
        InvalidNumber = 40,
        IncorrectSender = 41,
        InvalidMessageId = 42,
        IncorrectJson = 43,
        InvalidLoginOrPassword = 44,
        UserLocked = 45,
        EmptyText = 46,
        EmptyLogin = 47,
        EmptyPassword = 48,
        NotEnoughMoney = 49,
        AuthorizationError = 50,
        InvalidPhoneNumber = 51,
    }
}
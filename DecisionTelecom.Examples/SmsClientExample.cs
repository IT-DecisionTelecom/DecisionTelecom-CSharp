using System;
using System.Threading.Tasks;
using DecisionTelecom.Exceptions;
using DecisionTelecom.Models;

namespace DecisionTelecom.Examples
{
    public static class SmsClientExample
    {
        public static async Task SendMessageAsync()
        {
            try
            {
                // Create new instance of the SmsClient
                var smsClient = new SmsClient("<YOUR_LOGIN>", "<YOUR_PASSWORD>");

                // Create message object
                var smsMessage = new SmsMessage
                {
                    Sender = "380504444444",
                    ReceiverPhone = "380505555555",
                    Text = "test message",
                    Delivery = true,
                };

                // Call client SendMessage method to send SMS message
                var messageId = await smsClient.SendMessageAsync(smsMessage);

                // SendMessage method should return Id of the sent Viber message
                Console.WriteLine($"Message was successfully sent. MessageId: {messageId}");
            }
            catch (SmsException ex)
            {
                // SmsException may contain specific DecisionTelecom error code of what went wrong during the operation
                // It can be used to return desired result to the caller or show desired message.
                if (ex.HasErrorCode)
                {
                    Console.WriteLine($"Error occurred while sending SMS message: {ex.ErrorCode.ToString()}");
                }
                else
                {
                    // Otherwise non-DecisionTelecom error occurred during the operation,
                    // like unsuccessful response status code was returned by API
                    Console.WriteLine($"Error occurred while sending SMS message: {ex.Message}");
                }
            }
        }

        public static async Task GetMessageStatusAsync()
        {
            try
            {
                // Create new instance of the SmsClient
                var smsClient = new SmsClient("<YOUR_LOGIN>", "<YOUR_PASSWORD>");

                // Call client method to get SMS message status
                var status = await smsClient.GetMessageStatusAsync(31885463);

                // GetMessageStatus method should return status of the sent SMS message.
                Console.WriteLine($"Message status: {status.ToString()}");
            }
            catch (SmsException ex)
            {
                // SmsException may contain specific DecisionTelecom error code of what went wrong during the operation
                // It can be used to return desired result to the caller or show desired message.
                if (ex.HasErrorCode)
                {
                    Console.WriteLine($"Error occurred while getting SMS message status: {ex.ErrorCode.ToString()}");
                }
                else
                {
                    // Otherwise non-DecisionTelecom error occurred during the operation,
                    // like unsuccessful response status code was returned by API
                    Console.WriteLine($"Error occurred while getting SMS message status: {ex.Message}");
                }
            }
        }

        public static async Task GetBalanceAsync()
        {
            try
            {
                // Create new instance of the SmsClient
                var smsClient = new SmsClient("<YOUR_LOGIN>", "<YOUR_PASSWORD>");

                // Call client method to get the user balance
                var balance = await smsClient.GetBalanceAsync();

                // GetBalance method should return SMS balance information
                Console.WriteLine(
                    $"Balance information. Balance: {balance.BalanceAmount}, Credit: {balance.CreditAmount}, Currency: {balance.Currency}");
            }
            catch (SmsException ex)
            {
                // SmsException may contain specific DecisionTelecom error code of what went wrong during the operation
                // It can be used to return desired result to the caller or show desired message.
                if (ex.HasErrorCode)
                {
                    Console.WriteLine($"Error occurred getting SMS balance: {ex.ErrorCode.ToString()}");
                }
                else
                {
                    // Otherwise non-DecisionTelecom error occurred during the operation,
                    // like unsuccessful response status code was returned by API
                    Console.WriteLine($"Error occurred getting SMS balance: {ex.Message}");
                }
            }
        }
    }
}
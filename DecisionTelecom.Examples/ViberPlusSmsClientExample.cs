using System;
using System.Threading.Tasks;
using DecisionTelecom.Exceptions;
using DecisionTelecom.Models;

namespace DecisionTelecom.Examples
{
    public static class ViberPlusSmsClientExample
    {
        public static async Task SendTransactionalMessageAsync()
        {
            try
            {
                // Create new instance of ViberPlusSmsClient
                var viberPlusSmsClient = new ViberPlusSmsClient("<YOUR_ACCESS_KEY>");

                // Create message object. This one will be transactional message with message text only.
                var message = new ViberPlusSmsMessage
                {
                    Sender = "Custom Company",
                    Receiver = "380504444444",
                    MessageType = ViberMessageType.TextOnly,
                    Text = "Test Viber Message",
                    SmsText = "SMS Message Text",
                    SourceType = ViberMessageSourceType.Transactional,
                    CallbackUrl = "https://yourdomain.com/viber-callback",
                    ValidityPeriod = 3600
                };

                // Call client SendMessage method to send Viber plus SMS message
                var messageId = await viberPlusSmsClient.SendMessageAsync(message);
                
                // SendMessage method should return Id of the sent Viber message
                Console.WriteLine($"Message was successfully sent. MessageId: {messageId}");
            }
            catch (ViberException ex)
            {
                // ViberException may contain specific DecisionTelecom error with details of what went wrong
                if (ex.Error != null)
                {
                    Console.WriteLine("Error occurred while sending Viber plus SMS message./n" +
                                      $"Error name: {ex.Error.Name}/n" +
                                      $"Error message: {ex.Error.Message}/n" +
                                      $"Error code: {ex.Error.Code}/n" +
                                      $"Error status: {ex.Error.Status}");
                }
                else
                {
                    // Otherwise non-DecisionTelecom error occurred during the operation,
                    // like unsuccessful response status code was returned by API
                    Console.WriteLine($"Error occurred while sending Viber plus SMS message: {ex.Message}");
                }
            }
        }

        public static async Task GetMessageStatusAsync()
        {
            try
            {
                // Create new instance of ViberPlusSmsClient
                var viberPlusSmsClient = new ViberPlusSmsClient("<YOUR_ACCESS_KEY>");

                // Call client method to get Viber plus SMS message status
                var receipt = await viberPlusSmsClient.GetMessageStatusAsync(429);

                // GetMessageStatus method should return status of the sent Viber message
                Console.WriteLine($"Viber message status: {receipt.ViberMessageStatus.ToString()}/n" +
                                  $"SMS message status: {receipt.SmsMessageStatus.ToString()}");
            }
            catch (ViberException ex)
            {
                // ViberException may contain specific DecisionTelecom error with details of what went wrong
                if (ex.Error != null)
                {
                    Console.WriteLine("Error occurred while getting Viber plus SMS message status./n" +
                                      $"Error name: {ex.Error.Name}/n" +
                                      $"Error message: {ex.Error.Message}/n" +
                                      $"Error code: {ex.Error.Code}/n" +
                                      $"Error status: {ex.Error.Status}");
                }
                else
                {
                    // Otherwise non-DecisionTelecom error occurred during the operation,
                    // like unsuccessful response status code was returned by API
                    Console.WriteLine($"Error occurred while getting Viber plus SMS message status: {ex.Message}");
                }
            }
        }
    }
}
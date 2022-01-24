using System;
using System.Threading.Tasks;
using DecisionTelecom.Exceptions;
using DecisionTelecom.Models;

namespace DecisionTelecom.Examples
{
    public static class ViberClientExample
    {
        public static async Task SendTransactionalMessageAsync()
        {
            try
            {
                // Create new instance of ViberClient
                var viberClient = new ViberClient("<YOUR_ACCESS_KEY>");

                // Create message object. This one will be transactional message with message text only.
                var viberMessage = new ViberMessage
                {
                    Sender = "Custom Company",
                    Receiver = "380504444444",
                    MessageType = ViberMessageType.TextOnly,
                    Text = "Test Viber Message",
                    SourceType = ViberMessageSourceType.Transactional,
                    CallbackUrl = "https://yourdomain.com/viber-callback",
                    ValidityPeriod = 3600
                };

                // Call client SendMessage method to send Viber message
                var messageId = await viberClient.SendMessageAsync(viberMessage);

                // SendMessage method should return Id of the sent Viber message
                Console.WriteLine($"Message was successfully sent. MessageId: {messageId}");
            }
            catch (ViberException ex)
            {
                // ViberException may contain specific DecisionTelecom error with details of what went wrong
                if (ex.Error != null)
                {
                    Console.WriteLine("Error occurred while sending Viber message." +
                                      $"Error name: {ex.Error.Name}/n" +
                                      $"Error message: {ex.Error.Message}/n" +
                                      $"Error code: {ex.Error.Code}/n" +
                                      $"Error status: {ex.Error.Status}");
                }
                else
                {
                    // Otherwise non-DecisionTelecom error occurred during the operation (like connection error)
                    Console.WriteLine($"Error occurred while sending Viber message: {ex.Message}");
                }
            }
        }

        public static async Task SendPromotionalMessageAsync()
        {
            try
            {
                // Create new instance of ViberClient
                var viberClient = new ViberClient("<YOUR_ACCESS_KEY>");

                // Create message object. This one will be promotional message with message text, image and button.
                var viberMessage = new ViberMessage
                {
                    Sender = "Custom Company",
                    Receiver = "380504444444",
                    MessageType = ViberMessageType.TextImageButton,
                    Text = "Test Viber Message",
                    ImageUrl = "https://yourdomain.com/images/image.jpg",
                    ButtonCaption = "Join Us",
                    ButtonAction = "https://yourdomain.com/join-us",
                    SourceType = ViberMessageSourceType.Promotional,
                    CallbackUrl = "https://yourdomain.com/viber-callback",
                    ValidityPeriod = 50
                };

                // Call client SendMessage method to send Viber message.
                var messageId = await viberClient.SendMessageAsync(viberMessage);
                
                // SendMessage method should return Id of the sent Viber message.
                Console.WriteLine($"Message was successfully sent. MessageId: {messageId}");
            }
            catch (ViberException ex)
            {
                // ViberException may contain specific DecisionTelecom error with details of what went wrong
                if (ex.Error != null)
                {
                    Console.WriteLine("Error occurred while sending Viber message." +
                                      $"Error name: {ex.Error.Name}/n" +
                                      $"Error message: {ex.Error.Message}/n" +
                                      $"Error code: {ex.Error.Code}/n" +
                                      $"Error status: {ex.Error.Status}");
                }
                else
                {
                    // Otherwise non-DecisionTelecom error occurred during the operation (like connection error)
                    Console.WriteLine($"Error occurred while sending Viber message: {ex.Message}");
                }
            }
        }

        public static async Task GetMessageStatusAsync()
        {
            try
            {
                // Create new instance of ViberClient
                var viberClient = new ViberClient("<YOUR_ACCESS_KEY>");

                // Call client method to get Viber message status receipt
                var receipt = await viberClient.GetMessageStatusAsync(429);

                // GetMessageStatus method should return status of the sent Viber message
                Console.WriteLine($"Message status: {receipt.ViberMessageStatus.ToString()}");
            }
            catch (ViberException ex)
            {
                // ViberException may contain specific DecisionTelecom error with details of what went wrong
                if (ex.Error != null)
                {
                    Console.WriteLine("Error occurred while getting Viber message status./n" +
                                      $"Error name: {ex.Error.Name}/n" +
                                      $"Error message: {ex.Error.Message}/n" +
                                      $"Error code: {ex.Error.Code}/n" +
                                      $"Error status: {ex.Error.Status}");
                }
                else
                {
                    // Otherwise non-DecisionTelecom error occurred during the operation (like connection error)
                    Console.WriteLine($"Error occurred while getting Viber message status: {ex.Message}");
                }
            }
        }
    }
}
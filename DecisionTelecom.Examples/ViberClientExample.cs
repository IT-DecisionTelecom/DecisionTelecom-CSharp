using System;
using System.Threading.Tasks;
using DecisionTelecom.Models;

namespace DecisionTelecom.Examples
{
    public static class ViberClientExample
    {
        public static async Task SendMessageAsync()
        {
            // Create new instance of ViberClient
            var viberClient = new ViberClient("<YOUR_ACCESS_KEY>");
            
            // Create message object
            var viberMessage = new ViberMessage
            {
                Sender = "Custom Company",
                Receiver = "380504444444",
                MessageType = ViberMessageType.TextOnly,
                Text = "Test Viber Message",
                ImageUrl = "https://yourdomain.com/images/image.jpg",
                ButtonCaption = "Join Us",
                ButtonAction = "https://yourdomain.com/join-us",
                SourceType = ViberMessageSourceType.Transactional,
                CallbackUrl = "https://yourdomain.com/viber-callback",
                ValidityPeriod = 50
            };

            // Call client SendMessage method to send Viber message.
            // Returned object has flag to specify whether the operation was successful or not.
            var result = await viberClient.SendMessageAsync(viberMessage);
            
            // If operation was successful, Value property contains operation result.
            // So it can be returned or displayed to the caller.
            if (result.Success)
            {
                Console.WriteLine($"Message was successfully sent. MessageId: {result.Value}");
            }
            else
            {
                // Otherwise, if operation was not successful (if error appeared during the operation execution),
                // Error property contains the corresponding error information.
                // It can be used to return desired result to the caller or show desired message.
                Console.WriteLine("Error occurred while sending Viber message." +
                                  $"Error name: {result.Error.Name}/n" +
                                  $"Error message: {result.Error.Message}/n" +
                                  $"Error code: {result.Error.Code}/n" +
                                  $"Error status: {result.Error.Status}");   
            }
        }

        public static async Task GetMessageStatusAsync()
        {
            // Create new instance of ViberClient
            var viberClient = new ViberClient("<YOUR_ACCESS_KEY>");
            
            // Call client method to get Viber message status.
            // Returned object has flag to specify whether the operation was successful or not.
            var result = await viberClient.GetMessageStatusAsync(429);
            
            // If operation was successful, Value property contains operation result.
            // So it can be returned or displayed to the caller.
            if (result.Success)
            {
                Console.WriteLine($"Message status: {result.Value.ViberMessageStatus.ToString()}");   
            }
            else
            {
                // Otherwise, if operation was not successful (if error appeared during the operation execution),
                // Error property contains the corresponding error information.
                // It can be used to return desired result to the caller or show desired message.
                Console.WriteLine("Error occurred while getting Viber message status." +
                                  $"Error name: {result.Error.Name}/n" +
                                  $"Error message: {result.Error.Message}/n" +
                                  $"Error code: {result.Error.Code}/n" +
                                  $"Error status: {result.Error.Status}");   
            }
        }
    }
}
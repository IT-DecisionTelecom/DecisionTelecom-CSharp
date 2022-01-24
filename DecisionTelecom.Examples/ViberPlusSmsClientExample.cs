using System;
using System.Threading.Tasks;
using DecisionTelecom.Models;

namespace DecisionTelecom.Examples
{
    public static class ViberPlusSmsClientExample
    {
        public static async Task SendTransactionalMessageAsync()
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

            // Call client SendMessage method to send Viber plus SMS message.
            // Returned object has flag to specify whether the operation was successful or not.
            var result = await viberPlusSmsClient.SendMessageAsync(message);
            
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
                Console.WriteLine("Error occurred while sending Viber plus SMS message." +
                                  $"Error name: {result.Error.Name}/n" +
                                  $"Error message: {result.Error.Message}/n" +
                                  $"Error code: {result.Error.Code}/n" +
                                  $"Error status: {result.Error.Status}");   
            }
        }

        public static async Task GetMessageStatusAsync()
        {
            // Create new instance of ViberPlusSmsClient
            var viberPlusSmsClient = new ViberPlusSmsClient("<YOUR_ACCESS_KEY>");
            
            // Call client method to get Viber message status.
            // Returned object has flag to specify whether the operation was successful or not.
            var result = await viberPlusSmsClient.GetMessageStatusAsync(429);
            
            // If operation was successful, Value property contains operation result.
            // So it can be returned or displayed to the caller.
            if (result.Success)
            {
                Console.WriteLine($"Viber message status: {result.Value.ViberMessageStatus.ToString()}/n" +
                                  $"SMS message status: {result.Value.SmsMessageStatus.ToString()}");
            }
            else
            {
                // Otherwise, if operation was not successful (if error appeared during the operation execution),
                // Error property contains the corresponding error information.
                // It can be used to return desired result to the caller or show desired message.
                Console.WriteLine("Error occurred while getting Viber plus SMS message status." +
                                  $"Error name: {result.Error.Name}/n" +
                                  $"Error message: {result.Error.Message}/n" +
                                  $"Error code: {result.Error.Code}/n" +
                                  $"Error status: {result.Error.Status}");   
            }
        }
    }
}
using System;
using System.Threading.Tasks;
using DecisionTelecom.Models;

namespace DecisionTelecom.Examples
{
    public static class SmsClientExample
    {
        public static async Task SendMessageAsync()
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

            // Call client SendMessage method to send SMS message.
            // Returned object has flag to specify whether the operation was successful or not.
            var result = await smsClient.SendMessageAsync(smsMessage);
            
            if (result.Success)
            {
                // If operation was successful, Value property contains operation result.
                // So it can be returned or displayed to the caller.
                Console.WriteLine($"Message was successfully sent. MessageId: {result.Value}");
            }
            else
            {
                // Otherwise, if operation was not successful (if error appeared during the operation execution),
                // Error property contains the corresponding error code.
                // It can be used to return desired result to the caller or show desired message.
                Console.WriteLine($"Error occurred while sending SMS message: {result.Error.ToString()}");
            }
        }

        public static async Task GetMessageStatusAsync()
        {
            // Create new instance of the SmsClient
            var smsClient = new SmsClient("<YOUR_LOGIN>", "<YOUR_PASSWORD>");
            
            // Call client method to get SMS message status.
            // Returned object has flag to specify whether the operation was successful or not.
            var result = await smsClient.GetMessageDeliveryStatusAsync(31885463);
            
            if (result.Success)
            {
                // If operation was successful, Value property contains operation result.
                // So it can be returned or displayed to the caller.
                Console.WriteLine($"Message status {result.Value.ToString()}");
            }
            else
            {
                // Otherwise, if operation was not successful (if error appeared during the operation execution),
                // Error property contains the corresponding error code.
                // It can be used to return desired result to the caller or show desired message.
                Console.WriteLine($"Error occurred while getting SMS message status: {result.Error.ToString()}");
            }
        }

        public static async Task GetBalanceAsync()
        {
            // Create new instance of the SmsClient
            var smsClient = new SmsClient("<YOUR_LOGIN>", "<YOUR_PASSWORD>");

            // Call client method to get the user balance.
            // Returned object has flag to specify whether the operation was successful or not.
            var result = await smsClient.GetBalanceAsync();

            if (result.Success)
            {
                // If operation was successful, Value property contains operation result.
                // So it can be returned or displayed to the caller.
                Console.WriteLine(
                    $"Your balance information. Balance: {result.Value.BalanceAmount}, Credit: {result.Value.CreditAmount}, Currency: {result.Value.Currency}");
            }
            else
            {
                // Otherwise, if operation was not successful (if error appeared during the operation execution),
                // Error property contains the corresponding error code.
                // It can be used to return desired result to the caller or show desired message.
                Console.WriteLine($"Error occurred getting SMS balance: {result.Error.ToString()}");
            }
        }
    }
}
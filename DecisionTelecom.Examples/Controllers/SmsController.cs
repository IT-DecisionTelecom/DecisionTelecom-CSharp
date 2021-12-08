using System.Threading.Tasks;
using DecisionTelecom.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DecisionTelecom.Examples.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SmsController : ControllerBase
    {
        private readonly SmsClient _smsClient;

        public SmsController(SmsClient smsClient)
        {
            _smsClient = smsClient;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessageAsync()
        {
            // Create message object
            var smsMessage = new SmsMessage
            {
                Sender = "380503515090",
                ReceiverPhone = "380503515090",
                Text = "test",
                Delivery = true,
            };

            // Call client SendMessage method to send SMS message.
            // Returned object has flag to specify whether the operation was successful or not.
            var result = await _smsClient.SendMessageAsync(smsMessage);
            if (result.Success)
            {
                return Ok(result.Value);
            }
            
            // Otherwise, if operation was not successful (if error appeared during the operation execution),
            // Error property contains the corresponding error code.
            // It can be used to return desired result to the caller or show desired message.
            return result.Error switch
            {
                SmsErrorCode.InvalidNumber => BadRequest("Invalid number"),
                SmsErrorCode.InvalidLoginOrPassword => BadRequest("Invalid user login password"),
                // ... Process other error codes as you need
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Error while performing operation")
            };
        }

        [HttpGet("{messageId:long}/status")]
        public async Task<IActionResult> GetMessageStatusAsync(long messageId)
        {
            // Call client method to get SMS message status.
            // Returned object has flag to specify whether the operation was successful or not.
            var result = await _smsClient.GetMessageDeliveryStatusAsync(messageId);
            
            // If operation was successful, Value property contains the result of the operation.
            // So it can be returned to the caller.
            if (result.Success)
            {
                return Ok(result.Value);   
            }
            
            // Otherwise, if operation was not successful (if error appeared during the operation execution),
            // Error property contains the corresponding error code.
            // It can be used to return desired result to the caller or show desired message.
            return result.Error switch
            {
                SmsErrorCode.InvalidNumber => BadRequest("Invalid number"),
                SmsErrorCode.InvalidLoginOrPassword => BadRequest("Invalid user login password"),
                // ...
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Error while performing operation")
            };
        }
        
        [HttpGet("balance")]
        public async Task<IActionResult> GetBalanceAsync()
        {
            // Call client method to get the user balance.
            // Returned object has flag to specify whether the operation was successful or not.
            var result = await _smsClient.GetBalanceAsync();
            
            // If operation was successful, Value property contains the result of the operation.
            // So it can be returned to the caller.
            if (result.Success)
            {
                return Ok(result.Value);   
            }
            
            // Otherwise, if operation was not successful (if error appeared during the operation execution),
            // Error property contains the corresponding error code.
            // It can be used to return desired result to the caller or show desired message.
            return result.Error switch
            {
                SmsErrorCode.InvalidNumber => BadRequest("Invalid number"),
                SmsErrorCode.InvalidLoginOrPassword => BadRequest("Invalid user login password"),
                // ...
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Error while performing operation")
            };
        }
    }
}
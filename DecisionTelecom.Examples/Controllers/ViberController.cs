using System.Threading.Tasks;
using DecisionTelecom.Models;
using Microsoft.AspNetCore.Mvc;

namespace DecisionTelecom.Examples.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ViberController : ControllerBase
    {
        private readonly ViberClient _viberClient;

        public ViberController(ViberClient viberClient)
        {
            _viberClient = viberClient;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessageAsync()
        {
            // Create message object
            var viberMessage = new ViberMessage
            {
                Sender = "Custom Company",
                Receiver = "380503515090",
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
            var result = await _viberClient.SendMessageAsync(viberMessage);
            if (result.Success)
            {
                return Ok(result.Value);
            }
            
            // Otherwise, if operation was not successful (if error appeared during the operation execution),
            // Error property contains the corresponding error.
            // It can be used to return desired result to the caller or show desired message.
            return StatusCode(result.Error.Status, new
            {
                result.Error.Name,
                result.Error.Message,
                result.Error.Code,
            });
        }
        
        [HttpGet("{messageId:long}/status")]
        public async Task<IActionResult> GetMessageStatusAsync(long messageId)
        {
            // Call client method to get Viber message status.
            // Returned object has flag to specify whether the operation was successful or not.
            var result = await _viberClient.GetMessageStatusAsync(messageId);
            
            // If operation was successful, Value property contains the result of the operation.
            // So it can be returned to the caller.
            if (result.Success)
            {
                return Ok(result.Value);   
            }
            
            // Otherwise, if operation was not successful (if error appeared during the operation execution),
            // Error property contains the corresponding error code.
            // It can be used to return desired result to the caller or show desired message.
            return StatusCode(result.Error.Status, new
            {
                result.Error.Name,
                result.Error.Message,
                result.Error.Code,
            });
        }
    }
}
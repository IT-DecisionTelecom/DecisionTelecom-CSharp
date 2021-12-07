using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DecisionTelecom.Models;
using DecisionTelecom.Models.Common;

namespace DecisionTelecom
{
    /// <summary>
    /// Client to work with Viber plus SMS messages
    /// <remarks>This client allows sending SMS message when transactional Viber message was not delivered</remarks> 
    /// </summary>
    public class ViberPlusSmsClient : ViberClient
    {
        /// <summary>
        /// Creates a new instance of the ViberPlusSmsClient class
        /// </summary>
        /// <param name="httpClient">Http client to send http requests</param>
        /// <param name="accessKey">User access key</param>
        public ViberPlusSmsClient(HttpClient httpClient, string accessKey) : base(httpClient, accessKey)
        {
        }

        /// <summary>
        /// Creates a new instance of the ViberPlusSmsClient class
        /// </summary>
        /// <param name="accessKey">User access key</param>
        public ViberPlusSmsClient(string accessKey) : base(accessKey)
        {
        }

        /// <summary>
        /// Sends Viber message and also SMS message when transactional Viber message was not delivered
        /// </summary>
        /// <param name="message">Viber plus SMS message to send</param>
        /// <returns>Id of the sent Viber message in case of success or error information otherwise</returns>
        /// <exception cref="InvalidOperationException">Not possible to parse response received from the server</exception>
        public async Task<Result<long, ViberError>> SendMessageAsync(ViberPlusSmsMessage message)
        {
            return await base.SendMessageAsync(message);
        }
        
        /// <summary>
        /// Returns Viber plus SMS message status
        /// </summary>
        /// <param name="messageId">Id of the Viber message (sent in the last 5 days)</param>
        /// <returns>Viber message status in case of success or error information otherwise</returns>
        /// <exception cref="InvalidOperationException">Not possible to parse response received from the server</exception>
        public new async Task<Result<ViberPlusSmsMessageResult, ViberError>> GetMessageStatusAsync(long messageId)
        {
            var request = new Dictionary<string, long> { { MessageIdPropertyName, messageId } };
            ViberPlusSmsMessageResult OkResponseFunc(string json) =>
                JsonSerializer.Deserialize<ViberPlusSmsMessageResult>(json);

            return await ProcessRequestAsync($"{BaseUrl}/receive-viber", request, OkResponseFunc);
        }
    }
}
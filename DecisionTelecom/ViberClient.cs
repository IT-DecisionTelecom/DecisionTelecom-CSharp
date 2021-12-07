using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DecisionTelecom.Models;
using DecisionTelecom.Models.Common;

namespace DecisionTelecom
{
    /// <summary>
    /// Client to work with Viber messages 
    /// </summary>
    public class ViberClient
    {
        protected const string BaseUrl = "https://web.it-decision.com/v1/api";

        protected const string MessageIdPropertyName = "message_id";

        private readonly HttpClient httpClient;

        /// <summary>
        /// User access key 
        /// </summary>
        public string AccessKey { get; }
        
        /// <summary>
        /// Creates a new instance of the ViberClient class
        /// </summary>
        /// <param name="httpClient">Http client to send http requests</param>
        /// <param name="accessKey">User access key</param>
        public ViberClient(HttpClient httpClient, string accessKey)
        {
            this.httpClient = httpClient;
            AccessKey = accessKey;
        }
        
        /// <summary>
        /// Creates a new instance of the ViberClient class
        /// </summary>
        /// <param name="accessKey">User access key</param>
        public ViberClient(string accessKey) : this(new HttpClient(), accessKey)
        {
        }

        /// <summary>
        /// Sends Viber message
        /// </summary>
        /// <param name="message">Viber message to send</param>
        /// <returns>Id of the sent Viber message in case of success or error information otherwise</returns>
        /// <exception cref="InvalidOperationException">Not possible to parse response received from the server</exception>
        public async Task<Result<long, ViberError>> SendMessageAsync(ViberMessage message)
        {
            long OkResponseFunc(string json) =>
                JsonDocument.Parse(json).RootElement.GetProperty(MessageIdPropertyName).GetInt64();

            return await ProcessRequestAsync($"{BaseUrl}/receive-viber", message, OkResponseFunc);
        }

        /// <summary>
        /// Returns Viber message status
        /// </summary>
        /// <param name="messageId">Id of the Viber message (sent in the last 5 days)</param>
        /// <returns>Viber message status in case of success or error information otherwise</returns>
        /// <exception cref="InvalidOperationException">Not possible to parse response received from the server</exception>
        public async Task<Result<ViberMessageResult, ViberError>> GetMessageStatusAsync(long messageId)
        {
            var request = new Dictionary<string, long> { { MessageIdPropertyName, messageId } };
            ViberMessageResult OkResponseFunc(string json) => JsonSerializer.Deserialize<ViberMessageResult>(json);

            return await ProcessRequestAsync($"{BaseUrl}/receive-viber", request, OkResponseFunc);
        }

        protected async Task<Result<T, ViberError>> ProcessRequestAsync<T>(string url, object request, Func<string, T> okResponseFunc)
        {
            var response = await MakeRequestAsync(url, request);
            var json = await response.Content.ReadAsStringAsync();

            try
            {
                if (!response.IsSuccessStatusCode || !json.Contains(MessageIdPropertyName))
                {
                    return JsonSerializer.Deserialize<ViberError>(json);
                }

                return okResponseFunc(json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Unable to process service response. Please contact support. Response content: {json}", ex);
            }
        }

        private async Task<HttpResponseMessage> MakeRequestAsync(string url, object requestContent)
        {
            var accessKeyBytes = Encoding.UTF8.GetBytes(AccessKey);
            var accessKeyBase64 = Convert.ToBase64String(accessKeyBytes);
            
            using var request = new HttpRequestMessage(HttpMethod.Post, url);

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", accessKeyBase64);
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
            request.Content = new StringContent(JsonSerializer.Serialize(requestContent));

            return await httpClient.SendAsync(request);
        }
    }
}
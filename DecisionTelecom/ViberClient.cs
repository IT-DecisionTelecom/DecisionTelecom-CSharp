using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DecisionTelecom.Exceptions;
using DecisionTelecom.Models;

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
        /// <exception cref="ViberException">Specific Viber error occurred</exception>
        public async Task<long> SendMessageAsync(ViberMessage message)
        {
            long OkResponseFunc(string json) =>
                JsonDocument.Parse(json).RootElement.GetProperty(MessageIdPropertyName).GetInt64();

            return await ProcessRequestAsync($"{BaseUrl}/send-viber", message, OkResponseFunc);
        }

        /// <summary>
        /// Returns Viber message status
        /// </summary>
        /// <param name="messageId">Id of the Viber message (sent in the last 5 days)</param>
        /// <returns>Viber message status in case of success or error information otherwise</returns>
        /// <exception cref="ViberException">Specific Viber error occurred</exception>
        public async Task<ViberMessageReceipt> GetMessageStatusAsync(long messageId)
        {
            var request = new Dictionary<string, long> { { MessageIdPropertyName, messageId } };
            ViberMessageReceipt OkResponseFunc(string json) => JsonSerializer.Deserialize<ViberMessageReceipt>(json);

            return await ProcessRequestAsync($"{BaseUrl}/receive-viber", request, OkResponseFunc);
        }

        protected async Task<T> ProcessRequestAsync<T>(string url, object request, Func<string, T> okResponseFunc)
        {
            var response = await MakeRequestAsync(url, request);
            var json = await response.Content.ReadAsStringAsync();

            // Process unsuccessful status codes
            if (!response.IsSuccessStatusCode)
            {
                throw new ViberException(
                    $"An error occurred while processing request. Response code: {(int)response.StatusCode} ({response.StatusCode.ToString()})");
            }

            // If response contains "name", "message", "code" and "status" words, treat it as an ViberError   
            if (json.Contains("name") && json.Contains("message") && json.Contains("code") && json.Contains("status"))
            {
                throw ViberException.FromResponse(json);
            }

            try
            {
                return okResponseFunc(json);
            }
            catch (Exception ex)
            {
                throw new ViberException(
                    $"Unable to process service response. Please contact support. Response content: {json}", ex);
            }
        }

        private async Task<HttpResponseMessage> MakeRequestAsync(string url, object requestContent)
        {
            var accessKeyBytes = Encoding.UTF8.GetBytes(AccessKey);
            var accessKeyBase64 = Convert.ToBase64String(accessKeyBytes);

            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", accessKeyBase64);
                request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                request.Content = new StringContent(JsonSerializer.Serialize(requestContent));
                request.Content.Headers.ContentType = MediaTypeWithQualityHeaderValue.Parse("application/json");

                return await httpClient.SendAsync(request);
            }
        }
    }
}
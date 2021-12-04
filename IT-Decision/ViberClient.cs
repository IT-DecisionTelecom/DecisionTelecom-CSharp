using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ITDecision.Models;
using ITDecision.Models.Common;

namespace ITDecision
{
    public class ViberClient
    {
        private const string BaseUrl = "https://web.it-decision.com/v1/api";

        private const string MessageIdFieldName = "message_id";
        
        private readonly HttpClient httpClient;

        public string AccessKey { get; }
        
        public ViberClient(HttpClient httpClient, string accessKey)
        {
            this.httpClient = httpClient;
            AccessKey = accessKey;
        }
        
        public ViberClient(string accessKey) : this(new HttpClient(), accessKey)
        {
        }

        public async Task<Result<int, ViberError>> SendMessageAsync(ViberMessage request)
        {
            var response = await MakeRequestAsync($"{BaseUrl}/send-viber", request);
            var json = await response.Content.ReadAsStringAsync();

            try
            {
                if (!response.IsSuccessStatusCode || !json.Contains(MessageIdFieldName))
                {
                    return JsonSerializer.Deserialize<ViberError>(json);
                }

                return JsonDocument.Parse(json).RootElement.GetProperty("message_id").GetInt32();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Unable to process service response. Please contact support. Response content: {json}", ex);
            }
        }

        public async Task<Result<ViberMessageStatus, ViberError>> GetMessageStatusAsync(int messageId)
        {
            var request = new Dictionary<string, int>
            {
                {"message_id", messageId },
            };
            var response = await MakeRequestAsync($"{BaseUrl}/receive-viber", request);
            var json = await response.Content.ReadAsStringAsync();

            try
            {
                if (!response.IsSuccessStatusCode || !json.Contains(MessageIdFieldName))
                {
                    return JsonSerializer.Deserialize<ViberError>(json);
                }

                return (ViberMessageStatus)JsonDocument.Parse(json).RootElement.GetProperty("status").GetInt32();
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
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DecisionTelecom.Exceptions;
using DecisionTelecom.Models;

namespace DecisionTelecom
{
    /// <summary>
    /// Client to work with SMS messages
    /// </summary>
    public class SmsClient
    {
        private const string BaseUrl = "https://web.it-decision.com/ru/js";

        private const string ErrorPropertyName = "error";

        private const string MessageIdPropertyName = "msgid";

        private const string StatusPropertyName = "status";

        private readonly HttpClient httpClient;

        public string Login { get; }

        public string Password { get; }

        /// <summary>
        /// Creates a new instance of the SmsClient class
        /// </summary>
        /// <param name="httpClient">Http client to send http requests</param>
        /// <param name="login">Login to access the system</param>
        /// <param name="password">Password to access the system</param>
        public SmsClient(HttpClient httpClient, string login, string password)
        {
            this.httpClient = httpClient;
            Login = login;
            Password = password;
        }

        /// <summary>
        /// Creates new instance of the SmsClient class  
        /// </summary>
        /// <param name="login">Login in the system</param>
        /// <param name="password">Password in the system</param>
        public SmsClient(string login, string password) : this(new HttpClient(), login, password)
        {
        }

        /// <summary>
        /// Sends SMS message
        /// </summary>
        /// <param name="message">SMS message to send</param>
        /// <returns>The Id of the submitted SMS message in case of success or error code otherwise</returns>
        /// <exception cref="SmsException">Specific Sms error occurred</exception>
        public async Task<long> SendMessageAsync(SmsMessage message)
        {
            var requestUri =
                $"{BaseUrl}/send?login={Login}&password={Password}&phone={message.ReceiverPhone}&sender={message.Sender}&text={message.Text}&dlr={Convert.ToInt16(message.Delivery)}";

            return await MakeHttpRequestAsync(requestUri, OkResultFunc);

            long OkResultFunc(string responseContent) =>
                long.Parse(GetValueFromListResponseContent(responseContent, MessageIdPropertyName));
        }

        /// <summary>
        /// Returns SMS message delivery status
        /// </summary>
        /// <param name="messageId">The Id of the submitted SMS</param>
        /// <returns>SMS message delivery status in case of success or error code otherwise</returns>
        /// <exception cref="SmsException">Specific Sms error occurred</exception>
        public async Task<SmsMessageStatus> GetMessageStatusAsync(long messageId)
        {
            var requestUri = $"{BaseUrl}/state?login={Login}&password={Password}&msgid={messageId}";
            return await MakeHttpRequestAsync(requestUri, OkResultFunc);

            SmsMessageStatus OkResultFunc(string responseContent)
            {
                var responseValue = GetValueFromListResponseContent(responseContent, StatusPropertyName);
                return string.IsNullOrEmpty(responseValue)
                    ? SmsMessageStatus.Unknown
                    : (SmsMessageStatus)int.Parse(responseValue);
            }
        }

        /// <summary>
        /// Returns balance information
        /// </summary>
        /// <returns>User balance information</returns>
        /// <exception cref="SmsException">Specific Sms error occurred</exception>
        public async Task<SmsBalance> GetBalanceAsync()
        {
            var requestUri = $"{BaseUrl}/balance?login={Login}&password={Password}";
            return await MakeHttpRequestAsync(requestUri, OkResultFunc);

            SmsBalance OkResultFunc(string responseContent)
            {
                // Replace symbols to be able to parse response string as json
                // Regexp removes quotation marks ("") around the numbers, so they could be parsed as float
                var regex = new Regex("\"([-+]?[0-9]*\\.?[0-9]+)\"");
                var replacedContent = responseContent.Replace("[", "{").Replace("]", "}");
                replacedContent = regex.Replace(replacedContent, "$1");

                return JsonSerializer.Deserialize<SmsBalance>(replacedContent);
            }
        }

        /// <summary>
        /// Makes HTTP request and processes response 
        /// </summary>
        /// <param name="requestUri">Request URL</param>
        /// <param name="okResponseFunc">Function to create Result object in case when http request was successful</param>
        /// <typeparam name="T">Result value type</typeparam>
        /// <returns>Processed HTTP response</returns>
        /// <exception cref="SmsException">Specific Sms error occurred</exception>
        private async Task<T> MakeHttpRequestAsync<T>(
            string requestUri,
            Func<string, T> okResponseFunc)
        {
            var response = await httpClient.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
            {
                throw new SmsException(
                    $"An error occurred while processing request. Response code: {(int)response.StatusCode} ({response.StatusCode.ToString()})");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            try
            {
                if (!responseContent.StartsWith($"[\"{ErrorPropertyName}"))
                {
                    return okResponseFunc(responseContent);
                }

                var errorCode = int.Parse(GetValueFromListResponseContent(responseContent, ErrorPropertyName));
                throw new SmsException("An error occurred while processing request", (SmsErrorCode)errorCode);
            }
            catch (SmsException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SmsException(
                    $"Unable to process service response. Please contact support. Response content: {responseContent}",
                    ex);
            }
            
            /*var responseContent = await response.Content.ReadAsStringAsync();
            if (responseContent.StartsWith($"[\"{ErrorPropertyName}"))
            {
                var errorCode = int.Parse(GetValueFromListResponseContent(responseContent, ErrorPropertyName));
                throw new SmsException("An error occurred while processing request", (SmsErrorCode)errorCode);
            }

            try
            {
                return okResponseFunc(responseContent);
            }
            catch (Exception ex)
            {
                throw new SmsException(
                    $"Unable to process service response. Please contact support. Response content: {responseContent}",
                    ex);
            }*/
        }

        private static string GetValueFromListResponseContent(string responseContent, string keyPropertyName)
        {
            var responseList = responseContent
                .Trim('[', ']')
                .Split(new[] { "," }, StringSplitOptions.None)
                .Select(x => x.Trim('\"'))
                .ToList();
            
            if (!responseList[0].Equals(keyPropertyName))
            {
                throw new ArgumentException($"Unknown key '{responseList[0]}' in the response.");
            }

            return responseList[1];
        }
    }
}
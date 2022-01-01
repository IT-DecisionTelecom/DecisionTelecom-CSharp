using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DecisionTelecom.Models;
using DecisionTelecom.Models.Common;

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
        /// <exception cref="InvalidOperationException">Not possible to parse response from the server</exception>
        public async Task<Result<long, SmsErrorCode>> SendMessageAsync(SmsMessage message)
        {
            var requestUri =
                $"{BaseUrl}/send?login={Login}&password={Password}&phone={message.ReceiverPhone}&sender={message.Sender}&text={message.Text}&dlr={Convert.ToInt16(message.Delivery)}";

            var response = await httpClient.GetAsync(requestUri);
            return await GetResultFromHttpResponseMessage(response, OkResultFunc);

            long OkResultFunc(string responseContent) =>
                long.Parse(GetValueFromListResponseContent(responseContent, MessageIdPropertyName));
        }

        /// <summary>
        /// Returns SMS message delivery status
        /// </summary>
        /// <param name="messageId">The Id of the submitted SMS</param>
        /// <returns>SMS message delivery status in case of success or error code otherwise</returns>
        /// <exception cref="InvalidOperationException">Not possible to parse response from the server</exception>
        public async Task<Result<SmsMessageStatus, SmsErrorCode>> GetMessageDeliveryStatusAsync(long messageId)
        {
            var requestUri = $"{BaseUrl}/state?login={Login}&password={Password}&msgid={messageId}";
            var response = await httpClient.GetAsync(requestUri);

            return await GetResultFromHttpResponseMessage(response, OkResultFunc);

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
        /// <exception cref="InvalidOperationException">Not possible to parse response from the server</exception>
        public async Task<Result<SmsBalance, SmsErrorCode>> GetBalanceAsync()
        {
            var requestUri = $"{BaseUrl}/balance?login={Login}&password={Password}";
            var response = await httpClient.GetAsync(requestUri);

            return await GetResultFromHttpResponseMessage(response, OkResultFunc);

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
        /// Processes HttpResponseMessage and returns Result object 
        /// </summary>
        /// <param name="responseMessage">Http response message</param>
        /// <param name="okResultFunc">Function to create Result object in case when http request was successful</param>
        /// <typeparam name="T">Result value type</typeparam>
        /// <returns>Result object with the data from the http request</returns>
        /// <exception cref="InvalidOperationException">Not possible to parse response from the server</exception>
        private static async Task<Result<T, SmsErrorCode>> GetResultFromHttpResponseMessage<T>(
            HttpResponseMessage responseMessage,
            Func<string, T> okResultFunc)
        {
            var responseContent = await responseMessage.Content.ReadAsStringAsync();
            try
            {
                // Return error if it was sent in response. Otherwise, process response content to create result 
                return responseContent.Contains(ErrorPropertyName)
                    ? Result<T, SmsErrorCode>.Fail<T>((SmsErrorCode)long.Parse(GetValueFromListResponseContent(responseContent, ErrorPropertyName)))
                    : okResultFunc(responseContent);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Unable to process service response. Please contact support. Response content: {responseContent}",
                    ex);
            }
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
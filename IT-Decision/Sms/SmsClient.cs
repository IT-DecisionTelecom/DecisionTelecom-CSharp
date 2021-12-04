using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ITDecision.Sms.Models;

namespace ITDecision.Sms
{
    /// <summary>
    /// SMS Client
    /// </summary>
    public class SmsClient// : MessageClient
    {
        private const string BaseUrl = "https://web.it-decision.com/ru/js";

        private const string ErrorText = "error";

        private readonly HttpClient httpClient;

        public string Login { get; }

        public string Password { get; }
        

        /// <summary>
        /// Creates new instance of the SmsClient class
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
        /// <param name="message">Sms message to send</param>
        /// <returns>The Id of the submitted SMS</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<Result<int, ErrorCode>> SendMessageAsync(SmsMessage message)
        {
            var requestUri =
                $"{BaseUrl}/send?login={Login}&password={Password}&phone={message.ReceiverPhone}&sender={message.Sender}&text={message.Text}&dlr={Convert.ToInt16(message.Delivery)}";
            
            var response = await httpClient.GetAsync(requestUri);
            return await GetResultFromHttpResponseMessage(response, OkResultFunc);

            Result<int, ErrorCode> OkResultFunc(string responseContent)
            {
                var responseDict = GetDictionaryFromResponseContent(responseContent);
                return int.Parse(responseDict["msgid"]);
            }
        }

        /// <summary>
        /// Returns SMS delivery status
        /// </summary>
        /// <param name="messageId">The Id of the submitted SMS</param>
        /// <returns>SMS delivery status code</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<Result<ReceiptDeliveryStatus, ErrorCode>> GetMessageDeliveryStatusAsync(int messageId)
        {
            var requestUri = $"{BaseUrl}/state?login={Login}&password={Password}&msgid={messageId}";
            var response = await httpClient.GetAsync(requestUri);
            
            return await GetResultFromHttpResponseMessage(response, OkResultFunc);

            Result<ReceiptDeliveryStatus, ErrorCode> OkResultFunc(string responseContent)
            { 
                var responseDict = GetDictionaryFromResponseContent(responseContent);
                return string.IsNullOrEmpty(responseDict["status"])
                    ? ReceiptDeliveryStatus.Unknown
                    : (ReceiptDeliveryStatus)int.Parse(responseDict["status"]);
            }
        }

        /// <summary>
        /// Returns balance information
        /// </summary>
        /// <returns>User balance information</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<Result<Balance, ErrorCode>> GetBalanceAsync()
        {
            var requestUri = $"{BaseUrl}/balance?login={Login}&password={Password}";
            var response = await httpClient.GetAsync(requestUri);

            return await GetResultFromHttpResponseMessage(response, OkResultFunc);

            Result<Balance, ErrorCode> OkResultFunc(string responseContent)
            {
                var responseDict = GetDictionaryFromResponseContent(responseContent, false);
                return new Balance
                {
                    BalanceAmount = double.Parse(responseDict["balance"]),
                    CreditAmount = double.Parse(responseDict["credit"]),
                    Currency = responseDict["currency"],
                };
            }
        }

        /// <summary>
        /// Processes HttpResponseMessage and returns Result object 
        /// </summary>
        /// <param name="responseMessage">Http response message</param>
        /// <param name="okResultFunc">Function to create Result object in case when http request was successful</param>
        /// <typeparam name="T">Result value type</typeparam>
        /// <returns>Result object with the data from the http request</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static async Task<Result<T, ErrorCode>> GetResultFromHttpResponseMessage<T>(HttpResponseMessage responseMessage,
            Func<string, Result<T, ErrorCode>> okResultFunc)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                return ErrorCode.ServerError;
            }

            var responseContent = await responseMessage.Content.ReadAsStringAsync();
            try
            {
                // Return error if it was sent in response. Otherwise, process response content to create result 
                return responseContent.Contains(ErrorText, StringComparison.OrdinalIgnoreCase)
                    ? GetErrorResultFromResponseContent<T>(responseContent)
                    : okResultFunc(responseContent);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Unable to process service response. Please contact support. Response content: {responseContent}", ex);
            }
        }

        private static Dictionary<string, string> GetDictionaryFromResponseContent(string responseContent,
            bool replaceComma = true)
        {
            // Replace symbols to be able to parse response string as dictionary 
            var replacedContent = responseContent
                .Replace("[", "{")
                .Replace("]", "}");

            if (replaceComma)
            {
                replacedContent = replacedContent.Replace(",", ":");
            }
            
            return JsonSerializer.Deserialize<Dictionary<string, string>>(replacedContent);
        }
        
        private static Result<T, ErrorCode> GetErrorResultFromResponseContent<T>(string responseContent)
        {
            var responseDict = GetDictionaryFromResponseContent(responseContent);
            return (ErrorCode)int.Parse(responseDict[ErrorText]);
        }
    }
}
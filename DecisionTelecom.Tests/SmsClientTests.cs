using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DecisionTelecom.Exceptions;
using DecisionTelecom.Models;
using DecisionTelecom.Tests.Extensions;
using Moq;
using Xunit;

namespace DecisionTelecom.Tests
{
    public class SmsClientTests
    {
        private readonly Mock<HttpMessageHandler> handlerMock;

        private readonly SmsClient smsClient;

        public SmsClientTests()
        {
            handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);

            smsClient = new SmsClient(httpClient, string.Empty, string.Empty);
        }
        
        [Fact]
        public async Task TestSendMessageReturnsMessageIdAsync()
        {
            const int expectedMessageId = 31885463;
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent($"[\"msgid\",\"{expectedMessageId}\"]"),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var messageId = await smsClient.SendMessageAsync(new SmsMessage());

            Assert.Equal(expectedMessageId, messageId);
        }

        [Fact]
        public async Task TestSendMessageReturnsErrorCodeAsync()
        {
            const SmsErrorCode expectedErrorCode = SmsErrorCode.InvalidLoginOrPassword;
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent($"[\"error\",{(int)expectedErrorCode}]"),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var smsException = await Assert.ThrowsAsync<SmsException>(() => smsClient.SendMessageAsync(new SmsMessage()));
            
            Assert.NotNull(smsException);
            Assert.Equal(expectedErrorCode, smsException.ErrorCode);
        }
        
        [Fact]
        public async Task TestSendMessageReturnsUnsuccessfulResponseCodeAsync()
        {
            var expectedMessage = "An error occurred while processing request. Response code: 401 (Unauthorized)";
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = new StringContent("Some general error message"),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var smsException = await Assert.ThrowsAsync<SmsException>(() => smsClient.SendMessageAsync(new SmsMessage()));
            
            Assert.NotNull(smsException);
            Assert.Equal(expectedMessage, smsException.Message);
        }
        
        [Theory]
        [InlineData("[\"error\",\"InvalidNumber\"]")]
        [InlineData("[\"error\",\"\"]")]
        [InlineData("[\"err\",44]")]
        [InlineData("[\"msg\",\"31885463\"]")]
        [InlineData("[\"msgid\"]")]
        [InlineData("[\"error\"]")]
        [InlineData("[\"msgid\",\"\"]")]
        [InlineData("[]")]
        public async Task TestSendMessageReturnsUnprocessableResponseAsync(string responseContent)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var smsException = await Assert.ThrowsAsync<SmsException>(() => smsClient.SendMessageAsync(new SmsMessage()));
            
            Assert.NotNull(smsException);
            Assert.NotNull(smsException.Message);
        }
        
        [Fact]
        public async Task TestGetMessageStatusReturnsStatusCodeAsync()
        {
            const SmsMessageStatus expectedDeliveryStatus = SmsMessageStatus.Delivered;
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent($"[\"status\",{(int)expectedDeliveryStatus}]"),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var status = await smsClient.GetMessageStatusAsync(1234);

            Assert.Equal(expectedDeliveryStatus, status);
        }
        
        [Fact]
        public async Task TestGetMessageDeliveryStatusReturnsStatusWithoutCodeAsync()
        {
            const SmsMessageStatus expectedDeliveryStatus = SmsMessageStatus.Unknown;
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[\"status\",\"\"]"),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var status = await smsClient.GetMessageStatusAsync(1234);

            Assert.Equal(expectedDeliveryStatus, status);
        }

        [Fact]
        public async Task TestGetMessageDeliveryStatusReturnsErrorCodeAsync()
        {
            const SmsErrorCode expectedErrorCode = SmsErrorCode.InvalidLoginOrPassword;
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent($"[\"error\",{(int)expectedErrorCode}]"),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var smsException = await Assert.ThrowsAsync<SmsException>(() => smsClient.GetMessageStatusAsync(1234));
            
            Assert.NotNull(smsException);
            Assert.Equal(expectedErrorCode, smsException.ErrorCode);
        }
        
        [Fact]
        public async Task TestGetMessageStatusReturnsUnsuccessfulResponseCodeAsync()
        {
            var expectedMessage = "An error occurred while processing request. Response code: 401 (Unauthorized)";
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = new StringContent("Some general error message"),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var smsException = await Assert.ThrowsAsync<SmsException>(() => smsClient.GetMessageStatusAsync(1234));
            
            Assert.NotNull(smsException);
            Assert.Equal(expectedMessage, smsException.Message);
        }
        
        [Theory]
        [InlineData("[\"error\",\"InvalidNumber\"]")]
        [InlineData("[\"error\",\"\"]")]
        [InlineData("[\"err\",\"44\"]")]
        [InlineData("[\"stat\",\"4\"]")]
        [InlineData("[\"status\"]")]
        [InlineData("[\"error\"]")]
        [InlineData("[]")]
        public async Task TestGetMessageDeliveryStatusReturnsUnprocessableResponseAsync(string responseContent)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var smsException = await Assert.ThrowsAsync<SmsException>(() => smsClient.GetMessageStatusAsync(1234));
            
            Assert.NotNull(smsException);
            Assert.NotNull(smsException.Message);
        }
        
        [Fact]
        public async Task TestGetBalanceReturnsDataAsync()
        {
            const double expectedBalance = -791.8391870;
            const double expectedCredit = 1000;
            const string expectedCurrency = "EUR";

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
                    $"[\"balance\":\"{expectedBalance}\",\"credit\":\"{expectedCredit}\",\"currency\":\"{expectedCurrency}\"]"),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var balance = await smsClient.GetBalanceAsync();
            
            Assert.NotNull(balance);
            Assert.Equal(expectedBalance, balance.BalanceAmount);
            Assert.Equal(expectedCredit, balance.CreditAmount);
            Assert.Equal(expectedCurrency, balance.Currency);
        }

        [Fact]
        public async Task TestGetBalanceReturnsErrorCodeAsync()
        {
            const SmsErrorCode expectedErrorCode = SmsErrorCode.InvalidLoginOrPassword;
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent($"[\"error\",{(int)expectedErrorCode}]"),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var smsException = await Assert.ThrowsAsync<SmsException>(() => smsClient.GetBalanceAsync());
            
            Assert.NotNull(smsException);
            Assert.Equal(expectedErrorCode, smsException.ErrorCode);
        }
        
        [Fact]
        public async Task TestGetBalanceReturnsUnsuccessfulResponseCodeAsync()
        {
            var expectedMessage = "An error occurred while processing request. Response code: 401 (Unauthorized)";
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = new StringContent("Some response message text"),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var smsException = await Assert.ThrowsAsync<SmsException>(() => smsClient.GetBalanceAsync());
            
            Assert.NotNull(smsException);
            Assert.Equal(expectedMessage, smsException.Message);
        }
        
        [Theory]
        [InlineData("[\"error\",\"InvalidNumber\"]")]
        [InlineData("[\"error\",\"\"]")]
        [InlineData("[\"err\",44]")]
        [InlineData("[\"bal\",\"4\"]")]        
        [InlineData("[\"error\"]")]        
        public async Task TestGetBalanceReturnsUnprocessableResponseAsync(string responseContent)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var smsException = await Assert.ThrowsAsync<SmsException>(() => smsClient.GetBalanceAsync());
            
            Assert.NotNull(smsException);
            Assert.NotNull(smsException.Message);
        }

        [Theory]
        [InlineData("[\"balance\":\"4\"]")]
        [InlineData("[\"balance\":\"4\",\"cred\":\"3\"]")]
        public async Task TestGetBalanceReturnsOnlyBalanceAsync(string responseContent)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var balance = await smsClient.GetBalanceAsync();
            
            Assert.NotNull(balance);
            Assert.NotEqual(0, balance.BalanceAmount);
            Assert.Equal(0, balance.CreditAmount);
            Assert.Null(balance.Currency);            
        }

        [Theory]
        [InlineData("[\"credit\":\"4\"]")]        
        [InlineData("[\"balan\":\"4\",\"credit\":\"3\"]")]
        public async Task TestGetBalanceReturnsOnlyCreditAsync(string responseContent)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var balance = await smsClient.GetBalanceAsync();

            Assert.NotNull(balance);
            Assert.Equal(0, balance.BalanceAmount);
            Assert.NotEqual(0, balance.CreditAmount);
            Assert.Null(balance.Currency);            
        }

        [Theory]
        [InlineData("[]")]
        public async Task TestGetBalanceReturnsEmptyResultAsync(string responseContent)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var balance = await smsClient.GetBalanceAsync();

            Assert.NotNull(balance);
            Assert.Equal(0, balance.BalanceAmount);
            Assert.Equal(0, balance.CreditAmount);
            Assert.Null(balance.Currency);            
        }
    }
}
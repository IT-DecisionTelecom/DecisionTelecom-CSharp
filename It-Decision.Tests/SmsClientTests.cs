using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ITDecision.Sms;
using ITDecision.Sms.Models;
using Moq;
using Moq.Protected;
using Xunit;

namespace ITDecision.Tests
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
            
            SetupHttpHandlerResponse(response);

            var result = await smsClient.SendMessageAsync("", "", "", true);
            
            Assert.True(result.Success);
            Assert.Equal(expectedMessageId, result.Value);
        }

        [Fact]
        public async Task TestSendMessageReturnsNotSuccessStatusCodeAsync()
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
            };
            
            SetupHttpHandlerResponse(response);
            
            var result = await smsClient.SendMessageAsync("", "", "", true);
            
            Assert.True(result.Failure);
            Assert.Equal(ErrorCode.ServerError, result.Error);
        }
        
        [Fact]
        public async Task TestSendMessageReturnsErrorAsync()
        {
            const ErrorCode expectedErrorCode = ErrorCode.InvalidLoginOrPassword;
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent($"[\"error\",\"{(int)expectedErrorCode}\"]"),
            };
            
            SetupHttpHandlerResponse(response);

            var result = await smsClient.SendMessageAsync("", "", "", true);
            
            Assert.True(result.Failure);
            Assert.Equal(expectedErrorCode, result.Error);
        }
        
        [Theory]
        [InlineData("[\"error\",\"InvalidNumber\"]")]
        [InlineData("[\"error\",\"\"]")]
        [InlineData("[\"err\",\"44\"]")]
        [InlineData("[\"msg\",\"31885463\"]")]
        [InlineData("[\"msgid\",\"\"]")]
        [InlineData("[\"msgid\"]")]
        [InlineData("[\"error\"]")]
        [InlineData("[]")]
        public async Task TestSendMessageReturnsUnprocessableResponseAsync(string responseContent)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent),
            };
            
            SetupHttpHandlerResponse(response);

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await smsClient.SendMessageAsync("", "", "", true));
        }
        
        [Fact]
        public async Task TestGetMessageDeliveryStatusReturnsStatusCodeAsync()
        {
            const ReceiptDeliveryStatus expectedDeliveryStatus = ReceiptDeliveryStatus.Delivered;
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent($"[\"status\",\"{(int)expectedDeliveryStatus}\"]"),
            };
            
            SetupHttpHandlerResponse(response);

            var result = await smsClient.GetMessageDeliveryStatusAsync(1234);
            
            Assert.True(result.Success);
            Assert.Equal(expectedDeliveryStatus, result.Value);
        }
        
        [Fact]
        public async Task TestGetMessageDeliveryStatusReturnsStatusWithoutCodeAsync()
        {
            const ReceiptDeliveryStatus expectedDeliveryStatus = ReceiptDeliveryStatus.Unknown;
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[\"status\",\"\"]"),
            };
            
            SetupHttpHandlerResponse(response);

            var result = await smsClient.GetMessageDeliveryStatusAsync(1234);
            
            Assert.True(result.Success);
            Assert.Equal(expectedDeliveryStatus, result.Value);
        }
        
        [Fact]
        public async Task TestGetMessageDeliveryStatusReturnsNotSuccessStatusCodeAsync()
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
            };
            
            SetupHttpHandlerResponse(response);

            var result = await smsClient.GetMessageDeliveryStatusAsync(1234);
            
            Assert.True(result.Failure);
            Assert.Equal(ErrorCode.ServerError, result.Error);
        }
        
        [Fact]
        public async Task TestGetMessageDeliveryStatusReturnsErrorAsync()
        {
            const ErrorCode expectedErrorCode = ErrorCode.InvalidLoginOrPassword;
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent($"[\"error\",\"{(int)expectedErrorCode}\"]"),
            };
            
            SetupHttpHandlerResponse(response);

            var result = await smsClient.GetMessageDeliveryStatusAsync(1234);
            
            Assert.True(result.Failure);
            Assert.Equal(expectedErrorCode, result.Error);
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
            
            SetupHttpHandlerResponse(response);

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await smsClient.GetMessageDeliveryStatusAsync(1234));
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
            
            SetupHttpHandlerResponse(response);

            var result = await smsClient.GetBalanceAsync();
            
            Assert.True(result.Success);
            Assert.Equal(expectedBalance, result.Value.BalanceAmount);
            Assert.Equal(expectedCredit, result.Value.CreditAmount);
            Assert.Equal(expectedCurrency, result.Value.Currency);
        }
        
        [Fact]
        public async Task TestGetBalanceReturnsNotSuccessStatusCodeAsync()
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
            };
            
            SetupHttpHandlerResponse(response);

            var result = await smsClient.GetBalanceAsync();
            
            Assert.True(result.Failure);
            Assert.Equal(ErrorCode.ServerError, result.Error);
        }
        
        [Fact]
        public async Task TestGetBalanceReturnsErrorAsync()
        {
            const ErrorCode expectedErrorCode = ErrorCode.InvalidLoginOrPassword;
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent($"[\"error\",\"{(int)expectedErrorCode}\"]"),
            };
            
            SetupHttpHandlerResponse(response);

            var result = await smsClient.GetBalanceAsync();
            
            Assert.True(result.Failure);
            Assert.Equal(expectedErrorCode, result.Error);
        }
        
        [Theory]
        [InlineData("[\"error\",\"InvalidNumber\"]")]
        [InlineData("[\"error\",\"\"]")]
        [InlineData("[\"err\",\"44\"]")]
        [InlineData("[\"balance\":\"4\"]")]
        [InlineData("[\"bal\",\"4\"]")]
        [InlineData("[\"balance\":\"4\",\"credit\":\"3\"]")]
        [InlineData("[\"balance\":\"4\",\"cred\":\"3\"]")]
        [InlineData("[\"balance\":\"4\",\"credit\":\"3\",\"curr\":\"\"]")]
        [InlineData("[\"error\"]")]
        [InlineData("[]")]
        public async Task TestGetBalanceReturnsUnprocessableResponseAsync(string responseContent)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent),
            };
            
            SetupHttpHandlerResponse(response);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await smsClient.GetBalanceAsync());
        }

        private void SetupHttpHandlerResponse(HttpResponseMessage responseMessage) => handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);
    }
}
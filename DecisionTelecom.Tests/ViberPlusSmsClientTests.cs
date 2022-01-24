using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DecisionTelecom.Exceptions;
using DecisionTelecom.Models;
using DecisionTelecom.Tests.Extensions;
using Moq;
using Xunit;

namespace DecisionTelecom.Tests
{
    public class ViberPlusSmsClientTests
    {
        private readonly Mock<HttpMessageHandler> handlerMock;

        private readonly ViberPlusSmsClient viberPlusSmsClient;

        public ViberPlusSmsClientTests()
        {
            handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);

            viberPlusSmsClient = new ViberPlusSmsClient(httpClient, string.Empty);
        }
        
        [Fact]
        public async Task TestSendMessageReturnsMessageIdAsync()
        {
            const int expectedMessageId = 429;

            var sendMessageResponse = new Dictionary<string, int>
            {
                { "message_id", expectedMessageId },
            };

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(sendMessageResponse)),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var messageId = await viberPlusSmsClient.SendMessageAsync(new ViberPlusSmsMessage());
            
            Assert.Equal(expectedMessageId, messageId);
        }
        
        [Fact]
        public async Task TestSendMessageReturnsViberErrorAsync()
        {
            var error = new ViberError
            {
                Name = "Invalid Parameter: source_addr",
                Message = "Empty parameter or parameter validation error",
                Code = 1,
                Status = 400,
            };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(error)),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var viberException = await Assert.ThrowsAsync<ViberException>(() =>
                viberPlusSmsClient.SendMessageAsync(new ViberPlusSmsMessage()));

            Assert.NotNull(viberException);
            Assert.NotNull(viberException.Error);
            Assert.Equal(error.Name, viberException.Error.Name);
            Assert.Equal(error.Message, viberException.Error.Message);
            Assert.Equal(error.Code, viberException.Error.Code);
            Assert.Equal(error.Status, viberException.Error.Status);
        }
        
        [Fact]
        public async Task TestSendMessageReturnsNotSuccessStatusCodeAsync()
        {
            var expectedMessage = "An error occurred while processing request. Response code: 401 (Unauthorized)";

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = new StringContent(JsonSerializer.Serialize("Some response message text")),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var exception = await Assert.ThrowsAsync<ViberException>(() =>
                viberPlusSmsClient.SendMessageAsync(new ViberPlusSmsMessage()));
            
            Assert.NotNull(exception);
            Assert.Null(exception.Error);
            Assert.Equal(expectedMessage, exception.Message);
        }
        
        [Fact]
        public async Task TestSendMessageReturnsNotSupportedResponseAsync()
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{message: 429}"),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var exception = await Assert.ThrowsAsync<ViberException>(() =>
                viberPlusSmsClient.SendMessageAsync(new ViberPlusSmsMessage()));

            Assert.NotNull(exception);
            Assert.Null(exception.Error);
            Assert.NotNull(exception.Message);
        }
        
        [Fact]
        public async Task TestGetMessageStatusAsyncReturnsMessageIdAsync()
        {
            const int messageId = 429;
            const int smsMessageId = 3098;
            const ViberMessageStatus messageStatus = ViberMessageStatus.Delivered;
            const SmsMessageStatus smsMessageStatus = SmsMessageStatus.Accepted;

            var sendMessageResponse = new Dictionary<string, int>
            {
                { "message_id", messageId },
                { "status", (int)messageStatus },
                { "sms_message_id", smsMessageId },
                { "sms_message_status", (int)smsMessageStatus }
            };
            
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(sendMessageResponse)),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var receipt = await viberPlusSmsClient.GetMessageStatusAsync(messageId);
            
            Assert.NotNull(receipt);
            Assert.Equal(messageId, receipt.ViberMessageId);
            Assert.Equal(messageStatus, receipt.ViberMessageStatus);
            Assert.Equal(smsMessageId, receipt.SmsMessageId);
            Assert.Equal(smsMessageStatus, receipt.SmsMessageStatus);
        }
        
        [Fact]
        public async Task TestGetMessageStatusReturnsViberErrorAsync()
        {
            var error = new ViberError
            {
                Name = "Invalid Parameter: source_addr",
                Message = "Empty parameter or parameter validation error",
                Code = 1,
                Status = 400,
            };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(error)),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var viberException = await Assert.ThrowsAsync<ViberException>(() => viberPlusSmsClient.GetMessageStatusAsync(429));

            Assert.NotNull(viberException);
            Assert.NotNull(viberException.Error);
            Assert.Equal(error.Name, viberException.Error.Name);
            Assert.Equal(error.Message, viberException.Error.Message);
            Assert.Equal(error.Code, viberException.Error.Code);
            Assert.Equal(error.Status, viberException.Error.Status);
        }
        
        [Fact]
        public async Task TestGetMessageStatusReturnsNotSuccessStatusCodeAsync()
        {
            var expectedMessage = "An error occurred while processing request. Response code: 401 (Unauthorized)";
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = new StringContent(JsonSerializer.Serialize("Some error message text")),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var exception = await Assert.ThrowsAsync<ViberException>(() => viberPlusSmsClient.GetMessageStatusAsync(429));

            Assert.NotNull(exception);
            Assert.Null(exception.Error);
            Assert.Equal(expectedMessage, exception.Message);
        }
        
        [Fact]
        public async Task TestGetMessageStatusReturnsNotSupportedResponseAsync()
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{message: 429}"),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var exception = await Assert.ThrowsAsync<ViberException>(() => viberPlusSmsClient.GetMessageStatusAsync(429));

            Assert.NotNull(exception);
            Assert.Null(exception.Error);
            Assert.NotNull(exception.Message);
        }
    }
}
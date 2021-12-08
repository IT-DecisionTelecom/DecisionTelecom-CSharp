using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
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
            const int messageId = 429;

            var sendMessageResponse = new Dictionary<string, int>
            {
                { "message_id", messageId },
            };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(sendMessageResponse)),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var result = await viberPlusSmsClient.SendMessageAsync(new ViberMessage());
            
            Assert.True(result.Success);
            Assert.Equal(messageId, result.Value);
        }
        
        [Fact]
        public async Task TestSendMessageReturnsErrorSuccessStatusCodeAsync()
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

            var result = await viberPlusSmsClient.SendMessageAsync(new ViberMessage());
            
            Assert.True(result.Failure);
            Assert.NotNull(result.Error);
            Assert.Equal(error.Name, result.Error.Name);
            Assert.Equal(error.Message, result.Error.Message);
            Assert.Equal(error.Code, result.Error.Code);
            Assert.Equal(error.Status, result.Error.Status);
        }
        
        [Fact]
        public async Task TestSendMessageReturnsErrorNotSuccessStatusCodeAsync()
        {
            var error = new ViberError
            {
                Name = "Method Not Allowed",
                Message = "Method Not Allowed. This URL can only handle the following request methods: POST.",
                Code = 0,
                Status = 405,
            };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.MethodNotAllowed,
                Content = new StringContent(JsonSerializer.Serialize(error)),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var result = await viberPlusSmsClient.SendMessageAsync(new ViberMessage());
            
            Assert.True(result.Failure);
            Assert.NotNull(result.Error);
            Assert.Equal(error.Name, result.Error.Name);
            Assert.Equal(error.Message, result.Error.Message);
            Assert.Equal(error.Code, result.Error.Code);
            Assert.Equal(error.Status, result.Error.Status);
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

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                viberPlusSmsClient.SendMessageAsync(new ViberMessage()));
        }
        
        [Fact]
        public async Task TestGetMessageStatusAsyncReturnsMessageIdAsync()
        {
            const int messageId = 429;
            const ViberMessageStatus messageStatus = ViberMessageStatus.Delivered;

            var sendMessageResponse = new Dictionary<string, int>
            {
                { "message_id", messageId },
                { "status", (int)messageStatus },
            };
            
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(sendMessageResponse)),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var result = await viberPlusSmsClient.GetMessageStatusAsync(messageId);
            
            Assert.True(result.Success);
            Assert.Equal(messageId, result.Value.ViberMessageId);
            Assert.Equal(messageStatus, result.Value.ViberMessageStatus);
        }
        
        [Fact]
        public async Task TestGetMessageStatusReturnsErrorSuccessStatusCodeAsync()
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

            var result = await viberPlusSmsClient.GetMessageStatusAsync(429);
            
            Assert.True(result.Failure);
            Assert.NotNull(result.Error);
            Assert.Equal(error.Name, result.Error.Name);
            Assert.Equal(error.Message, result.Error.Message);
            Assert.Equal(error.Code, result.Error.Code);
            Assert.Equal(error.Status, result.Error.Status);
        }
        
        [Fact]
        public async Task TestGetMessageStatusReturnsErrorNotSuccessStatusCodeAsync()
        {
            var error = new ViberError
            {
                Name = "Method Not Allowed",
                Message = "Method Not Allowed. This URL can only handle the following request methods: POST.",
                Code = 0,
                Status = 405,
            };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.MethodNotAllowed,
                Content = new StringContent(JsonSerializer.Serialize(error)),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var result = await viberPlusSmsClient.GetMessageStatusAsync(429);
            
            Assert.True(result.Failure);
            Assert.NotNull(result.Error);
            Assert.Equal(error.Name, result.Error.Name);
            Assert.Equal(error.Message, result.Error.Message);
            Assert.Equal(error.Code, result.Error.Code);
            Assert.Equal(error.Status, result.Error.Status);
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

            await Assert.ThrowsAsync<InvalidOperationException>(() => viberPlusSmsClient.GetMessageStatusAsync(429));
        }
    }
}
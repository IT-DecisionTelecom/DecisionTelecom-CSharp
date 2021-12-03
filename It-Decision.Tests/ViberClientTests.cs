using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ITDecision.Tests.Extensions;
using ITDecision.Viber;
using ITDecision.Viber.Models;
using Moq;
using Xunit;

namespace ITDecision.Tests
{
    public class ViberClientTests
    {
        private readonly Mock<HttpMessageHandler> handlerMock;

        private readonly ViberClient viberClient;

        public ViberClientTests()
        {
            handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);

            viberClient = new ViberClient(httpClient, string.Empty);
        }
        
        [Fact]
        public async Task TestSendMessageReturnsMessageIdAsync()
        {
            var sendMessageResponse = new SendMessageResponse { MessageId = 429 };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(sendMessageResponse)),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var result = await viberClient.SendMessageAsync(new SendMessageRequest());
            
            Assert.True(result.Success);
            Assert.Equal(sendMessageResponse.MessageId, result.Value);
        }
        
        [Fact]
        public async Task TestSendMessageReturnsErrorSuccessStatusCodeAsync()
        {
            var error = new Error
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

            var result = await viberClient.SendMessageAsync(new SendMessageRequest());
            
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
            var error = new Error
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

            var result = await viberClient.SendMessageAsync(new SendMessageRequest());
            
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
                viberClient.SendMessageAsync(new SendMessageRequest()));
        }
        
        [Fact]
        public async Task TestGetMessageStatusAsyncReturnsMessageIdAsync()
        {
            var sendMessageResponse = new GetMessageStatusResponse
                { MessageId = 429, Status = MessageStatus.Delivered };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(sendMessageResponse)),
            };
            
            handlerMock.SetupHttpHandlerResponse(response);

            var result = await viberClient.GetMessageStatusAsync(sendMessageResponse.MessageId);
            
            Assert.True(result.Success);
            Assert.Equal(sendMessageResponse.Status, result.Value);
        }
        
        [Fact]
        public async Task TestGetMessageStatusReturnsErrorSuccessStatusCodeAsync()
        {
            var error = new Error
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

            var result = await viberClient.GetMessageStatusAsync(429);
            
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
            var error = new Error
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

            var result = await viberClient.GetMessageStatusAsync(429);
            
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

            await Assert.ThrowsAsync<InvalidOperationException>(() => viberClient.GetMessageStatusAsync(429));
        }
    }
}
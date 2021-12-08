using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace DecisionTelecom.Tests.Extensions
{
    public static class HttpMessageHandlerExtensions
    {
        internal static void SetupHttpHandlerResponse(this Mock<HttpMessageHandler> handlerMock,
            HttpResponseMessage responseMessage) => handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);
    }
}
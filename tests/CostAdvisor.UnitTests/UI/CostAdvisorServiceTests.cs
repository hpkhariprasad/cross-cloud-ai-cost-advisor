using Xunit;
using CostAdvisor.UI.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using CostAdvisor.Shared.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace CostAdvisor.UnitTests.UI
{
    public class CostAdvisorServiceTests
    {
        [Fact]
        public async Task GetResourceCostsAsync_EmptyResponse_ReturnsEmptyList()
        {
            // Arrange
            var httpClient = new HttpClient(new MockHttpMessageHandler("[]"));
            httpClient.BaseAddress = new Uri("http://localhost");
            var service = new CostAdvisorService(httpClient);

            // Act
            var result = await service.GetResourceCostsAsync(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetResourceCostsAsync_ValidResponse_ReturnsDeserializedList()
        {
            // Arrange
            var costs = new List<NormalizedCost>
            {
                new NormalizedCost { Id = 1, Service = "EC2", UsageAmount = 10, Date = DateTime.Today }
            };
            var json = JsonSerializer.Serialize(costs);
            var httpClient = new HttpClient(new MockHttpMessageHandler(json));
            httpClient.BaseAddress = new Uri("http://localhost");
            var service = new CostAdvisorService(httpClient);

            // Act
            var result = await service.GetResourceCostsAsync(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // Assert
            Assert.Single(result);
            Assert.Equal("EC2", result[0].Service);
            Assert.Equal(10, result[0].UsageAmount);
        }

        [Fact]
        public async Task FetchCostsAsync_SuccessfulPost_DoesNotThrow()
        {
            // Arrange
            var httpClient = new HttpClient(new MockHttpMessageHandler("{}", HttpStatusCode.OK, HttpMethod.Post));
            httpClient.BaseAddress = new Uri("http://localhost");
            var service = new CostAdvisorService(httpClient);

            // Act & Assert
            await service.FetchCostsAsync();
        }

        [Fact]
        public async Task FetchCostsAsync_FailedPost_ThrowsException()
        {
            // Arrange
            var httpClient = new HttpClient(new MockHttpMessageHandler("{}", HttpStatusCode.BadRequest, HttpMethod.Post));
            httpClient.BaseAddress = new Uri("http://localhost");
            var service = new CostAdvisorService(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => service.FetchCostsAsync());
        }

        // Enhanced mock handler for different scenarios
        public class MockHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _response;
            private readonly HttpStatusCode _statusCode;
            private readonly HttpMethod _method;

            public MockHttpMessageHandler(string response, HttpStatusCode statusCode = HttpStatusCode.OK, HttpMethod method = null)
            {
                _response = response;
                _statusCode = statusCode;
                _method = method;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(_statusCode)
                {
                    Content = new StringContent(_response, System.Text.Encoding.UTF8, "application/json")
                };
                return Task.FromResult(response);
            }
        }
    }
}

using Xunit;
using CostAdvisor.Infrastructure.Repositories;
using CostAdvisor.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using OpenAI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CostAdvisor.UnitTests.Infrastructure
{
    public class RecommendationServiceTests
    {
        [Fact]
        public async Task GenerateRecommendationsAsync_ReturnsDummyOnException()
        {
            // Arrange
            var openAiMock = new Mock<OpenAIClient>();
            var configMock = new Mock<IConfiguration>();
            var loggerMock = new Mock<ILogger<RecommendationService>>();
            var service = new RecommendationService(openAiMock.Object, configMock.Object, loggerMock.Object);
            var costs = new List<NormalizedCost> { new NormalizedCost { Id = 1, Service = "EC2" } };

            // Act
            var result = await service.GenerateRecommendationsAsync(costs);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count > 0);
        }
    }
}

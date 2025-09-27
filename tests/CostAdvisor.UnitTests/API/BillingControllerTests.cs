using Xunit;
using CostAdvisor.API.Controllers;
using CostAdvisor.Core.Services;
using CostAdvisor.Core.Repositories;
using CostAdvisor.Core.Providers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CostAdvisor.Shared.Models;

namespace CostAdvisor.UnitTests.API
{
    public class BillingControllerTests
    {
        [Fact]
        public async Task GetCosts_ReturnsOkResult()
        {
            // Arrange
            var costRepoMock = new Mock<ICostRepository>();
            var recommendationRepoMock = new Mock<IRecommendationRepository>();
            var cloudProvidersMock = new List<ICloudBillingProvider>();
            var billingService = new BillingService(costRepoMock.Object, null, recommendationRepoMock.Object, cloudProvidersMock);
            costRepoMock.Setup(s => s.GetCostsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), null))
                .ReturnsAsync(new List<NormalizedCost> { new NormalizedCost { Id = 1, Service = "EC2" } });
            var controller = new BillingController(billingService);

            // Act
            var result = await controller.GetCosts(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var costs = Assert.IsAssignableFrom<IEnumerable<NormalizedCost>>(okResult.Value);
            Assert.Single(costs);
        }

        [Fact]
        public async Task GetCosts_EmptyList_ReturnsOkResultWithEmptyList()
        {
            // Arrange
            var costRepoMock = new Mock<ICostRepository>();
            var recommendationRepoMock = new Mock<IRecommendationRepository>();
            var cloudProvidersMock = new List<ICloudBillingProvider>();
            var billingService = new BillingService(costRepoMock.Object, null, recommendationRepoMock.Object, cloudProvidersMock);
            costRepoMock.Setup(s => s.GetCostsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), null))
                .ReturnsAsync(new List<NormalizedCost>());
            var controller = new BillingController(billingService);

            // Act
            var result = await controller.GetCosts(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var costs = Assert.IsAssignableFrom<IEnumerable<NormalizedCost>>(okResult.Value);
            Assert.Empty(costs);
        }

        [Fact]
        public async Task GetCosts_ServiceThrows_ReturnsStatusCode500()
        {
            // Arrange
            var costRepoMock = new Mock<ICostRepository>();
            var recommendationRepoMock = new Mock<IRecommendationRepository>();
            var cloudProvidersMock = new List<ICloudBillingProvider>();
            var billingService = new BillingService(costRepoMock.Object, null, recommendationRepoMock.Object, cloudProvidersMock);
            costRepoMock.Setup(s => s.GetCostsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), null))
                .ThrowsAsync(new Exception("Service error"));
            var controller = new BillingController(billingService);

            // Act
            var result = await controller.GetCosts(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetCosts_InvalidDateRange_ReturnsBadRequest()
        {
            // Arrange
            var costRepoMock = new Mock<ICostRepository>();
            var recommendationRepoMock = new Mock<IRecommendationRepository>();
            var cloudProvidersMock = new List<ICloudBillingProvider>();
            var billingService = new BillingService(costRepoMock.Object, null, recommendationRepoMock.Object, cloudProvidersMock);
            var controller = new BillingController(billingService);

            // Act
            var result = await controller.GetCosts(DateTime.UtcNow, DateTime.UtcNow.AddDays(-1));

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid date range.", badRequestResult.Value);
        }

        [Fact]
        public async Task Fetch_ReturnsOkResult()
        {
            // Arrange
            var costRepoMock = new Mock<ICostRepository>();
            var recommendationRepoMock = new Mock<IRecommendationRepository>();
            var recommendationServiceMock = new Mock<IRecommendationService>();
            var cloudProvidersMock = new List<ICloudBillingProvider>();
            var billingService = new BillingService(costRepoMock.Object, recommendationServiceMock.Object, recommendationRepoMock.Object, cloudProvidersMock);
            var controller = new BillingController(billingService);
            var request = new FetchRequest { AccountId = "acc1", From = DateTime.UtcNow.AddDays(-5), To = DateTime.UtcNow };

            // Act
            var result = await controller.Fetch(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value;
            var messageProp = value.GetType().GetProperty("message");
            Assert.NotNull(messageProp);
            var message = messageProp.GetValue(value) as string;
            Assert.Equal("Costs fetched and stored", message);
        }

        [Fact]
        public async Task GetDashboard_ReturnsOkResult()
        {
            // Arrange
            var costRepoMock = new Mock<ICostRepository>();
            var recommendationRepoMock = new Mock<IRecommendationRepository>();
            var cloudProvidersMock = new List<ICloudBillingProvider>();
            // Setup costs to produce TotalCost=100, ResourceCount=2
            var costs = new List<NormalizedCost>
            {
                new NormalizedCost { UsageAmount = 40 },
                new NormalizedCost { UsageAmount = 60 }
            };
            costRepoMock.Setup(s => s.GetCostsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), null))
                .ReturnsAsync(costs);
            var billingService = new BillingService(costRepoMock.Object, null, recommendationRepoMock.Object, cloudProvidersMock);
            var controller = new BillingController(billingService);

            // Act
            var result = await controller.GetDashboard(DateTime.UtcNow.AddDays(-5), DateTime.UtcNow);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var summary = okResult.Value as DashboardSummary;
            Assert.NotNull(summary);
            Assert.Equal(100, summary.TotalCost);
            Assert.Equal(2, summary.ResourceCount);
        }

        [Fact]
        public async Task GetRecommendations_ReturnsOkResultWithRecommendations()
        {
            // Arrange
            var costRepoMock = new Mock<ICostRepository>();
            var recommendationRepoMock = new Mock<IRecommendationRepository>();
            var cloudProvidersMock = new List<ICloudBillingProvider>();
            var costs = new List<NormalizedCost>
            {
                new NormalizedCost { Id = 1, Recommendations = new List<Recommendation> { new Recommendation { Id = 10 } } },
                new NormalizedCost { Id = 2, Recommendations = new List<Recommendation> { new Recommendation { Id = 20 } } }
            };
            costRepoMock.Setup(s => s.GetCostsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), null))
                .ReturnsAsync(costs);
            var billingService = new BillingService(costRepoMock.Object, null, recommendationRepoMock.Object, cloudProvidersMock);
            var controller = new BillingController(billingService);

            // Act
            var result = await controller.GetRecommendations(DateTime.UtcNow.AddDays(-5), DateTime.UtcNow);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var recs = okResult.Value as IEnumerable<Recommendation>;
            Assert.NotNull(recs);
            var recList = new List<Recommendation>(recs);
            Assert.Equal(2, recList.Count);
            Assert.Contains(recList, r => r.Id == 10);
            Assert.Contains(recList, r => r.Id == 20);
        }
    }
}

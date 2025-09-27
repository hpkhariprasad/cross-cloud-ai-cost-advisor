using CostAdvisor.Core.Providers;
using CostAdvisor.Core.Repositories;
using CostAdvisor.Core.Services;
using CostAdvisor.Shared.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CostAdvisor.UnitTests.Core
{
    public class BillingServiceTests
    {
        [Fact]
        public async Task GetCostsAsync_ReturnsCostsInRange()
        {
            // Arrange
            var repoMock = new Mock<ICostRepository>();
            var recommendationServiceMock = new Mock<IRecommendationService>();
            var recommendationRepositoryMock = new Mock<IRecommendationRepository>();
            repoMock.Setup(r => r.GetCostsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(),null))
                .ReturnsAsync(new List<NormalizedCost> { new NormalizedCost { Id = 1, Service = "EC2" } });
            recommendationRepositoryMock.Setup(r => r.SaveRecommendationsAsync(It.IsAny<IEnumerable<Recommendation>>()))
                .Returns(Task.CompletedTask);
            recommendationServiceMock.Setup(r => r.GenerateRecommendationsAsync(It.IsAny<IEnumerable<NormalizedCost>>()))
                .ReturnsAsync(new Dictionary<int, List<Recommendation>>());


            var service = new BillingService(repoMock.Object, recommendationServiceMock.Object,recommendationRepositoryMock.Object , new List<ICloudBillingProvider>());

            // Act
            var result = await service.GetCostsAsync(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // Assert
            Assert.Single(result);
            Assert.Equal("EC2", result.First().Service);
        }

        [Fact]
        public async Task FetchAndStoreAsync_CallsRepositoryForEachProvider()
        {
            // Arrange
            var repoMock = new Mock<ICostRepository>();
            var providerMock1 = new Mock<ICloudBillingProvider>();
            var providerMock2 = new Mock<ICloudBillingProvider>();
            var recommendationServiceMock = new Mock<IRecommendationService>();
            var recommendationRepositoryMock = new Mock<IRecommendationRepository>();
            // Replace this line:
            // UsageRecord nn = new UsageRecord { Date = DateTime.Today, Service = "EC2", Region = "us-east-1", Cost = 10 };

            // With the following line to fix CS7036 and IDE0090:
            List<UsageRecord> nn = new List<UsageRecord> { new UsageRecord(DateTime.Today, null, "EC2", 10, "us-east-1") };

            providerMock1.Setup(p => p.GetBillingDataAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new BillingData(100, new List<UsageRecord> { new UsageRecord(DateTime.Today, null, "EC2", 10, "us-east-1") }));

            providerMock2.Setup(p => p.GetBillingDataAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new BillingData(200, new List<UsageRecord> { new UsageRecord(DateTime.Today, null, "Blob", 5, "eastus") }));
            recommendationRepositoryMock.Setup(r => r.SaveRecommendationsAsync(It.IsAny<IEnumerable<Recommendation>>()))
               .Returns(Task.CompletedTask);
            recommendationServiceMock.Setup(r => r.GenerateRecommendationsAsync(It.IsAny<IEnumerable<NormalizedCost>>()))
                .ReturnsAsync(new Dictionary<int, List<Recommendation>>());

            var providers = new List<ICloudBillingProvider> { providerMock1.Object, providerMock2.Object };

            var service = new BillingService(repoMock.Object, recommendationServiceMock.Object, recommendationRepositoryMock.Object, providers);

            // Act
            await service.FetchAndStoreAsync("acc1", DateTime.Today.AddDays(-1), DateTime.Today);

            //Assert
            repoMock.Verify(r => r.SaveCostsAsync(It.IsAny<IEnumerable<NormalizedCost>>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GetCostsAsync_EmptyResult_ReturnsEmptyList()
        {
            // Arrange
            var repoMock = new Mock<ICostRepository>();
            var recommendationServiceMock = new Mock<IRecommendationService>();
            var recommendationRepositoryMock = new Mock<IRecommendationRepository>();

            repoMock.Setup(r => r.GetCostsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), null))
                .ReturnsAsync(new List<NormalizedCost>());
            recommendationRepositoryMock.Setup(r => r.SaveRecommendationsAsync(It.IsAny<IEnumerable<Recommendation>>()))
               .Returns(Task.CompletedTask);
            recommendationServiceMock.Setup(r => r.GenerateRecommendationsAsync(It.IsAny<IEnumerable<NormalizedCost>>()))
                .ReturnsAsync(new Dictionary<int, List<Recommendation>>());

            var service = new BillingService(repoMock.Object, recommendationServiceMock.Object, recommendationRepositoryMock.Object, new List<ICloudBillingProvider>());

            // Act
            var result = await service.GetCostsAsync(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetDashboardSummaryAsync_ReturnsCorrectSummary()
        {
            // Arrange
            var repoMock = new Mock<ICostRepository>();
            var recommendationServiceMock = new Mock<IRecommendationService>();
            var recommendationRepositoryMock = new Mock<IRecommendationRepository>();
            var costs = new List<NormalizedCost>
            {
                new NormalizedCost {
                    Id = 1,
                    UsageAmount = 40,
                    Service = "EC2",
                    Date = new DateTime(2024, 6, 1),
                    Account = new Account { AccountIdentifier = "aws-1" }
                },
                new NormalizedCost {
                    Id = 2,
                    UsageAmount = 60,
                    Service = "S3",
                    Date = new DateTime(2024, 6, 1),
                    Account = new Account { AccountIdentifier = "aws-1" }
                },
                new NormalizedCost {
                    Id = 3,
                    UsageAmount = 100,
                    Service = "Blob",
                    Date = new DateTime(2024, 5, 1),
                    Account = new Account { AccountIdentifier = "azure-1" }
                }
            };
            repoMock.Setup(r => r.GetCostsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), null))
                .ReturnsAsync(costs);
            var service = new BillingService(repoMock.Object, recommendationServiceMock.Object, recommendationRepositoryMock.Object, new List<ICloudBillingProvider>());

            // Act
            var summary = await service.GetDashboardSummaryAsync(DateTime.UtcNow.AddMonths(-2), DateTime.UtcNow);

            // Assert
            Assert.Equal(200, summary.TotalCost); // 40+60+100
            Assert.Equal(3, summary.ResourceCount);
            Assert.Equal("aws-1", summary.TopProvider); // 100 > 100 (azure-1), aws-1 = 100
            Assert.Equal("Blob", summary.TopService); // 100 (Blob) > 60 (S3) > 40 (EC2)
            Assert.Equal(2, summary.ProviderBreakdown.Count);
            Assert.Contains(summary.ProviderBreakdown, p => p.Provider == "aws-1" && p.TotalCost == 100 && p.ResourceCount == 2);
            Assert.Contains(summary.ProviderBreakdown, p => p.Provider == "azure-1" && p.TotalCost == 100 && p.ResourceCount == 1);
            Assert.Equal(3, summary.ServiceBreakdown.Count);
            Assert.Contains(summary.ServiceBreakdown, s => s.Service == "Blob" && s.TotalCost == 100);
            Assert.Contains(summary.ServiceBreakdown, s => s.Service == "S3" && s.TotalCost == 60);
            Assert.Contains(summary.ServiceBreakdown, s => s.Service == "EC2" && s.TotalCost == 40);
            Assert.Equal(2, summary.MonthlyTrends.Count); // 2024-05, 2024-06
            Assert.Contains(summary.MonthlyTrends, m => m.Year == 2024 && m.Month == 5 && m.TotalCost == 100);
            Assert.Contains(summary.MonthlyTrends, m => m.Year == 2024 && m.Month == 6 && m.TotalCost == 100);
        }
    }
}
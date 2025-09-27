using Xunit;
using CostAdvisor.Shared.Models;

namespace CostAdvisor.UnitTests.Shared
{
    public class RecommendationTests
    {
        [Fact]
        public void Recommendation_Properties_Work()
        {
            var rec = new Recommendation
            {
                Id = 1,
                Message = "Test",
                Confidence = 90,
                EstimatedSavings = 100,
                CreatedOn = System.DateTime.UtcNow
            };
            Assert.Equal(1, rec.Id);
            Assert.Equal("Test", rec.Message);
            Assert.Equal(90, rec.Confidence);
            Assert.Equal(100, rec.EstimatedSavings);
        }
    }
}

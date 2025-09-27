using Xunit;
using CostAdvisor.Shared.Models;

namespace CostAdvisor.UnitTests.Shared
{
    public class NormalizedCostTests
    {
        [Fact]
        public void NormalizedCost_Properties_Work()
        {
            var cost = new NormalizedCost
            {
                Id = 1,
                Service = "EC2",
                UsageAmount = 10,
                Date = System.DateTime.Today
            };
            Assert.Equal(1, cost.Id);
            Assert.Equal("EC2", cost.Service);
            Assert.Equal(10, cost.UsageAmount);
        }
    }
}

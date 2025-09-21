using CostAdvisor.Core.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostAdvisor.Infrastructure.Providers
{
    public class GCPBillingProviderDummy:ICloudBillingProvider
    {
        private static readonly Random _random = new();

        public Task<decimal> GetMonthlyCostAsync(string accountId)
        {
            var cost = _random.Next(200, 1000); // Fake monthly cost
            return Task.FromResult((decimal)cost);
        }

        public Task<IEnumerable<UsageRecord>> GetUsageDetailsAsync(string accountId, DateTime start, DateTime end)
        {
            var data = Enumerable.Range(0, (end - start).Days + 1)
                .Select(i => new UsageRecord(
                    start.AddDays(i),
                    "gcp-proj-" + _random.Next(1000, 1003),
                    _random.Next(0, 2) == 0 ? "Compute Engine" : "Cloud Storage",
                    (decimal)_random.NextDouble() * 40m,
                    _random.Next(0, 2) == 0 ? "us-central1" : "europe-west1"
                ));

            return Task.FromResult(data);
        }

        public async Task<BillingData> GetBillingDataAsync(string accountId, DateTime start, DateTime end)
        {
            var monthlyCost = await GetMonthlyCostAsync(accountId);
            var usage = await GetUsageDetailsAsync(accountId, start, end);
            return new BillingData(monthlyCost, usage);
        }
    }
}

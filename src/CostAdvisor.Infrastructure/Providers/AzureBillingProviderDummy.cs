using CostAdvisor.Core.Providers;
using CostAdvisor.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostAdvisor.Infrastructure.Providers
{
    public class AzureBillingProviderDummy :ICloudBillingProvider
    {
        private static readonly Random _random = new();
        public Task<decimal> GetMonthlyCostAsync(string accountId)
        {
            var cost = _random.Next(500, 1500); // Fake monthly cost
            return Task.FromResult((decimal)cost);
        }
        public Task<IEnumerable<UsageRecord>> GetUsageDetailsAsync(string accountId, DateTime start, DateTime end)
        {
            var data = Enumerable.Range(0, (end - start).Days + 1)
                .Select(i => new UsageRecord(
                    start.AddDays(i),
                    "azure-sub-" + _random.Next(1000, 1003),
                    _random.Next(0, 2) == 0 ? "Azure App Service" : "Azure SQL Database",
                    (decimal)_random.NextDouble() * 50m ,
                    _random.Next(0,2) ==0 ? "East US" : "West Europe"
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

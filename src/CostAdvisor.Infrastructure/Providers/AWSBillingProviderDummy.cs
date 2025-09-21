using CostAdvisor.Core.Providers;
using CostAdvisor.Shared.Models;

namespace CostAdvisor.Infrastructure.Providers
{
    public class AWSBillingProviderDummy:ICloudBillingProvider
    {
        private static readonly Random _random = new();

        public Task<decimal> GetMonthlyCostAsync(string accountId)
        {
            var cost = _random.Next(1000, 2500); // Fake AWS monthly cost
            return Task.FromResult((decimal)cost);
        }

        public Task<IEnumerable<UsageRecord>> GetUsageDetailsAsync(string accountId, DateTime start, DateTime end)
        {
            var services = new[] { "EC2", "S3", "RDS", "Lambda" };
            var regions = new[] { "us-east-1", "us-west-2", "eu-west-1", "ap-southeast-1" };
            var accountIdentifier = new[] { "aws-ac-1234", "aws-ac-356", "aws-ac-555" };
            var data = Enumerable.Range(0, (end - start).Days + 1)
                .Select(i => new UsageRecord(
                    start.AddDays(i),
                    accountIdentifier[_random.Next(accountIdentifier.Length)],
                    services[_random.Next(services.Length)],
                    (decimal)_random.NextDouble() * 60m,
                    regions[_random.Next(regions.Length)]
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

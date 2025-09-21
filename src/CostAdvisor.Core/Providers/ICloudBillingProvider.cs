using CostAdvisor.Shared.Models;

namespace CostAdvisor.Core.Providers
{
    public interface ICloudBillingProvider
    {
        Task<decimal> GetMonthlyCostAsync(string accountId);
        Task<IEnumerable<UsageRecord>> GetUsageDetailsAsync(string accountId, DateTime start, DateTime end);

        Task<BillingData> GetBillingDataAsync(string accountId, DateTime start, DateTime end);
    }

    public record UsageRecord(DateTime Date,string AccountIdentifier, string Service, decimal Cost,string Region);

    public record BillingData(decimal MonthlyCost, IEnumerable<UsageRecord> UsageDetails);
}

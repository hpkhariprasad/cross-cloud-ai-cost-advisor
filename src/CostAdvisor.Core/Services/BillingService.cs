using CostAdvisor.Shared.Models;
using CostAdvisor.Core.Repositories;
using CostAdvisor.Core.Providers;

namespace CostAdvisor.Core.Services;

public class BillingService
{
    private readonly ICostRepository _repository;

    private readonly IEnumerable<ICloudBillingProvider> _cloudBillingProviders;

    public BillingService(ICostRepository repository, IEnumerable<ICloudBillingProvider> cloudBillingProviders)
    {
        _repository = repository;
        _cloudBillingProviders = cloudBillingProviders;
    }

    // For MVP, mock costs instead of real API calls
    public async Task FetchAndStoreAsync(string accountId, DateTime start, DateTime end)
    {
        foreach (var provider in _cloudBillingProviders)
        {
            var billingData = await provider.GetBillingDataAsync(accountId, start, end);
            var groupedByAccount = billingData.UsageDetails
        .GroupBy(u => u.AccountIdentifier);

            foreach (var accountGroup in groupedByAccount)
            {
                var accountCosts = new List<NormalizedCost>();
                foreach (var usage in accountGroup)
                {
                    accountCosts.Add(new NormalizedCost
                    {
                        AccountId = 0,
                        Region = usage.Region,
                        Date = usage.Date,
                        Service = usage.Service,
                        UsageAmount = Math.Round(usage.Cost, 2)
                    });
                }
                var providerName = provider.GetType().Name.Replace("Dummy", "").Replace("BillingProvider", "");

                // Save costs for this account identifier
                await _repository.SaveCostsAsync(accountCosts, providerName, accountGroup.Key);
            }
        }
    }

    public Task<IEnumerable<NormalizedCost>> GetCostsAsync(DateTime from, DateTime to) =>
        _repository.GetCostsAsync(from, to);
}

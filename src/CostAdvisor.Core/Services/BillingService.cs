using CostAdvisor.Shared.Models;
using CostAdvisor.Core.Repositories;
using CostAdvisor.Core.Providers;

namespace CostAdvisor.Core.Services;

public class BillingService
{
    private readonly ICostRepository _repository;

    private readonly IEnumerable<ICloudBillingProvider> _cloudBillingProviders;

    public BillingService(ICostRepository repository,IEnumerable<ICloudBillingProvider> cloudBillingProviders)
    {
        _repository = repository;
        _cloudBillingProviders = cloudBillingProviders;
    }

    // For MVP, mock costs instead of real API calls
    public async Task FetchAndStoreAsync(string accountId, DateTime start, DateTime end)
    {
        var allCosts = new List<NormalizedCost>();

        foreach (var provider in _cloudBillingProviders)
        {
            var billingData = await provider.GetBillingDataAsync(accountId, start, end);

            foreach (var usage in billingData.UsageDetails)
            {
                allCosts.Add(new NormalizedCost
                {
                    Provider = provider.GetType().Name.Replace("Dummy", "").Replace("BillingProvider",""),
                    Region = usage.Region,
                    Date = usage.Date,
                    Service = usage.Service,
                    Cost = Math.Round(usage.Cost,2)
                });
            }

            await _repository.SaveCostsAsync(allCosts);
        }
    }

    public Task<IEnumerable<NormalizedCost>> GetCostsAsync(DateTime from, DateTime to) =>
        _repository.GetCostsAsync(from, to);
}

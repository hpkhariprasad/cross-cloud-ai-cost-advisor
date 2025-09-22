using CostAdvisor.Shared.Models;
using CostAdvisor.Core.Repositories;
using CostAdvisor.Core.Providers;

namespace CostAdvisor.Core.Services;

public class BillingService
{
    private readonly ICostRepository _repository;

    private readonly IRecommendationService _recommendationService;

    private readonly IRecommendationRepository _recommendationRepository;

    private readonly IEnumerable<ICloudBillingProvider> _cloudBillingProviders;

    public BillingService(ICostRepository repository,IRecommendationService recommendationService, IRecommendationRepository recommendationRepository , IEnumerable<ICloudBillingProvider> cloudBillingProviders)
    {
        _repository = repository;
        _recommendationService = recommendationService;
        _recommendationRepository = recommendationRepository;
        _cloudBillingProviders = cloudBillingProviders;
    }

    // For MVP, mock costs instead of real API calls
    public async Task FetchAndStoreAsync(string accountId, DateTime start, DateTime end)
    {
        List<NormalizedCost> allCosts = new();
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
                allCosts.AddRange(accountCosts);
            }
        }
        if(allCosts.Any())
        {
            var recsByCostId = await _recommendationService.GenerateRecommendationsAsync(allCosts);
            var allRecs = recsByCostId.Values.SelectMany(x => x);
            await _recommendationRepository.SaveRecommendationsAsync(allRecs);
        }
    }

    public Task<IEnumerable<NormalizedCost>> GetCostsAsync(DateTime from, DateTime to) =>
        _repository.GetCostsAsync(from, to);

    public Task<DashboardSummary> GetDashboardSummaryAsync(DateTime from, DateTime to)
    {
        var costs = _repository.GetCostsAsync(from, to).Result;
        var summary = new DashboardSummary
        {
            TotalCost = costs.Sum(c => c.UsageAmount),
            ResourceCount = costs.Count(),
            TopProvider = costs.GroupBy(c => c.Account?.AccountIdentifier ?? "")
                                  .OrderByDescending(g => g.Sum(x => x.UsageAmount))
                                  .FirstOrDefault()?.Key ?? "",
            TopService = costs.GroupBy(c => c.Service)
                                 .OrderByDescending(g => g.Sum(x => x.UsageAmount))
                                 .FirstOrDefault()?.Key ?? "",
            ProviderBreakdown = costs.GroupBy(c => c.Account?.AccountIdentifier ?? "")
                                        .Select(g => new ProviderCostSummary
                                        {
                                            Provider = g.Key,
                                            TotalCost = g.Sum(x => x.UsageAmount),
                                            ResourceCount = g.Count()
                                        }).ToList(),
            ServiceBreakdown = costs.GroupBy(c => c.Service)
                                       .OrderByDescending(g => g.Sum(x => x.UsageAmount))
                                       .Take(5)
                                       .Select(g => new ServiceCostSummary
                                       {
                                           Service = g.Key,
                                           TotalCost = g.Sum(x => x.UsageAmount)
                                       }).ToList(),
            MonthlyTrends = costs.GroupBy(c => new { c.Date.Year, c.Date.Month })
                                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                                    .Select(g => new MonthlyCostTrend
                                    {
                                        Year = g.Key.Year,
                                        Month = g.Key.Month,
                                        TotalCost = g.Sum(x => x.UsageAmount)
                                    }).ToList()
        };
        return Task.FromResult(summary);
    }
}

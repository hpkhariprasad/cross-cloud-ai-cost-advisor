using CostAdvisor.Shared.Models;
using CostAdvisor.Core.Repositories;

namespace CostAdvisor.Core.Services;

public class BillingService
{
    private readonly ICostRepository _repository;

    public BillingService(ICostRepository repository)
    {
        _repository = repository;
    }

    // For MVP, mock costs instead of real API calls
    public async Task FetchAndStoreAsync()
    {
        var mockCosts = new List<NormalizedCost>
        {
            new() { Provider = "AWS", Service = "EC2", Region = "us-east-1", Date = DateTime.UtcNow.Date, UsageAmount = 10, Cost = 50 },
            new() { Provider = "Azure", Service = "VM", Region = "eastus", Date = DateTime.UtcNow.Date, UsageAmount = 5, Cost = 40 }
        };

        await _repository.SaveCostsAsync(mockCosts);
    }

    public Task<IEnumerable<NormalizedCost>> GetCostsAsync(DateTime from, DateTime to) =>
        _repository.GetCostsAsync(from, to);
}

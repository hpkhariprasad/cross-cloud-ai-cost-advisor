using CostAdvisor.Shared.Models;


namespace CostAdvisor.Core.Repositories;
public interface ICostRepository
{
    Task SaveCostsAsync(IEnumerable<NormalizedCost> costs, string providerName, string accountIdentifier);
    Task<IEnumerable<NormalizedCost>> GetCostsAsync(DateTime from, DateTime to, string? providerName = null);
}
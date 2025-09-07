using CostAdvisor.Core.Models;

namespace CostAdvisor.Core.Repositories;

public interface ICostRepository
{
    Task SaveCostsAsync(IEnumerable<NormalizedCost> costs);
    Task<IEnumerable<NormalizedCost>> GetCostsAsync(DateTime from, DateTime to);
}

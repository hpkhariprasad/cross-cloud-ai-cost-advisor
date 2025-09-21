using System.Collections.Generic;
using System.Threading.Tasks;
using CostAdvisor.Shared.Models;

namespace CostAdvisor.UI.Services;
public interface ICostAdvisorService
{
    Task FetchCostsAsync();
    Task<DashboardSummary?> GetDashboardAsync(DateTime from,DateTime to);
    Task<List<NormalizedCost>> GetResourceCostsAsync(DateTime from, DateTime to);
    Task<List<Recommendation>> GetRecommendationsAsync(DateTime from, DateTime to);
}

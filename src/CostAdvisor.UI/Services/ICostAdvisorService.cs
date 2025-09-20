using System.Collections.Generic;
using System.Threading.Tasks;
using CostAdvisor.Shared.Models;

namespace CostAdvisor.UI.Services;
public interface ICostAdvisorService
{
    Task<List<NormalizedCost>> GetResourceCostsAsync();
    Task FetchCostsAsync();
}

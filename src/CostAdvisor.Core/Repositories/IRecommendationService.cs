using CostAdvisor.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostAdvisor.Core.Repositories
{
    public interface IRecommendationService
    {
        Task<IDictionary<int, List<Recommendation>>> GenerateRecommendationsAsync(IEnumerable<NormalizedCost> costs);
    }

}

using CostAdvisor.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostAdvisor.Core.Repositories
{
    public interface IRecommendationRepository
    {
        Task SaveRecommendationsAsync(IEnumerable<Recommendation> recs);
        Task<IEnumerable<Recommendation>> GetRecommendationsByCostIdAsync(int costId);
        Task<IEnumerable<Recommendation>> GetAllRecommendationsAsync();
    }
}

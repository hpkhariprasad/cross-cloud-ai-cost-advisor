using CostAdvisor.Core.Repositories;
using CostAdvisor.Infrastructure.Data;
using CostAdvisor.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostAdvisor.Infrastructure.Repositories;
public class RecommendationRepository : IRecommendationRepository
{
    private readonly CostAdvisorDbContext _db;

    public RecommendationRepository(CostAdvisorDbContext db)
    {
        _db = db;
    }

    public async Task SaveRecommendationsAsync(IEnumerable<Recommendation> recs)
    {
        _db.Recommendations.AddRange(recs);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Recommendation>> GetRecommendationsByCostIdAsync(int costId)
    {
        return await _db.Recommendations
            .Where(r => r.CostId == costId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Recommendation>> GetAllRecommendationsAsync()
    {
        return await _db.Recommendations.ToListAsync();
    }
}

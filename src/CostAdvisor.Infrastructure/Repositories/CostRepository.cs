using CostAdvisor.Shared.Models;
using CostAdvisor.Core;
using CostAdvisor.Core.Repositories;
using CostAdvisor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CostAdvisor.Infrastructure.Repositories;
    public class CostRepository : ICostRepository
    {
        private readonly CostAdvisorDbContext _db;

        public CostRepository(CostAdvisorDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Saves costs for a given provider + account. 
        /// Automatically creates provider/account if not present.
        /// </summary>
        public async Task SaveCostsAsync(IEnumerable<NormalizedCost> costs, string providerName, string accountIdentifier)
        {
            if (!costs.Any())
                return;

            // Ensure provider exists
            var provider = await _db.Providers
                .FirstOrDefaultAsync(p => p.Name == providerName);

            if (provider == null)
            {
                provider = new Provider { Name = providerName };
                _db.Providers.Add(provider);
                await _db.SaveChangesAsync();
            }

            // Ensure account exists
            var account = await _db.Accounts
                .FirstOrDefaultAsync(a => a.AccountIdentifier == accountIdentifier && a.ProviderId == provider.Id);

            if (account == null)
            {
                account = new Account
                {
                    ProviderId = provider.Id,
                    AccountIdentifier = accountIdentifier
                };
                _db.Accounts.Add(account);
                await _db.SaveChangesAsync();
            }

            // Link costs to account
            foreach (var cost in costs)
            {
                cost.AccountId = account.Id;
                _db.NormalizedCosts.Add(cost);
            }

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Fetches costs between two dates. 
        /// Optionally filter by provider name.
        /// </summary>
        public async Task<IEnumerable<NormalizedCost>> GetCostsAsync(DateTime from, DateTime to, string? providerName = null)
        {
            var query = _db.NormalizedCosts
                .Include(c => c.Account)
                    .ThenInclude(a => a.Provider)
                .Include(c => c.Recommendations)
                .Where(c => c.Date >= from && c.Date <= to);

            if (!string.IsNullOrEmpty(providerName))
            {
                query = query.Where(c => c.Account.Provider.Name == providerName);
            }

            return await query.AsNoTracking().ToListAsync();
        }
    }



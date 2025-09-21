using CostAdvisor.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostAdvisor.Infrastructure.Data
{
    public class CostAdvisorDbContext : DbContext
    {
        public CostAdvisorDbContext(DbContextOptions<CostAdvisorDbContext> options) : base(options)
        {
        }
        public DbSet<Provider> Providers { get; set; } = null!;
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<NormalizedCost> NormalizedCosts { get; set; } = null!;
        public DbSet<Recommendation> Recommendations { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Fluent API configurations can go here if needed
            modelBuilder.Entity<Provider>()
               .HasMany(p => p.Accounts)
               .WithOne(a => a.Provider)
               .HasForeignKey(a => a.ProviderId);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Costs)
                .WithOne(c => c.Account)
                .HasForeignKey(c => c.AccountId);

            modelBuilder.Entity<NormalizedCost>()
                .HasMany(c => c.Recommendations)
                .WithOne(r => r.Cost)
                .HasForeignKey(r => r.CostId);
        }

    }
}

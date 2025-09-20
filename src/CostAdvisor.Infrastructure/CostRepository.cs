using CostAdvisor.Shared.Models;
using CostAdvisor.Core;
using CostAdvisor.Core.Repositories;

namespace CostAdvisor.Infrastructure;

public class CostRepository :ICostRepository
{
    private List<NormalizedCost> _inMemory = new(); 
    // Later replace with EF Core + SQL Server/Postgres

    public CostRepository()
    {
        _inMemory.AddRange(new[]
        {
            new NormalizedCost { Provider = "AWS", Service = "EC2", Region = "us-east-1", Date = DateTime.Today.AddDays(-4), UsageAmount = 10, Cost = 25.50m },
            new NormalizedCost { Provider = "Azure", Service = "VM", Region = "eastus", Date = DateTime.Today.AddDays(-3), UsageAmount = 8, Cost = 20.00m },
            new NormalizedCost { Provider = "AWS", Service = "S3", Region = "us-west-2", Date = DateTime.Today.AddDays(-2), UsageAmount = 15, Cost = 30.75m },
            new NormalizedCost { Provider = "Azure", Service = "Blob", Region = "westeurope", Date = DateTime.Today.AddDays(-1), UsageAmount = 12, Cost = 22.10m },
            new NormalizedCost { Provider = "AWS", Service = "Lambda", Region = "ap-southeast-1", Date = DateTime.Today, UsageAmount = 5, Cost = 10.00m }
        });
    }

    public Task SaveCostsAsync(IEnumerable<NormalizedCost> costs)
    {
        _inMemory.AddRange(costs);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<NormalizedCost>> GetCostsAsync(DateTime from, DateTime to)
    {
        var results = _inMemory
            .Where(c => c.Date >= from && c.Date <= to)
            .ToList();

        return Task.FromResult<IEnumerable<NormalizedCost>>(results);
    }
}

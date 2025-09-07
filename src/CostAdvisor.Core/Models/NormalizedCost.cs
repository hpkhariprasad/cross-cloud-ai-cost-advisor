namespace CostAdvisor.Core.Models;

public class NormalizedCost
{
    public string Provider { get; set; } = string.Empty;   // AWS or Azure
    public string Service { get; set; } = string.Empty;    // e.g., EC2, S3, VM
    public string Region { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal UsageAmount { get; set; }
    public decimal Cost { get; set; }
}

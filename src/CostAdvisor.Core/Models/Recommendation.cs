namespace CostAdvisor.Core.Models;

public class Recommendation
{
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public string Summary { get; set; } = string.Empty;   // Plain-text insight
}

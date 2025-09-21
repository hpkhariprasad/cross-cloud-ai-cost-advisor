using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostAdvisor.Shared.Models
{
    public class DashboardSummary
    {
        public decimal TotalCost { get; set; }
        public int ResourceCount { get; set; }
        public string TopProvider { get; set; } = string.Empty;
        public string TopService { get; set; } = string.Empty;

        public List<ProviderCostSummary> ProviderBreakdown { get; set; } = new();
        public List<ServiceCostSummary> ServiceBreakdown { get; set; } = new();
        public List<MonthlyCostTrend> MonthlyTrends { get; set; } = new();
    }
    public class ProviderCostSummary
    {
        public string Provider { get; set; } = string.Empty;
        public decimal TotalCost { get; set; }
        public int ResourceCount { get; set; }
    }

    public class ServiceCostSummary
    {
        public string Service { get; set; } = string.Empty;
        public decimal TotalCost { get; set; }
    }

    public class MonthlyCostTrend
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalCost { get; set; }
    }

  
}

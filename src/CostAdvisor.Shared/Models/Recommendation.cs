using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostAdvisor.Shared.Models
{
    public class Recommendation
    {
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public string Summary { get; set; } = string.Empty;   // Plain-text insight
    }
}

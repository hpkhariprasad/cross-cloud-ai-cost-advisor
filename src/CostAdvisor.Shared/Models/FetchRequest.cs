using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostAdvisor.Shared.Models
{
    public class FetchRequest
    {
        public string AccountId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}

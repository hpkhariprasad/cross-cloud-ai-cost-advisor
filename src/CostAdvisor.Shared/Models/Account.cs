using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostAdvisor.Shared.Models
{
    [Table("Accounts")]
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProviderId { get; set; }

        [Required, MaxLength(200)]
        public string AccountIdentifier { get; set; } = string.Empty;

        // Navigation
        public Provider Provider { get; set; } = null!;
        public ICollection<NormalizedCost> Costs { get; set; } = new List<NormalizedCost>();
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CostAdvisor.Shared.Models
{
    [Table("NormalizedCosts")]
    public class NormalizedCost
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AccountId { get; set; }

        [Required, MaxLength(50)]
        public string Region { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Service { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal UsageAmount { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        // Navigation
        public Account Account { get; set; } = null!;
        public ICollection<Recommendation> Recommendations { get; set; } = new List<Recommendation>();

        public string BillingAccount { get; set; } = string.Empty;
        public string ResourceName { get; set; }=string.Empty;
        public Dictionary<string,string>? Tags { get; set; }
    }
}

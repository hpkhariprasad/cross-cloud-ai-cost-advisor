using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CostAdvisor.Shared.Models
{
    [Table("Recommendations")]
    public class Recommendation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CostId { get; set; }

        [Required, MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        [Column(TypeName = "decimal(5,2)")]
        public decimal Confidence { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        // Navigation
        public NormalizedCost Cost { get; set; } = null!;
    }
}

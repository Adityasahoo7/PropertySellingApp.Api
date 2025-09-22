using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.Models.Entities
{
    public class Property
    {
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(2000)]
        public string? Description { get; set; }
        [Required]
        public string Type { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        [Required, MaxLength(300)]
        public string Location { get; set; } = string.Empty;
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public double AreaSqFt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? ImageUrl { get; set; }
        public int SellerId { get; set; }
        public User? Seller { get; set; }


        public ICollection<VisitRequest> VisitRequests { get; set; } = new List<VisitRequest>();
    }
}

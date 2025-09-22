using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.Models.Entities
{
    public class VisitRequest
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public User? Buyer { get; set; }


        public int PropertyId { get; set; }
        public Property? Property { get; set; }


        public DateTime RequestedOn { get; set; } = DateTime.UtcNow;
        [MaxLength(500)]
        public string? Message { get; set; }
        public VisitStatus Status { get; set; } = VisitStatus.Pending;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        [Required, MaxLength(120)]
        public string FullName { get; set; } = string.Empty;
        [Required, EmailAddress, MaxLength(180)]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = string.Empty;

        public ICollection<Property> Properties { get; set; } = new List<Property>();
        public ICollection<VisitRequest> VisitRequests { get; set; } = new List<VisitRequest>();
    }
}

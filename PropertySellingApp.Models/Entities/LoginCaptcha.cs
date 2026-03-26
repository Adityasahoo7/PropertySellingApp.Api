using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.Models.Entities
{
    public class LoginCaptcha
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Code { get; set; } = string.Empty;
        public DateTime ExpiryTimeUtc { get; set; }
        public bool IsUsed { get; set; }
    }

}

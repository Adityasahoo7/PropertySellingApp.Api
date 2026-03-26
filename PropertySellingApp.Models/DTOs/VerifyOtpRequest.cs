using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.Models.DTOs
{
    public class VerifyOtpRequest
    {
        [Required]
        public string Otp { get; set; }

        [Required]
        public string Captcha { get; set; }
    }
}

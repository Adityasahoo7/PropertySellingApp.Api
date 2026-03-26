using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.Models.DTOs
{
    public class TempAuthResponse
    {
        public bool OtpSent { get; set; }
        public string TempToken { get; set; } = string.Empty;
        public string CaptchaCode { get; set; }
    }
}

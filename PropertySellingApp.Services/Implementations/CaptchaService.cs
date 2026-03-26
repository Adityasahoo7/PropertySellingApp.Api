using PropertySellingApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.Services.Implementations
{
    public class CaptchaService : ICaptchaService
    {
        public string Generate()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var random = new Random();

            return new string(
                Enumerable.Repeat(chars, 6)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());
        }
    }

}

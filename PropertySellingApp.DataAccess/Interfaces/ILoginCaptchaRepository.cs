using PropertySellingApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.DataAccess.Interfaces
{
    public interface ILoginCaptchaRepository
    {
        Task AddAsync(LoginCaptcha captcha);
        Task<LoginCaptcha?> GetValidCaptchaAsync(int userId, string code);
        Task MarkAsUsedAsync(LoginCaptcha captcha);
    }

}

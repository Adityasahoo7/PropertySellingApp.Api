using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PropertySellingApp.DataAccess.Interfaces;
using PropertySellingApp.Models.Entities;

namespace PropertySellingApp.DataAccess.Repositories
{
  

    public class LoginCaptchaRepository : ILoginCaptchaRepository
    {
        private readonly AppDbContext _context;

        public LoginCaptchaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(LoginCaptcha captcha)
        {
            _context.LoginCaptchas.Add(captcha);
            await _context.SaveChangesAsync();
        }

        public async Task<LoginCaptcha?> GetValidCaptchaAsync(int userId, string code)
        {
            return await _context.LoginCaptchas.FirstOrDefaultAsync(c =>
                c.UserId == userId &&
                c.Code == code &&
                !c.IsUsed &&
                c.ExpiryTimeUtc > DateTime.UtcNow);
        }

        public async Task MarkAsUsedAsync(LoginCaptcha captcha)
        {
            captcha.IsUsed = true;
            _context.LoginCaptchas.Update(captcha);
            await _context.SaveChangesAsync();
        }
    }

}

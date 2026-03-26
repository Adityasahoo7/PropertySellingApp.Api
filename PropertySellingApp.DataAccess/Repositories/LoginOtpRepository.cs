using Microsoft.EntityFrameworkCore;
using PropertySellingApp.DataAccess.Interfaces;
using PropertySellingApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.DataAccess.Repositories
{
    public class LoginOtpRepository : ILoginOtpRepository
    {
        private readonly AppDbContext _context;

        public LoginOtpRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(LoginOtp otp)
        {
            _context.LoginOtps.Add(otp);
            await _context.SaveChangesAsync();
        }

        public async Task<LoginOtp?> GetValidOtpAsync(int userId, string otp)
        {
            return await _context.LoginOtps
                .FirstOrDefaultAsync(o =>
                    o.UserId == userId &&
                    o.Otp == otp &&
                    !o.IsUsed &&
                    o.ExpiryTimeUtc > DateTime.UtcNow);
        }

        public async Task MarkAsUsedAsync(LoginOtp otp)
        {
            otp.IsUsed = true;
            _context.LoginOtps.Update(otp);
            await _context.SaveChangesAsync();
        }

    }

}

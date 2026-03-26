using PropertySellingApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.DataAccess.Interfaces
{
    public interface ILoginOtpRepository
    {
        Task AddAsync(LoginOtp otp);
        Task<LoginOtp?> GetValidOtpAsync(int userId, string otp);

        Task MarkAsUsedAsync(LoginOtp otp);
    }

}

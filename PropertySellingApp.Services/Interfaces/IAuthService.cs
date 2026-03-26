using PropertySellingApp.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<TempAuthResponse> LoginAsync(LoginRequest request);

        Task<AuthResponse> VerifyOtpAsync(
        VerifyOtpRequest request,
        ClaimsPrincipal userClaims);
    }
}

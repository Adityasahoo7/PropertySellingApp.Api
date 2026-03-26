using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PropertySellingApp.DataAccess.Interfaces;
using PropertySellingApp.Models.DTOs;
using PropertySellingApp.Models.Entities;
using PropertySellingApp.Services.Interfaces;
using PropertySellingApp.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly PasswordHasher<User> _hasher;
        private readonly TokenService _tokenService;
        private  readonly ILoginOtpRepository _otpRepo;
        private readonly IEmailService _emailService;
        private readonly ILoginCaptchaRepository _captchaRepo;
        private readonly ICaptchaService _captchaService;



        public AuthService(IUserRepository users, TokenService tokenService ,ILoginOtpRepository otpRepo,
    ILoginCaptchaRepository captchaRepo,
    ICaptchaService captchaService,
    IEmailService emailService)
        {
            _users = users;
            _tokenService = tokenService;
            _otpRepo = otpRepo;
            _captchaRepo = captchaRepo;
            _captchaService = captchaService;
            _emailService = emailService;
            _hasher = new PasswordHasher<User>();
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var existing = await _users.GetByEmailAsync(request.Email);
            if (existing != null) throw new InvalidOperationException("Email already in use.");


            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                Role = request.Role,
            };
            user.PasswordHash = _hasher.HashPassword(user, request.Password);


            await _users.AddAsync(user);
            await _users.SaveChangesAsync();


            var (token, expires) = _tokenService.CreateToken(user);
            return new AuthResponse(user.Id, user.FullName, user.Email, user.Role, token, expires);
        }

        //public async Task<AuthResponse> LoginAsync(LoginRequest request)
        //{
        //    var user = await _users.GetByEmailAsync(request.Email) ?? throw new UnauthorizedAccessException("Invalid credentials");
        //    var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        //    if (result == PasswordVerificationResult.Failed) throw new UnauthorizedAccessException("Invalid credentials");


        //    var (token, expires) = _tokenService.CreateToken(user);
        //    return new AuthResponse(user.Id, user.FullName, user.Email, user.Role, token, expires);
        //}


        //UPDATED METHOD FOR LOGIN ADDING OTP AND CAPTCHA VERIFICATION

        public async Task<TempAuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _users.GetByEmailAsync(request.Email)
                ?? throw new UnauthorizedAccessException("Invalid credentials");

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Invalid credentials");

            // 🔢 Generate OTP
            var otp = new Random().Next(100000, 999999).ToString();

            await _otpRepo.AddAsync(new LoginOtp
            {
                UserId = user.Id,
                Otp = otp,
                ExpiryTimeUtc = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            });

            // 📧 Send OTP email
            await _emailService.SendOtpEmailAsync(user.Email, otp);

            // 🤖 Generate CAPTCHA
            var captchaCode = _captchaService.Generate();

            await _captchaRepo.AddAsync(new LoginCaptcha
            {
                UserId = user.Id,
                Code = captchaCode,
                ExpiryTimeUtc = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            });

            // 🔐 Generate TEMP token (short-lived)
            var (tempToken, _) = _tokenService.CreateToken(user);

            return new TempAuthResponse
            {
                OtpSent = true,
                TempToken = tempToken,
                CaptchaCode = captchaCode   // 👈 frontend will display this
            };
        }



        //ADD METHOD FOR VERIFY THE OTP 

        public async Task<AuthResponse> VerifyOtpAsync(
     VerifyOtpRequest request,
     ClaimsPrincipal userClaims)
        {
            var userIdClaim = userClaims.FindFirst("uid")
                ?? throw new UnauthorizedAccessException("Invalid or expired token");

            var userId = int.Parse(userIdClaim.Value);

            var captcha = await _captchaRepo.GetValidCaptchaAsync(userId, request.Captcha)
                ?? throw new UnauthorizedAccessException("Invalid or expired captcha");

            var otpEntity = await _otpRepo.GetValidOtpAsync(userId, request.Otp)
                ?? throw new UnauthorizedAccessException("Invalid or expired OTP");

            await _captchaRepo.MarkAsUsedAsync(captcha);
            await _otpRepo.MarkAsUsedAsync(otpEntity);

            var user = await _users.GetByIdAsync(userId)
                ?? throw new UnauthorizedAccessException("User not found");

            var (token, expires) = _tokenService.CreateToken(user);

            return new AuthResponse(
                user.Id,
                user.FullName,
                user.Email,
                user.Role,
                token,
                expires);
        }





    }
}

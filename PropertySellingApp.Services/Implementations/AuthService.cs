using Microsoft.AspNetCore.Identity;
using PropertySellingApp.DataAccess.Interfaces;
using PropertySellingApp.Models.DTOs;
using PropertySellingApp.Models.Entities;
using PropertySellingApp.Services.Interfaces;
using PropertySellingApp.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly PasswordHasher<User> _hasher;
        private readonly TokenService _tokenService;


        public AuthService(IUserRepository users, TokenService tokenService)
        {
            _users = users;
            _tokenService = tokenService;
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

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _users.GetByEmailAsync(request.Email) ?? throw new UnauthorizedAccessException("Invalid credentials");
            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed) throw new UnauthorizedAccessException("Invalid credentials");


            var (token, expires) = _tokenService.CreateToken(user);
            return new AuthResponse(user.Id, user.FullName, user.Email, user.Role, token, expires);
        }
    }
}

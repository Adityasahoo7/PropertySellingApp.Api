using System;
using System.ComponentModel.DataAnnotations;
using PropertySellingApp.Models.Entities;

namespace PropertySellingApp.Models.DTOs
{
    public class RegisterRequest
    {
        [Required, MaxLength(120)]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }

    public class LoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class AuthResponse
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAtUtc { get; set; }

        public AuthResponse(int userId, string fullName, string email, string role, string token, DateTime expiresAtUtc)
        {
            UserId = userId;
            FullName = fullName;
            Email = email;
            Role = role;
            Token = token;
            ExpiresAtUtc = expiresAtUtc;
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertySellingApp.Models.DTOs;
using PropertySellingApp.Services.Interfaces;

namespace PropertySellingApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        // ---------------- REGISTER ----------------
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            var res = await _auth.RegisterAsync(request);
            return Ok(res);
        }

        // ---------------- LOGIN (EMAIL + PASSWORD) ----------------
        // Step 1 → Validates user, sends OTP + CAPTCHA, returns TEMP token
        [HttpPost("login")]
        public async Task<ActionResult<TempAuthResponse>> Login([FromBody] LoginRequest request)
        {
            var res = await _auth.LoginAsync(request);
            return Ok(res);
        }

        // ---------------- VERIFY OTP + CAPTCHA ----------------
        // Step 2 → Uses TEMP token, verifies OTP & CAPTCHA, returns FINAL JWT
        [Authorize] // 🔐 Temp token required
        [HttpPost("verify-otp")]
        public async Task<ActionResult<AuthResponse>> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var res = await _auth.VerifyOtpAsync(request, User);
            return Ok(res);
        }
    }
}

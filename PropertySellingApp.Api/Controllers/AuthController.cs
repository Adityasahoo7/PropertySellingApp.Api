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
        public AuthController(IAuthService auth) { _auth = auth; }


        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            var res = await _auth.RegisterAsync(request);
            return Ok(res);
        }


        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            var res = await _auth.LoginAsync(request);
            return Ok(res);
        }
    }
}

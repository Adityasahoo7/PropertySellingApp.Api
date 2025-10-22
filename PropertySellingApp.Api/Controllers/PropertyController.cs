using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertySellingApp.Models.DTOs;
using PropertySellingApp.Services.Implementations;
using PropertySellingApp.Services.Interfaces;
using System.Security.Claims;

namespace PropertySellingApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyController : Controller
    {
        private readonly IPropertyService _svc;
        public PropertyController(IPropertyService svc) { _svc = svc; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PropertyResponse>>> GetAll()
        {
            return Ok(await _svc.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<PropertyResponse>> GetById(int id)
        {
            var item = await _svc.GetByIdAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpGet("seller/my")]
        [Authorize(Roles = "Seller,Admin")]  // 👈 string roles
        public async Task<ActionResult<IEnumerable<PropertyResponse>>> MyProperties()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            return Ok(await _svc.GetBySellerAsync(userId));
        }

        [HttpPost]
        [Authorize(Roles = "Seller,Admin")]  // 👈 string roles
        public async Task<ActionResult<int>> Create([FromBody] PropertyCreateRequest request)
        {
            int sellerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var id = await _svc.CreateAsync(sellerId, request);
            await _svc.CreateAsync(sellerId, request);
            await _svc.CreateAsync(sellerId, request);
            await _svc.CreateAsync(sellerId, request);
            await _svc.CreateAsync(sellerId, request);
            await _svc.CreateAsync(sellerId, request);
            return Ok(id);
        }

        // PropertiesController.cs
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest("Search term cannot be empty.");

            var results = await _svc.SearchAsync(q);
            return Ok(results);
        }


        [HttpPut("{id:int}")]
        [Authorize(Roles = "Seller,Admin")]  // 👈 string roles
        public async Task<IActionResult> Update(int id, [FromBody] PropertyUpdateRequest request)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            bool admin = User.IsInRole("Admin");  // 👈 string role
            var ok = await _svc.UpdateAsync(id, userId, request, admin);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Seller,Admin")]  // 👈 string roles
        public async Task<IActionResult> Delete(int id)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            bool admin = User.IsInRole("Admin");  // 👈 string role
            var ok = await _svc.DeleteAsync(id, userId, admin);
            return ok ? NoContent() : NotFound();
        }
    }
}

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
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
        private readonly TelemetryClient _telemetry;

        public PropertyController(IPropertyService svc, TelemetryClient telemetry)
        {
            _svc = svc;
            _telemetry = telemetry;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PropertyResponse>>> GetAll()
        {
            _telemetry.TrackEvent("Get All Propery data Api Called");
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
        //[AllowAnonymous]
        [Authorize(Roles = "Seller,Admin")]  // 👈 string roles
        public async Task<ActionResult<int>> Create([FromForm] PropertyCreateRequest request)
        {
          // var url=await _svc.UploadFileAsync(request.ImageFile);
          var url = await _svc.ConvertIFormFileToBase64Url(request.ImageFile);


            int sellerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            request.ImageUrl = url;
            var id = await _svc.CreateAsync(sellerId, request);
            return Ok(0);
        }

        // PropertiesController.cs
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            _telemetry.TrackEvent("Search Propery  Api Called");
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest("Search term cannot be empty.");

            var results = await _svc.SearchAsync(q);
            return Ok(results);
        }


        [HttpPut("{id:int}")]
        [Authorize(Roles = "Seller,Admin")]  // 👈 string roles
        public async Task<IActionResult> Update(int id, [FromBody] PropertyUpdateRequest request)
        {
            _telemetry.TrackEvent("Update this("+id+") property Data");
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            bool admin = User.IsInRole("Admin");  // 👈 string role
            var ok = await _svc.UpdateAsync(id, userId, request, admin);
            return ok ? NoContent() : NotFound();
        }


        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Seller,Admin")]  // 👈 string roles
        public async Task<IActionResult> Delete(int id)
        {
            _telemetry.TrackEvent("Delete this(" + id + ") property Data");
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            _telemetry.TrackEvent("Delete this(" + id + ") property Data by this("+userId+") user");
            bool admin = User.IsInRole("Admin");  // 👈 string role
            var ok = await _svc.DeleteAsync(id, userId, admin);
            return ok ? NoContent() : NotFound();
        }
    }
}

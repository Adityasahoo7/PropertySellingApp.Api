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
            _telemetry.TrackEvent("Get All Method Called");
            return Ok(await _svc.GetAllAsync());
        }

        [HttpPost]
        [Route("upload")]
        [AllowAnonymous]
        public async Task<ActionResult> UploadFileAsync(IFormFile file)
        {
           var url = await _svc.UploadFileAsync(file);


            return Ok(url);
        }

        [HttpGet]
        [Route("sasURL")]
        [AllowAnonymous]
        public async Task<ActionResult> GenerateSas(string fileName)
        {
            var sasUrl = _svc.GetSasUrl(fileName,2); 
            return Ok(new { SasUrl = sasUrl });
        }



        [HttpGet]
        [Route("download")]
        [AllowAnonymous]
        public async Task<ActionResult> DownloadAsync(string filename)
        {
            var stream = await _svc.GetFileAsync(filename);


            return File(stream, "application/octet-stream" , filename);
        }


        [HttpDelete]
        [Route("deletefile")]
        [AllowAnonymous]
        public async Task<ActionResult> DeleteAsync(string filename)
        {
            var returnValue = await _svc.DeleteFileAsync(filename);


            return Ok(returnValue);
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
           var url=await _svc.UploadFileAsync(request.ImageFile);
           int sellerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            request.ImageUrl = url;
            var id = await _svc.CreateAsync(sellerId, request);
            return Ok(0);
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

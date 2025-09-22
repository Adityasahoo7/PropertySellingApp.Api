using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertySellingApp.Models.DTOs;
using PropertySellingApp.Services.Interfaces;
using System.Security.Claims;

namespace PropertySellingApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VisitRequestController : Controller
    {
        private readonly IVisitRequestService _svc;
        public VisitRequestController(IVisitRequestService svc) { _svc = svc; }

        [HttpPost]
        [Authorize(Roles = "Buyer")]   // 👈 string role
        public async Task<ActionResult<int>> Create([FromBody] VisitRequestCreate request)
        {
            int buyerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var id = await _svc.CreateAsync(buyerId, request);
            return Ok(id);
        }

        [HttpGet("seller")]
        [Authorize(Roles = "Seller,Admin")]   // 👈 string roles
        public async Task<ActionResult<IEnumerable<VisitRequestResponse>>> ForSeller()
        {
            int sellerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            return Ok(await _svc.GetForSellerAsync(sellerId));
        }

        [HttpGet("buyer")]
        [Authorize(Roles = "Buyer")]   // 👈 string role
        public async Task<ActionResult<IEnumerable<VisitRequestResponse>>> ForBuyer()
        {
            int buyerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            return Ok(await _svc.GetForBuyerAsync(buyerId));
        }

        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = "Seller,Admin")]   // 👈 string roles
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] VisitRequestUpdateStatus request)
        {
            int sellerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var ok = await _svc.UpdateStatusAsync(id, sellerId, request);
            return ok ? NoContent() : NotFound();
        }
    }
}

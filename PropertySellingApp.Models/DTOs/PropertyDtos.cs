using PropertySellingApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.Models.DTOs
{
    public record PropertyCreateRequest(
        [Required, MaxLength(200)] string Title,
        [MaxLength(2000)] string? Description,
        [Required] string Type,
        [Range(0, double.MaxValue)] decimal Price,
        [Required, MaxLength(300)] string Location,
        [Range(0, 100)] int Bedrooms,
        [Range(0, 100)] int Bathrooms,
        [Range(0, double.MaxValue)] double AreaSqFt,
        string? ImageUrl // 👈 add this for property image
    );

    public record PropertyUpdateRequest(
        [Required, MaxLength(200)] string Title,
        [MaxLength(2000)] string? Description,
        [Required] string Type,
        [Range(0, double.MaxValue)] decimal Price,
        [Required, MaxLength(300)] string Location,
        [Range(0, 100)] int Bedrooms,
        [Range(0, 100)] int Bathrooms,
        [Range(0, double.MaxValue)] double AreaSqFt,
        string? ImageUrl // 👈 allow updating image also
    );

    public record PropertyResponse(
        int Id,
        string Title,
        string? Description,
        string Type,
        decimal Price,
        string Location,
        int Bedrooms,
        int Bathrooms,
        double AreaSqFt,
        int SellerId,
        string SellerName,
        string? ImageUrl, // 👈 return image to frontend
        DateTime CreatedAt
    );
}

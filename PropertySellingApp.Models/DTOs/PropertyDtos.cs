using PropertySellingApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace PropertySellingApp.Models.DTOs
{
    //public record PropertyCreateRequest(
    //    [Required, MaxLength(200)] string Title,
    //    [MaxLength(2000)] string? Description,
    //    [Required] string Type,
    //    [Range(0, double.MaxValue)] decimal Price,
    //    [Required, MaxLength(300)] string Location,
    //    [Range(0, 100)] int Bedrooms,
    //    [Range(0, 100)] int Bathrooms,
    //    [Range(0, double.MaxValue)] double AreaSqFt,
    //    string? ImageUrl // 👈 add this for property image

    //);

    public class FileUploadRequest
    {
        public string Title { get; set; }
        public IFormFile ImageFile { get; set; }
    }

    public class PropertyCreateRequest
    {
        //[ MaxLength(200)]
        public string? Title { get; set; }

        //[MaxLength(2000)]
        public string? Description { get; set; }

        public string? Type { get; set; }

        //[Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        //[Required, MaxLength(300)]
        public string? Location { get; set; }

        //[Range(0, 100)]
        public int Bedrooms { get; set; }

        //[Range(0, 100)]
        public int Bathrooms { get; set; }

        //[Range(0, double.MaxValue)]
        public double AreaSqFt { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? ImageFile { get; set; } // ✅ Add this for upload
    }



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

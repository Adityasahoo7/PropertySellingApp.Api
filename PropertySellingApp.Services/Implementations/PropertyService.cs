using PropertySellingApp.Models.DTOs;
using PropertySellingApp.Models.Entities;
using PropertySellingApp.Services.Interfaces;
using PropertySellingApp.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PropertySellingApp.Models.Extensions;
using Microsoft.Identity.Client.Extensions.Msal;
using Azure.Storage.Blobs;
using Azure.Core;
using System.Reflection.Metadata;

namespace PropertySellingApp.Services.Implementations
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _properties;
        private readonly IUserRepository _users;
        private readonly IAiService _aiService;
        private readonly ILogger<PropertyService> _logger;
        private readonly IStorage _storage;

        public PropertyService(IPropertyRepository properties, IUserRepository users, IAiService ai, ILogger<PropertyService> logger,IStorage storage)
        {
            _properties = properties;
            _users = users;
            _aiService = ai;
            _logger = logger;
            _storage = storage;
        }

        public async Task<IEnumerable<PropertyResponse>> GetAllAsync()
        {
            var list = await _properties.GetAllWithSellerAsync();
            return list.Select(p => MapToDto(p));
        }

        public async Task<IEnumerable<PropertyResponse>> GetBySellerAsync(int sellerId)
        {
            var list = await _properties.GetBySellerAsync(sellerId);
            return list.Select(p => MapToDto(p, includeSellerName: true));
        }

        public async Task<PropertyResponse?> GetByIdAsync(int id)
        {
            var entity = await _properties.GetByIdWithSellerAsync(id);
            return entity is null ? null : MapToDto(entity);
        }

        public async Task<int> CreateAsync(int sellerId, PropertyCreateRequest request)
        {
            var seller = await _users.GetByIdAsync(sellerId) ?? throw new InvalidOperationException("Seller not found");
            if (!seller.Role.Equals("Seller", StringComparison.OrdinalIgnoreCase) &&
                !seller.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Only sellers/admin can create properties");
            }

            var entity = new Property
            {
                Title = request.Title,
                Description = request.Description,
                Type = request.Type,
                Price = request.Price,
                Location = request.Location,
                Bedrooms = request.Bedrooms,
                Bathrooms = request.Bathrooms,
                AreaSqFt = request.AreaSqFt,
                SellerId = sellerId,
                ImageUrl = request.ImageUrl // ✅ store image path if provided
            };

            await _properties.AddAsync(entity);
            await _properties.SaveChangesAsync();
            return entity.Id;
        }

        //public async Task<int> CreateAsync(int sellerId, PropertyCreateRequest request)
        //{
        //    var seller = await _users.GetByIdAsync(sellerId)
        //        ?? throw new InvalidOperationException("Seller not found");

        //    if (!seller.Role.Equals("Seller", StringComparison.OrdinalIgnoreCase) &&
        //        !seller.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        //    {
        //        throw new UnauthorizedAccessException("Only sellers/admin can create properties");
        //    }

        //    // ✅ Upload image to Azure Blob Storage if file provided
        //    string imageUrl = null;
        //    if (request.ImageFile != null && request.ImageFile.Length > 0)
        //    {
        //        using (var stream = request.ImageFile.OpenReadStream())
        //        {
        //            imageUrl = await _storage.UploadAsync(stream, request.ImageFile.FileName, request.ImageFile.ContentType);
        //        } 
        //    }

        //    var entity = new Property
        //    {
        //        Title = request.Title,
        //        Description = request.Description,
        //        Type = request.Type,
        //        Price = request.Price,
        //        Location = request.Location,
        //        Bedrooms = request.Bedrooms,
        //        Bathrooms = request.Bathrooms,
        //        AreaSqFt = request.AreaSqFt,
        //        SellerId = sellerId,
        //        ImageUrl = imageUrl // ✅ store blob URL
        //    };

        //    await _properties.AddAsync(entity);
        //    await _properties.SaveChangesAsync();
        //    return entity.Id;
        //}





        public async Task<bool> UpdateAsync(int id, int sellerId, PropertyUpdateRequest request, bool adminOverride = false)
        {
            var entity = await _properties.GetByIdAsync(id);
            if (entity == null)
                return false;

            if (!adminOverride && entity.SellerId != sellerId)
                throw new UnauthorizedAccessException("Not your property");

            entity.Title = request.Title;
            entity.Description = request.Description;
            entity.Type = request.Type;
            entity.Price = request.Price;
            entity.Location = request.Location;
            entity.Bedrooms = request.Bedrooms;
            entity.Bathrooms = request.Bathrooms;
            entity.AreaSqFt = request.AreaSqFt;
            //
            entity.ImageUrl = request.ImageUrl ?? entity.ImageUrl; // ✅ update only if new provided

            _properties.Update(entity);
            await _properties.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int sellerId, bool adminOverride = false)
        {
            var entity = await _properties.GetByIdAsync(id);
            if (entity == null) return false;
            if (!adminOverride && entity.SellerId != sellerId)
                throw new UnauthorizedAccessException("Not your property");

            //if (!string.IsNullOrEmpty(entity.ImageUrl)) {

            //    var url = entity.ImageUrl;

            //    var filename = url.Split('/').Last();

            //    await _storage.DeleteAsync(filename);
            //}

             _properties.Remove(entity);
            await _properties.SaveChangesAsync();
            return true;
        }

        private async Task<string?> SaveImageAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;

            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "properties");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // ✅ return relative path for frontend
            return "/images/properties/" + fileName;
        }


        //// Method for SearchBar
        //public async Task<IEnumerable<PropertyResponse>> SearchAsync(string query)
        //{
        //    var properties = await _properties.SearchAsync(query);
        //    return properties.Select(p => MapToDto(p));
        //}
        //public async Task<IEnumerable<PropertyResponse>> SearchAsync(string query)
        //{
        //    var aiResult = await _ai.ParseSearchQueryAsync(query); // AI parsing

        //    IEnumerable<Property> properties;
        //    if (aiResult != null)
        //        properties = await _properties.SearchAsync(aiResult); // uses new DAL method
        //    else
        //        properties = await _properties.SearchAsync(new AiSearchResult { Keywords = query }); // fallback

        //    return properties.Select(p => MapToDto(p));

        //}

        public async Task<IEnumerable<PropertyResponse>> SearchAsync(string naturalQuery)
        {
            if (string.IsNullOrWhiteSpace(naturalQuery))
                return Enumerable.Empty<PropertyResponse>();

            // 1️⃣ Ask AI to parse
            var filter = await _aiService.ParseSearchQueryAsync(naturalQuery);

            IEnumerable<Property> properties;

            // 2️⃣ Fallback or AI-based search
            if (filter == null || filter.IsEmpty())
            {
                _logger.LogInformation("AI returned no structured filter — falling back to keyword search for '{q}'", naturalQuery);
                properties = await _properties.SearchAsync(new AiSearchResult { Keywords = naturalQuery });
            }
            else
            {
                properties = await _properties.SearchAsync(filter);
            }

            // 3️⃣ Map to DTO before returning
            return properties.Select(p => MapToDto(p));

        }

        //Method That Convert the Iform file to Base64 url

        public  async Task<string> ConvertIFormFileToBase64Url(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                byte[] fileBytes = memoryStream.ToArray();

                string base64String = Convert.ToBase64String(fileBytes);
                string contentType = file.ContentType; // image/webp, image/png, etc.

                return $"data:{contentType};base64,{base64String}";
            }
        }



        //Dummy method to test
        public async Task<string> UploadFileAsync(IFormFile file)
        {
            string url = null;

            using (var stream = file.OpenReadStream())
            {
                url = await _storage.UploadAsync(stream, file.FileName, file.ContentType);
            }
            return url;
        }

        public async Task<bool> DeleteFileAsync(string filename)
        {

            return await _storage.DeleteAsync(filename);
        }

        public async Task<Stream> GetFileAsync(string filename)
        {
            return await _storage.GetAsync(filename);
        }


        public string GetSasUrl(string filename, int time)
        {
            var url = _storage.GenerateSasUrl(filename, time);

            return url;
        }
        private static PropertyResponse MapToDto(Property p, bool includeSellerName = true)
        {
            return new PropertyResponse(
                p.Id,
                p.Title,
                p.Description,
                p.Type,
                p.Price,
                p.Location,
                p.Bedrooms,
                p.Bathrooms,
                p.AreaSqFt,
                p.SellerId,
                includeSellerName ? (p.Seller?.FullName ?? string.Empty) : string.Empty,
                p.ImageUrl, // ✅ include image in response
                p.CreatedAt
            );
        }

        
    }
}

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

namespace PropertySellingApp.Services.Implementations
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _properties;
        private readonly IUserRepository _users;

        public PropertyService(IPropertyRepository properties, IUserRepository users)
        {
            _properties = properties;
            _users = users;
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

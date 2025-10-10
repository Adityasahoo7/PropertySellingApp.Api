using PropertySellingApp.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.Services.Interfaces
{
    public interface IPropertyService
    {
        Task<IEnumerable<PropertyResponse>> GetAllAsync();
        Task<IEnumerable<PropertyResponse>> GetBySellerAsync(int sellerId);
        Task<PropertyResponse?> GetByIdAsync(int id);
        Task<int> CreateAsync(int sellerId, PropertyCreateRequest request);
        Task<bool> UpdateAsync(int id, int sellerId, PropertyUpdateRequest request, bool adminOverride = false);
        Task<bool> DeleteAsync(int id, int sellerId, bool adminOverride = false);

        // For Search bar
        // Task<IEnumerable<PropertyResponse>> SearchAsync(string query);
        // Task<IEnumerable<PropertyResponse>> SearchAsync(string query);

        Task<IEnumerable<PropertyResponse>> SearchAsync(string naturalQuery);
        // Task<List<PropertyResponse>> SearchAsync(AiSearchResult search);



    }
}

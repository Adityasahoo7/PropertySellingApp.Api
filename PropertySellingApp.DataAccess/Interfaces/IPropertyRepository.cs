using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertySellingApp.Models.Entities;

namespace PropertySellingApp.DataAccess.Interfaces
{
    public interface IPropertyRepository : IGenericRepository<Property>
    {
        Task<IEnumerable<Property>> GetAllWithSellerAsync();
        Task<Property?> GetByIdWithSellerAsync(int id);
        Task<IEnumerable<Property>> GetBySellerAsync(int sellerId);
    }
}

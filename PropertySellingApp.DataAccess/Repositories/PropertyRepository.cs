using Microsoft.EntityFrameworkCore;
using PropertySellingApp.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertySellingApp.Models.Entities;

namespace PropertySellingApp.DataAccess.Repositories
{
    public class PropertyRepository : GenericRepository<Property>, IPropertyRepository
    {
        public PropertyRepository(AppDbContext context) : base(context) { }


        public async Task<IEnumerable<Property>> GetAllWithSellerAsync()
        {
            return await _context.Properties.AsNoTracking().Include(p => p.Seller).OrderByDescending(p => p.CreatedAt).ToListAsync();
        }


        public async Task<Property?> GetByIdWithSellerAsync(int id)
        {
            return await _context.Properties.Include(p => p.Seller).FirstOrDefaultAsync(p => p.Id == id);
        }


        public async Task<IEnumerable<Property>> GetBySellerAsync(int sellerId)
        {
            return await _context.Properties.AsNoTracking().Where(p => p.SellerId == sellerId).ToListAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PropertySellingApp.DataAccess.Interfaces;
using PropertySellingApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.DataAccess.Repositories
{
    public class VisitRequestRepository : GenericRepository<VisitRequest>, IVisitRequestRepository
    {
        public VisitRequestRepository(AppDbContext context) : base(context) { }


        public async Task<IEnumerable<VisitRequest>> GetForSellerAsync(int sellerId)
        {
            return await _context.VisitRequests
            .AsNoTracking()
            .Include(v => v.Property)
            .ThenInclude(p => p!.Seller)
            .Include(v => v.Buyer)
            .Where(v => v.Property!.SellerId == sellerId)
            .OrderByDescending(v => v.RequestedOn)
            .ToListAsync();
        }

        public async Task<IEnumerable<VisitRequest>> GetForBuyerAsync(int buyerId)
        {
            return await _context.VisitRequests
            .AsNoTracking()
            .Include(v => v.Property)
            .Include(v => v.Buyer)
            .Where(v => v.BuyerId == buyerId)
            .OrderByDescending(v => v.RequestedOn)
            .ToListAsync();
        }


        public async Task<VisitRequest?> GetDetailedByIdAsync(int id)
        {
            return await _context.VisitRequests
            .Include(v => v.Property)!.ThenInclude(p => p!.Seller)
            .Include(v => v.Buyer)
            .FirstOrDefaultAsync(v => v.Id == id);
        }
    }
    }

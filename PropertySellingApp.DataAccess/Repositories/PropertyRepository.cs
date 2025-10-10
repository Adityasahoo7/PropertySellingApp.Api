using Microsoft.EntityFrameworkCore;
using PropertySellingApp.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertySellingApp.Models.Entities;
using PropertySellingApp.Models.DTOs;

namespace PropertySellingApp.DataAccess.Repositories
{
    public class PropertyRepository : GenericRepository<Property>, IPropertyRepository
    {
        public PropertyRepository(AppDbContext context) : base(context) { }


        public async Task<IEnumerable<Property>> GetAllWithSellerAsync()
        {
            return await _context.Properties.AsNoTracking().Include(p => p.Seller).OrderByDescending(p => p.CreatedAt).ToListAsync();
        }
        // PropertyRepository.cs
        //public async Task<IEnumerable<Property>> SearchAsync(string query)
        //{
        //    if (string.IsNullOrWhiteSpace(query))
        //        return new List<Property>();

        //    var q = query.ToLower();

        //    return await _context.Properties
        //        .Where(p =>
        //            p.Title.ToLower().Contains(q) ||
        //            p.Description.ToLower().Contains(q) ||
        //            p.Location.ToLower().Contains(q))
        //        .ToListAsync();
        //}

        //public async Task<IEnumerable<Property>> SearchAsync(AiSearchResult filter)
        //{
        //    var query = _context.Properties.AsQueryable();

        //    if (!string.IsNullOrWhiteSpace(filter.Keywords))
        //    {
        //        var q = filter.Keywords.ToLower();
        //        query = query.Where(p =>
        //            p.Title.ToLower().Contains(q) ||
        //            p.Description.ToLower().Contains(q));
        //    }

        //    if (!string.IsNullOrWhiteSpace(filter.Location))
        //        query = query.Where(p => p.Location.ToLower().Contains(filter.Location.ToLower()));

        //    if (filter.MinPrice.HasValue)
        //        query = query.Where(p => p.Price >= filter.MinPrice.Value);

        //    if (filter.MaxPrice.HasValue)
        //        query = query.Where(p => p.Price <= filter.MaxPrice.Value);

        //    if (filter.Bedrooms.HasValue)
        //        query = query.Where(p => p.Bedrooms == filter.Bedrooms.Value);

        //    return await query.ToListAsync();
        //}


        public async Task<IEnumerable<Property>> SearchAsync(AiSearchResult filter, string? fallbackKeywords = null)
        {
            var query = _context.Properties.AsQueryable();

            bool anyFilterApplied = false;

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.Keywords))
                {
                    var q = filter.Keywords.ToLower();
                    query = query.Where(p =>
                        p.Title.ToLower().Contains(q) ||
                        (p.Description != null && p.Description.ToLower().Contains(q)) ||
                        (p.Type != null && p.Type.ToLower().Contains(q)));
                    anyFilterApplied = true;
                }

                if (!string.IsNullOrWhiteSpace(filter.Location))
                {
                    var loc = filter.Location.ToLower();
                    query = query.Where(p => p.Location.ToLower().Contains(loc));
                    anyFilterApplied = true;
                }

                if (filter.MinPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= filter.MinPrice.Value);
                    anyFilterApplied = true;
                }

                if (filter.MaxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= filter.MaxPrice.Value);
                    anyFilterApplied = true;
                }

                if (filter.Bedrooms.HasValue)
                {
                    query = query.Where(p => p.Bedrooms == filter.Bedrooms.Value);
                    anyFilterApplied = true;
                }

                if (filter.Bathrooms.HasValue)
                {
                    // Ensure Property has Bathrooms column — if not, skip
                    query = query.Where(p => p.Bathrooms == filter.Bathrooms.Value);
                    anyFilterApplied = true;
                }

                if (filter.MinAreaSqFt.HasValue)
                {
                    query = query.Where(p => p.AreaSqFt >= filter.MinAreaSqFt.Value);
                    anyFilterApplied = true;
                }

                if (filter.MaxAreaSqFt.HasValue)
                {
                    query = query.Where(p => p.AreaSqFt <= filter.MaxAreaSqFt.Value);
                    anyFilterApplied = true;
                }
            }

            // If AI didn't give any usable filters, use a safe fallback text search if we have fallback keywords.
            if (!anyFilterApplied)
            {
                if (!string.IsNullOrWhiteSpace(fallbackKeywords))
                {
                    var q = fallbackKeywords.ToLower();
                    query = query.Where(p =>
                        p.Title.ToLower().Contains(q) ||
                        (p.Description != null && p.Description.ToLower().Contains(q)) ||
                        p.Location.ToLower().Contains(q) ||
                        (p.Type != null && p.Type.ToLower().Contains(q))
                    );
                }
                else
                {
                    // No filters + no fallback -> return empty (instead of all results)
                    return Enumerable.Empty<Property>();
                }
            }

            return await query.ToListAsync();
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

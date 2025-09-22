using PropertySellingApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.DataAccess.Interfaces
{
    public interface IVisitRequestRepository : IGenericRepository<VisitRequest>
    {
        Task<IEnumerable<VisitRequest>> GetForSellerAsync(int sellerId);
        Task<IEnumerable<VisitRequest>> GetForBuyerAsync(int buyerId);
        Task<VisitRequest?> GetDetailedByIdAsync(int id);
    }
}

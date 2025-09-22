using PropertySellingApp.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.Services.Interfaces
{
    public interface IVisitRequestService
    {
        Task<int> CreateAsync(int buyerId, VisitRequestCreate request);
        Task<IEnumerable<VisitRequestResponse>> GetForSellerAsync(int sellerId);
        Task<IEnumerable<VisitRequestResponse>> GetForBuyerAsync(int buyerId);
        Task<bool> UpdateStatusAsync(int requestId, int sellerId, VisitRequestUpdateStatus request);
    }
}

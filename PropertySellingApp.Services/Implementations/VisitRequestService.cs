using PropertySellingApp.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertySellingApp.DataAccess.Interfaces;
using PropertySellingApp.Models.DTOs;
using PropertySellingApp.Models.Entities;
using PropertySellingApp.Services.Interfaces;


namespace PropertySellingApp.Services.Implementations
{
    public class VisitRequestService : IVisitRequestService
    {
        private readonly IVisitRequestRepository _visits;
        private readonly IPropertyRepository _properties;


        public VisitRequestService(IVisitRequestRepository visits, IPropertyRepository properties)
        {
            _visits = visits;
            _properties = properties;
        }

        public async Task<int> CreateAsync(int buyerId, VisitRequestCreate request)
        {
            var prop = await _properties.GetByIdAsync(request.PropertyId) ?? throw new InvalidOperationException("Property not found");
            if (prop.SellerId == buyerId) throw new InvalidOperationException("Cannot request visit for your own property");


            var entity = new VisitRequest
            {
                BuyerId = buyerId,
                PropertyId = request.PropertyId,
                Message = request.Message
            };
            await _visits.AddAsync(entity);
            await _visits.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<IEnumerable<VisitRequestResponse>> GetForSellerAsync(int sellerId)
        {
            var list = await _visits.GetForSellerAsync(sellerId);
            return list.Select(v => new VisitRequestResponse(
            v.Id,
            v.PropertyId,
            v.Property!.Title,
            v.BuyerId,
            v.Buyer!.FullName,
            v.RequestedOn,
            v.Message,
            v.Status
            ));
        }



        public async Task<IEnumerable<VisitRequestResponse>> GetForBuyerAsync(int buyerId)
        {
            var list = await _visits.GetForBuyerAsync(buyerId);
            return list.Select(v => new VisitRequestResponse(
            v.Id,
            v.PropertyId,
            v.Property!.Title,
            v.BuyerId,
            v.Buyer!.FullName,
            v.RequestedOn,
            v.Message,
            v.Status
            ));
        }


        public async Task<bool> UpdateStatusAsync(int requestId, int sellerId, VisitRequestUpdateStatus request)
        {
            var entity = await _visits.GetDetailedByIdAsync(requestId);
            if (entity == null) return false;
            if (entity.Property!.SellerId != sellerId) throw new UnauthorizedAccessException("Not authorized");
            entity.Status = request.Status;
            _visits.Update(entity);
            await _visits.SaveChangesAsync();
            return true;
        }
    }
}

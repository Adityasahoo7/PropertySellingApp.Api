using PropertySellingApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.Models.DTOs
{
    public record VisitRequestCreate(
[Required] int PropertyId,
[MaxLength(500)] string? Message
);


    public record VisitRequestUpdateStatus(
    [Required] VisitStatus Status
    );


    public record VisitRequestResponse(
    int Id,
    int PropertyId,
    string PropertyTitle,
    int BuyerId,
    string BuyerName,
    DateTime RequestedOn,
    string? Message,
    VisitStatus Status
    );
}

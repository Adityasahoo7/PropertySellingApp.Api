using PropertySellingApp.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.Models.Extensions
{
    public static class AiSearchResultExtensions
    {
        public static bool IsEmpty(this AiSearchResult filter)
        {
            return string.IsNullOrWhiteSpace(filter.Keywords)
                && string.IsNullOrWhiteSpace(filter.Location)
                && !filter.MinPrice.HasValue
                && !filter.MaxPrice.HasValue
                && !filter.Bedrooms.HasValue
                && !filter.Bathrooms.HasValue;
        }
    }
}

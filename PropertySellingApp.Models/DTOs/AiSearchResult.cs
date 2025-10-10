using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.Models.DTOs
{
    public class AiSearchResult
    {
        public string Keywords { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public double? MinAreaSqFt { get; set; }
        public double? MaxAreaSqFt { get; set; }
    }

}


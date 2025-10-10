using PropertySellingApp.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.Services.Interfaces
{
    public interface IAiService
    {
        Task<AiSearchResult?> ParseSearchQueryAsync(string naturalQuery);
    }

}

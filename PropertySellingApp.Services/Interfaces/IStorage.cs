using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.Services.Interfaces
{
    public interface IStorage
    {
        Task<string> UploadAsync(Stream fileStream, string fileName, string contentType);
        Task<Stream> GetAsync(string fileName);
        Task<bool> DeleteAsync(string fileName);

        string GenerateSasUrl(string fileName, int validMinutes = 2);
    }

}

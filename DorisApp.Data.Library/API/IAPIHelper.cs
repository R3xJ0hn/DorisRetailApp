using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace DorisApp.Data.Library.API
{
    public interface IAPIHelper
    {
        HttpClient ApiCleint { get; }
        IConfiguration Config { get; }

        void Dispose();
        void LogInUser(string? tokenBearer);
        Task<string?> LogInUserAsync(string username, string password);
        void LogOffUser();
        Task<string?> RequestSecurityStamp(string? password);
    }
}
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace DorisApp.Data.Library.API
{
    public interface IAPIHelper
    {
        HttpClient ApiCleint { get; }
        IConfiguration Config { get; }

        void Dispose();
        void LogInUser(string tokenBearer);
        void LogOffUser();
    }
}
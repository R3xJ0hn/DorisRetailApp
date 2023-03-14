using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DorisApp.Data.Library.API
{
    public class CategoryEndpoint
    {
        private readonly IAPIHelper _apiHelper;

        public CategoryEndpoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task AddCategoryAsync(string categoryName)
        {
            var json = JsonConvert.SerializeObject(new {categoryName});
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await _apiHelper.ApiCleint.PostAsync(_apiHelper.Config["URL:add-category"], data);
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

    }

}

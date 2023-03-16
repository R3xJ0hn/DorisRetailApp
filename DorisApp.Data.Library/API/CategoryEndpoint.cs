using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            var json = JsonConvert.SerializeObject(new { categoryName });
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await _apiHelper.ApiCleint.PostAsync(_apiHelper.Config["URL:add-category"], data);
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<RequestModel<CategoryTableDTO>?> GetTableCategories(RequestPageDTO request)
        {
            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await _apiHelper.ApiCleint.PostAsync(_apiHelper.Config["URL:get-category/table"], data);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<RequestModel<CategoryTableDTO>>(result);
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<string> UpdateCategory(CategoryModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await _apiHelper.ApiCleint.PostAsync(_apiHelper.Config["URL:update-category"], data);
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }



    }

}

using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DorisApp.Data.Library.API
{

    public class CategoryEndpoint:BaseEndpoint
    {
        public CategoryEndpoint(IAPIHelper apiHelper) 
            : base(apiHelper)
        {
        }

        public async Task AddCategoryAsync(string categoryName)
        {
            var category = new CategoryModel() { CategoryName = categoryName };
            await SendPostAysnc(category, "URL:add-category");
        }

        public async Task<RequestModel<CategorySummaryDTO>?> GetCategorySummary(RequestPageDTO request)
        {
            var result = await SendPostAysnc(request, "URL:get-category/summary");
            return JsonConvert.DeserializeObject<RequestModel<CategorySummaryDTO>>(result);
        }

        public async Task<string> UpdateCategory(CategoryModel model)
        {
           return await SendPostAysnc(model, "URL:update-category");
        }

        public async Task<string> DeleteCategory(CategoryModel model)
        {
            return await SendPostAysnc(model, "URL:delete-category");
        }

    }

}

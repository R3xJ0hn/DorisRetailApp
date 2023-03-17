using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DorisApp.Data.Library.API
{
    public class SubCategoryEndpoint : BaseEndpoint
    {
        public SubCategoryEndpoint(IAPIHelper apiHelper)
            : base(apiHelper)
        {
        }

        public async Task AddSubCategoryAsync(string subCategoryName, int categoryId)
        {
            //TODO: Here Na You
            var category = new SubCategoryModel() { SubCategoryName = subCategoryName };
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

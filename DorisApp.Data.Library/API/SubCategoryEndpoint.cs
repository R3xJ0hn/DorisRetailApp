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
            var subCategory = new SubCategoryModel() 
            { 
                SubCategoryName = subCategoryName,
                CategoryId= categoryId
            };

            await SendPostAysnc(subCategory, "URL:add-subcategory");
        }

        public async Task<RequestModel<SubCategorySummaryDTO>?> GetSubCategorySummary(RequestPageDTO request)
        {
            var result = await SendPostAysnc(request, "URL:get-subcategory/summary");
            return JsonConvert.DeserializeObject<RequestModel<SubCategorySummaryDTO>>(result);
        }

        public async Task<string> UpdateCategory(SubCategoryModel model)
        {
            return await SendPostAysnc(model, "URL:update-subcategory");
        }

        public async Task<string> DeleteCategory(SubCategoryModel model)
        {
            return await SendPostAysnc(model, "URL:delete-subcategory");
        }

    }

}

using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace DorisApp.Data.Library.API
{
    public class CategoryEndpoint : BaseEndpoint
    {
        public CategoryEndpoint(IAPIHelper apiHelper)
            : base(apiHelper)
        {
        }

        public async Task<int> CountCategoryItems()
        {
            var request = new RequestPageDTO
            {
                PageNo = 1,
                ItemPerPage = 1
            };

            var result = await GetCategorySummary(request);
            return result != null ? result.TotalItems : 0;
        }

        public async Task AddCategoryAsync(CategoryModel category)
        {
            ValidateCategory(category);
            await SendPostAysnc(category, "URL:add-category");
        }

        public async Task<string> UpdateCategory(CategoryModel category)
        {
            ValidateCategory(category);
            return await SendPostAysnc(category, "URL:update-category");
        }

        public async Task<RequestModel<CategorySummaryDTO>?> GetCategorySummary(RequestPageDTO request)
        {
            var result = await SendPostAysnc(request, "URL:get-category/summary");
            return JsonConvert.DeserializeObject<RequestModel<CategorySummaryDTO>>(result);
        }

        public async Task<string> DeleteCategory(CategoryModel model)
        {
            return await SendPostAysnc(model, "URL:delete-category");
        }

        private void ValidateCategory(CategoryModel category)
        {
            if (string.IsNullOrWhiteSpace(category.CategoryName))
                throw new NullReferenceException("Category name is null.");
        }

    }

}

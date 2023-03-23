using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DorisApp.Data.Library.API
{
    public class SubCategoryEndpoint : BaseEndpoint
    {
        public SubCategoryEndpoint(IAPIHelper apiHelper)
            : base(apiHelper)
        {
        }

        public async Task<int> CountSubCategoryItems()
        {
            var request = new RequestPageDTO
            {
                PageNo = 1,
                ItemPerPage = 1
            };

            var result = await GetSubCategorySummary(request);
            return result != null ? result.TotalItems : 0;
        }

        public async Task AddSubCategoryAsync(SubCategoryModel subCategory)
        {
            await ValidateSubCategory(subCategory);
            await SendPostAysnc(subCategory, "URL:add-subcategory");
        }
        public async Task<string> UpdateCategory(SubCategoryModel subCategory)
        {
            await ValidateSubCategory(subCategory);
            return await SendPostAysnc(subCategory, "URL:update-subcategory");
        }

        public async Task<RequestModel<SubCategorySummaryDTO>?> GetSubCategorySummary(RequestPageDTO request)
        {
            var result = await SendPostAysnc(request, "URL:get-subcategory/summary");
            return JsonConvert.DeserializeObject<RequestModel<SubCategorySummaryDTO>>(result);
        }

        public async Task<List<SubCategoryModel>?> GetSubCategorySummaryByCategoryId(int id)
        {
            var result = await SendPostByIdAysnc(id, "URL:get-subcategory/bycategory");
            return JsonConvert.DeserializeObject<List<SubCategoryModel>>(result);
        }

        public async Task<string> DeleteCategory(SubCategoryModel model)
        {
            return await SendPostAysnc(model, "URL:delete-subcategory");
        }

        private async Task ValidateSubCategory(SubCategoryModel subCategory)
        {
            if (string.IsNullOrWhiteSpace(subCategory.SubCategoryName))
                throw new NullReferenceException("Sub Category name is null.");

            var getIfExist = await SendPostByIdAysnc(subCategory.CategoryId, "URL:get-category/id");

            if (string.IsNullOrEmpty(getIfExist))
            {
                throw new NullReferenceException("Category not exist.");
            }
        }

    }

}

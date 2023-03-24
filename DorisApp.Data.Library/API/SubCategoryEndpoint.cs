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

            var sent = await GetSubCategorySummary(request);

            var newRequest = new RequestPageDTO
            {
                PageNo = 1,
                ItemPerPage = sent.Data.TotalItems 
            };

            var result = await GetSubCategorySummary(newRequest);

            return result?.Data.Models.Count ?? 0;
        }

        public async Task<ResultDTO<List<SubCategorySummaryDTO>>?> AddSubCategoryAsync(SubCategoryModel subCategory)
        {
            await ValidateSubCategory(subCategory);
            var summary = await SendPostAysnc(subCategory, "URL:add-subcategory");
            return JsonConvert.DeserializeObject<ResultDTO<List<SubCategorySummaryDTO>>>(summary);
        }

        public async Task<ResultDTO<List<SubCategorySummaryDTO>>?> UpdateCategory(SubCategoryModel subCategory)
        {
            await ValidateSubCategory(subCategory);
            var summary = await SendPostAysnc(subCategory, "URL:update-subcategory");
            return JsonConvert.DeserializeObject<ResultDTO<List<SubCategorySummaryDTO>>>(summary);
        }

        public async Task<ResultDTO<RequestModel<SubCategorySummaryDTO>>?> GetSubCategorySummary(RequestPageDTO request)
        {
            var result = await SendPostAysnc(request, "URL:get-subcategory/summary");
            return JsonConvert.DeserializeObject<ResultDTO<RequestModel<SubCategorySummaryDTO>>>(result);
        }

        public async Task<ResultDTO<List<SubCategorySummaryDTO>>?> GetSubCategorySummaryByCategoryId(int id)
        {
            var result = await SendPostByIdAysnc(id, "URL:get-subcategory/bycategory");
            return JsonConvert.DeserializeObject<ResultDTO<List<SubCategorySummaryDTO>>>(result);
        }

        public async Task<ResultDTO<SubCategorySummaryDTO>?> DeleteCategory(SubCategoryModel model)
        {
            var result = await SendPostAysnc(model, "URL:delete-subcategory");
            return JsonConvert.DeserializeObject<ResultDTO<SubCategorySummaryDTO>>(result);
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

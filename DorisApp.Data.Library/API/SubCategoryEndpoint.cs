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
            var countedResult = 0;

            var request = new RequestPageDTO
            {
                PageNo = 1,
                ItemPerPage = 1
            };

            var sent = await GetSubCategorySummary(request);

            if (sent?.Data != null)
            {
                var newRequest = new RequestPageDTO
                {
                    PageNo = 1,
                    ItemPerPage = sent.Data.TotalItems
                };

                var getNewRequest = await GetSubCategorySummary(newRequest);
                countedResult = getNewRequest?.Data.Models.Count ?? 0;
            }

            return countedResult;
        }

        public async Task<ResultDTO<List<SubCategorySummaryDTO>>?> AddSubCategoryAsync(SubCategoryModel subCategory)
        {
            await ValidateSubCategory(subCategory);
            var summary = await SendPostAysnc(subCategory, "URL:add-subcategory");

            try
            {
                return JsonConvert.DeserializeObject<ResultDTO<List
                    <SubCategorySummaryDTO>>>(summary);
            }
            catch
            {
                return new ResultDTO<List<SubCategorySummaryDTO>>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = summary
                };
            }

        }

        public async Task<ResultDTO<List<SubCategorySummaryDTO>>?> UpdateCategory(SubCategoryModel subCategory)
        {
            await ValidateSubCategory(subCategory);
            var summary = await SendPostAysnc(subCategory, "URL:update-subcategory");
            try
            {
                return JsonConvert.DeserializeObject<ResultDTO<List
                    <SubCategorySummaryDTO>>>(summary);
            }
            catch
            {
                return new ResultDTO<List<SubCategorySummaryDTO>>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = summary
                };
            }
        }

        public async Task<ResultDTO<RequestModel<SubCategorySummaryDTO>>?> GetSubCategorySummary(RequestPageDTO request)
        {
            var result = await SendPostAysnc(request, "URL:get-subcategory/summary");

            try
            {
                return JsonConvert.DeserializeObject<ResultDTO
                    <RequestModel<SubCategorySummaryDTO>>>(result);
            }
            catch
            {
                return new ResultDTO<RequestModel<SubCategorySummaryDTO>>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = result
                };
            }

        }

        public async Task<ResultDTO<List<SubCategorySummaryDTO>>?> GetSubCategorySummaryByCategoryId(int id)
        {
            var result = await SendPostByIdAysnc(id, "URL:get-subcategory/bycategory");

            try
            {
                return JsonConvert.DeserializeObject<ResultDTO
                    <List<SubCategorySummaryDTO>>>(result);
            }
            catch
            {
                return new ResultDTO<List<SubCategorySummaryDTO>>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = result
                };
            }

        }

        public async Task<ResultDTO<SubCategorySummaryDTO>?> DeleteCategory(SubCategoryModel model)
        {
            var result = await SendPostAysnc(model, "URL:delete-subcategory");

            try
            {
                return JsonConvert.DeserializeObject<ResultDTO
                    <SubCategorySummaryDTO>>(result);
            }
            catch
            {
                return new ResultDTO<SubCategorySummaryDTO>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = result
                };
            }
        }

        public async Task<ResultDTO<SubCategoryModel>?> GetById(int Id)
        {
            var result = await SendPostByIdAysnc(Id, "URL:get-subcategory/id");

            try
            {
                return JsonConvert.DeserializeObject<ResultDTO
                    <SubCategoryModel>>(result);
            }
            catch
            {
                return new ResultDTO<SubCategoryModel>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = result
                };
            }
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

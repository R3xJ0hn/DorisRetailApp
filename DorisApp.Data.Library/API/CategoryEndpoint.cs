using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            var countedResult = 0;

            var request = new RequestPageDTO
            {
                PageNo = 1,
                ItemPerPage = 1
            };

            var sent = await GetCategorySummary(request);

            if (sent?.Data != null)
            {
                var newRequest = new RequestPageDTO
                {
                    PageNo = 1,
                    ItemPerPage = sent.Data.TotalItems
                };

                var getNewRequest = await GetCategorySummary(newRequest);
                countedResult = getNewRequest?.Data.Models.Count ?? 0;
            }

            return countedResult;
        }

        public async Task<ResultDTO<List<CategorySummaryDTO>>?> AddCategoryAsync(CategoryModel category)
        {
            ValidateCategory(category);
            var summary = await SendPostAysnc(category, "URL:add-category");

            try
            {
                return JsonConvert.DeserializeObject<ResultDTO
                    <List<CategorySummaryDTO>>>(summary);
            }
            catch
            {
                return new ResultDTO<List<CategorySummaryDTO>>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = summary
                };
            }

        }

        public async Task<ResultDTO<List<CategorySummaryDTO>>?> UpdateCategory(CategoryModel category)
        {
            ValidateCategory(category);
            var summary = await SendPostAysnc(category, "URL:update-category");

            try
            {
                return JsonConvert.DeserializeObject<ResultDTO
                    <List<CategorySummaryDTO>>>(summary);
            }
            catch
            {
                return new ResultDTO<List<CategorySummaryDTO>>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = summary
                };
            }
        }

        public async Task<ResultDTO<RequestModel<CategorySummaryDTO>>?> GetCategorySummary(RequestPageDTO request)
        {
            var result = await SendPostAysnc(request, "URL:get-category/summary");
            try
            {
                return JsonConvert.DeserializeObject<ResultDTO
                    <RequestModel<CategorySummaryDTO>>>(result);
            }
            catch
            {
                return new ResultDTO<RequestModel<CategorySummaryDTO>>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = result
                };
            }

        }

        public async Task<ResultDTO<CategorySummaryDTO>?> DeleteCategory(CategoryModel model)
        {
            var result = await SendPostAysnc(model, "URL:delete-category");
            try
            {
                return JsonConvert.DeserializeObject<ResultDTO
                    <CategorySummaryDTO>>(result);
            }
            catch
            {
                return new ResultDTO<CategorySummaryDTO>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = result
                };
            }
        }

        public async Task<ResultDTO<CategoryModel>?> GetById(int Id)
        {
            var result = await SendPostByIdAysnc(Id, "URL:get-category/id");

            try
            {
                return JsonConvert.DeserializeObject<ResultDTO
                    <CategoryModel>>(result);
            }
            catch
            {
                return new ResultDTO<CategoryModel>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = result
                };
            }
        }

        private void ValidateCategory(CategoryModel category)
        {
            if (string.IsNullOrWhiteSpace(category.CategoryName))
                throw new NullReferenceException("Category name is null.");
        }

    }

}

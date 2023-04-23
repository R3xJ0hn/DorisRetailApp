using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DorisApp.Data.Library.API
{
    public class ProductEndpoint : BaseEndpoint
    {
        public ProductEndpoint(IAPIHelper apiHelper)
            : base(apiHelper)
        {
        }

        public async Task<int> CountProductItems()
        {
            var countedResult = 0;

            var request = new RequestPageDTO
            {
                PageNo = 1,
                ItemPerPage = 1
            };

            var sent = await GetProductSummaryAsync(request);

            if (sent?.Data != null)
            {
                var newRequest = new RequestPageDTO
                {
                    PageNo = 1,
                    ItemPerPage = sent.Data.TotalItems
                };

                var getNewRequest = await GetProductSummaryAsync(newRequest);
                countedResult = getNewRequest?.Data.Models.Count ?? 0;
            }

            return countedResult;
        }


        public async Task<ResultDTO<ProductModel>?> AddProductAsync(ProductModel product, Stream? imgStream, string fileName)
        {
            await ValidateProduct(product);

            if (imgStream != null && imgStream.Length != 0)
            {
                var result = await SendImgAsync(imgStream, fileName);

                if (result != null)
                {
                    product.StoredImageName = result.StoredFileName;
                }
            }

            var summary = await SendPostAysnc(product, "URL:add-product");

            try
            {
                return JsonConvert.DeserializeObject<ResultDTO<ProductModel>>(summary);
            }
            catch
            {
                return new ResultDTO<ProductModel>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = summary
                };
            }
        }

        public async Task<ResultDTO<List<ProductSummaryDTO>>?> UpdateProduct(ProductModel product, Stream? imgStream, string? fileName)
        {
            await ValidateProduct(product);

            if (imgStream != null && imgStream.Length != 0)
            {
                var result = await SendImgAsync(imgStream, fileName);

                if (result != null)
                {
                    product.StoredImageName = result.StoredFileName;
                }
            }

            var summary = await SendPostAysnc(product, "URL:update-product");

            try
            {
                return JsonConvert.DeserializeObject<ResultDTO
                    <List<ProductSummaryDTO>>>(summary);
            }
            catch
            {
                return new ResultDTO<List<ProductSummaryDTO>>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = summary
                };
            }
        }

        public async Task<ResultDTO<RequestModel<ProductSummaryDTO>>?> GetProductSummaryAsync(RequestPageDTO request)
        {
            var result = await SendPostAysnc(request, "URL:get-product/summary");

            try
            {
                return JsonConvert.DeserializeObject<ResultDTO
                    <RequestModel<ProductSummaryDTO>>>(result);
            }
            catch
            {
                return new ResultDTO<RequestModel<ProductSummaryDTO>>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = result
                };
            }
        }

        public async Task<ResultDTO<ProductSummaryDTO>?> DeleteProduct(ProductModel model)
        {
            var result = await SendPostAysnc(model, "URL:delete-product");

            try
            {
                return JsonConvert.DeserializeObject<ResultDTO
                    <ProductSummaryDTO>>(result);
            }
            catch
            {
                return new ResultDTO<ProductSummaryDTO>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = result
                };
            }

        }

        public async Task<ResultDTO<ProductModel>?> GetById(int Id)
        {
            var result = await SendPostByIdAysnc(Id, "URL:get-product/id");

            try
            {
                return JsonConvert.DeserializeObject<ResultDTO
                    <ProductModel>>(result);
            }
            catch
            {
                return new ResultDTO<ProductModel>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = result
                };
            }
        }

        private async Task ValidateProduct(ProductModel product)
        {
            if (string.IsNullOrWhiteSpace(product.ProductName))
            {
                throw new NullReferenceException("Product name is null.");
            }

            if (string.IsNullOrWhiteSpace(product.Sku))
            {
                throw new NullReferenceException("Product sku is null.");
            }

            if (string.IsNullOrWhiteSpace(product.Size))
            {
                throw new NullReferenceException("Product size is null.");
            }

            var getIfExist = await SendPostByIdAysnc(product.CategoryId, "URL:get-category/id");

            if (string.IsNullOrEmpty(getIfExist))
            {
                throw new NullReferenceException("Product category not exist.");
            }
        }

    }

}

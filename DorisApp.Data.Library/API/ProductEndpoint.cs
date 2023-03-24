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
        public ProductEndpoint(IAPIHelper apiHelper, CategoryEndpoint categoryEndpoint)
            : base(apiHelper)
        {
        }

        public async Task<int> CountProductItems()
        {
            var request = new RequestPageDTO
            {
                PageNo = 1,
                ItemPerPage = 1
            };

            var sent = await GetProductSummary(request);

            var newRequest = new RequestPageDTO
            {
                PageNo = 1,
                ItemPerPage = sent.Data.TotalItems
            };

            var result = await GetProductSummary(newRequest);

            return result?.Data.Models.Count ?? 0;
        }

        public async Task<ResultDTO<List<ProductSummaryDTO>>?> AddProductAsync(ProductModel product, Stream? imgStream, string fileName)
        {
           await ValidateProduct(product);

            if (imgStream != null && imgStream.Length != 0)
            {
                var result = await SendImg(imgStream, fileName);

                if (result != null)
                {
                    product.StoredImageName = result.StoredFileName;
                }
            }

            var summary  = await SendPostAysnc(product, "URL:add-product");
            return JsonConvert.DeserializeObject<ResultDTO<List<ProductSummaryDTO>>>(summary);
        }

        public async Task<ResultDTO<List<ProductSummaryDTO>>?> UpdateProduct(ProductModel product, Stream? imgStream, string fileName)
        {
            await ValidateProduct(product);

            if (imgStream != null && imgStream.Length != 0)
            {
                var result = await SendImg(imgStream, fileName);

                if (result != null)
                {
                    product.StoredImageName = result.StoredFileName;
                }
            }

            var summary = await SendPostAysnc(product, "URL:update-product");
            return JsonConvert.DeserializeObject<ResultDTO<List<ProductSummaryDTO>>>(summary);
        }

        public async Task<ResultDTO<RequestModel<ProductSummaryDTO>>?> GetProductSummary(RequestPageDTO request)
        {
            var result = await SendPostAysnc(request, "URL:get-product/summary");
            return JsonConvert.DeserializeObject<ResultDTO<RequestModel<ProductSummaryDTO>>>(result);
        }

        public async Task<ResultDTO<ProductSummaryDTO>?> DeleteProduct(ProductModel model)
        {
            var result = await SendPostAysnc(model, "URL:delete-product");
            return JsonConvert.DeserializeObject<ResultDTO<ProductSummaryDTO>>(result);
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

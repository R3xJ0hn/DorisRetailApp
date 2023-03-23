using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using Newtonsoft.Json;
using System;
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

            var result = await GetProductSummary(request);
            return result?.TotalItems ?? 0;
        }

        public async Task AddProductAsync(ProductModel product, Stream? imgStream, string fileName)
        {
           await ValidateProduct(product);

            var result = await SendImg(imgStream, fileName);

            if (result != null)
            {
                product.StoredImageName = result.StoredFileName;
            }

            await SendPostAysnc(product, "URL:add-product");
        }

        public async Task<string> UpdateProduct(ProductModel product, Stream? imgStream, string fileName)
        {
            await ValidateProduct(product);

            var result = await SendImg(imgStream, fileName);

            product.StoredImageName = result?.StoredFileName;

            return await SendPostAysnc(product, "URL:update-product");
        }

        public async Task<RequestModel<ProductSummaryDTO>?> GetProductSummary(RequestPageDTO request)
        {
            var result = await SendPostAysnc(request, "URL:get-product/summary");
            return JsonConvert.DeserializeObject<RequestModel<ProductSummaryDTO>>(result);
        }

        public async Task<string> DeleteProduct(ProductModel model)
        {
            return await SendPostAysnc(model, "URL:delete-product");
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

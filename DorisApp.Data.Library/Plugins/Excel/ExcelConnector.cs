using DorisApp.Data.Library.Model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DorisApp.Data.Library.Plugins.Excel
{
    public class ExcelConnector
    {

        public ExcelConnector()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }


        public async Task<List<ExtractedProductModel>> RetrieveProductsAsync(Stream stream, Func<string, int, Task> statusUpdate)
        {
            await statusUpdate!.Invoke($"Preparing Excel File... ", 0);
            using var package = new ExcelPackage(stream);

            await statusUpdate!.Invoke($"Preparing Worksheet... ", 0);
            var ws = package.Workbook.Worksheets[0];

            int startCol = (string.IsNullOrEmpty(ws.Cells[1, 1].Value?.ToString()) &&
                      string.IsNullOrEmpty(ws.Cells[1, 2].Value?.ToString())) ? 2 : 1;

            int startRow = FindRowWithMaxNonEmptyCells(ws);
            int totalTasks = ws.Dimension.Rows - (startRow + 1);
            int processedItems = 0;
            int progressPercentage = 0;
            var headers = new Dictionary<string, int>
            {
                { "ProductName", -1 },
                { "Image", -1 },
                { "Sku",  -1 },
                { "Brand", -1 },
                { "Category", -1 },
                { "SubCategory", -1 },
                { "Size", -1 },
                { "Color", -1 },
                { "Description",  -1 }
            };

            totalTasks += headers.Count();

            for (var col = startCol; col <= ws.Dimension.Columns; col++)
            {
                var cellValue = ws.Cells[startRow, col].Value;
                if (cellValue != null)
                {
                    var normalizedCellValue = cellValue.ToString().ToLower().Replace(" ", string.Empty);
                    foreach (var header in headers)
                    {
                        if (normalizedCellValue.Contains(header.Key.ToLower()))
                        {
                            if (header.Key == "Category")
                            {
                                if (normalizedCellValue == "category")
                                {
                                    headers["Category"] = col;
                                }
                            }
                            else if (header.Key == "SubCategory")
                            {
                                if (normalizedCellValue == "subcategory")
                                {
                                    headers["SubCategory"] = col;
                                }
                            }
                            else
                            {
                                headers[header.Key] = col;
                            }
                        }
                    }
                }
            }


            foreach (var header in headers)
            {
                if (header.Value == -1)
                {
                    await statusUpdate!.Invoke($"Error: The '{header.Key}' header not found!", 0);
                    return new List<ExtractedProductModel>();
                }
                else
                {
                    processedItems++;
                    progressPercentage = (processedItems * 33) / totalTasks;
                    await statusUpdate!.Invoke($"Success: The '{header.Key}' header verified!", progressPercentage);
                }
            }

            var setId = 0;
            var products = new List<ExtractedProductModel>();

            for (var row = startRow + 1; row <= ws.Dimension.Rows; row++)
            {
                processedItems++;
                progressPercentage = (processedItems * 33) / totalTasks;

                var extractedProduct = new ExtractedProductModel
                {
                    ProductName = ws.Cells[row, headers["ProductName"]].Value?.ToString()!,
                    Image = ws.Cells[row, headers["Image"]].Value?.ToString()!,
                    Sku = ws.Cells[row, headers["Sku"]].Value?.ToString()!,
                    BrandName = ws.Cells[row, headers["Brand"]].Value?.ToString()!,
                    Category = ws.Cells[row, headers["Category"]].Value?.ToString()!,
                    SubCategory = ws.Cells[row, headers["SubCategory"]].Value?.ToString()!,
                    Size = ws.Cells[row, headers["Size"]].Value?.ToString()!,
                    Color = ws.Cells[row, headers["Color"]].Value?.ToString()!,
                    Description = ws.Cells[row, headers["Description"]].Value?.ToString()!,
                    Id = ++setId
                };

                if (!string.IsNullOrEmpty(extractedProduct.ProductName))
                {
                    products.Add(extractedProduct);

                    await Task.Run(async () =>
                    {
                        await statusUpdate!.Invoke($"Product Extracted [{processedItems}]: {extractedProduct.ProductName}", progressPercentage);
                    });
                }

            }

            statusUpdate?.Invoke("------------------------End------------------------", progressPercentage);
            return products;
        }

        public async Task<List<BrandModel>> RetrieveBrandsAsync(Stream stream, Func<string, int, Task> statusUpdate)
        {
            List<BrandModel> brands = new List<BrandModel>();

            using var package = new ExcelPackage(stream);
            var ws = package.Workbook.Worksheets[0];

            int startCol = (string.IsNullOrEmpty(ws.Cells[1, 1].Value?.ToString()) &&
                            string.IsNullOrEmpty(ws.Cells[1, 2].Value?.ToString())) ? 2 : 1;

            int startRow = FindRowWithMaxNonEmptyCells(ws);
            int totalTask = ws.Dimension.Rows - startRow;
            int processedItem = 0;
            int progressPercentage = 0;
            var setId = 0;

            // Verify if sheet have Brand Name and Image header
            int brandCol = ws.Cells[startRow, startCol, startRow, ws.Dimension.Columns + 1]
                .FirstOrDefault(cell => cell.Value?.ToString().ToLower().Replace(" ", string.Empty).Contains("brandname") == true)?.Start.Column ?? -1;

            int imgCol = ws.Cells[startRow, startCol, startRow, ws.Dimension.Columns + 1]
                .FirstOrDefault(cell => cell.Value?.ToString().ToLower().Replace(" ", string.Empty).Contains("image") == true)?.Start.Column ?? -1;

            if (brandCol != -1 && imgCol != -1)
            {
                for (int row = startRow + 1; row <= ws.Dimension.Rows; row++)
                {
                    processedItem++;
                    progressPercentage = (processedItem * 100) / totalTask;

                    await Task.Yield();

                    string brandName = ws.Cells[row, brandCol].Value?.ToString()!;
                    string image = ws.Cells[row, imgCol].Value?.ToString()!;

                    if (!string.IsNullOrEmpty(brandName))
                    {
                        brands.Add(new BrandModel { Id = setId, BrandName = brandName, StoredImageName = image });

                        await Task.Run(async () =>
                        {
                            await statusUpdate!.Invoke($"Brand Extracted: {brandName}", progressPercentage);
                        });

                        setId++;
                    }
                }
            }

            statusUpdate?.Invoke("------------------------End------------------------", progressPercentage);
            return brands;
        }

        public async Task<List<CategoryModel>> RetrieveCategoriesAsync(Stream stream, Func<string, int, Task> statusUpdate)
        {
            List<CategoryModel> categories = new List<CategoryModel>();

            using var package = new ExcelPackage(stream);
            var ws = package.Workbook.Worksheets[0];

            int startCol = 1;


            if (string.IsNullOrEmpty(ws.Cells[1, 2].Value?.ToString()) &&
                 string.IsNullOrEmpty(ws.Cells[1, 3].Value?.ToString()))
            {
                startCol = 2;
            }

            int startRow = FindRowWithMaxNonEmptyCells(ws);
            int currentCount = 0;
            int totalTask = (ws.Dimension.Rows - startRow) * (ws.Dimension.Columns - startCol);
            int percentDone = 0;

            var setId = 0;

            for (int col = startCol; col <= ws.Dimension.Columns; col++)
            {
                currentCount++;
                percentDone = (currentCount * 100) / totalTask;

                string categoryName = ws.Cells[startRow, col].Value?.ToString()!;
                if (!string.IsNullOrEmpty(categoryName))
                {
                    categories.Add(new CategoryModel { Id = setId, CategoryName = categoryName });

                    await Task.Run(async () =>
                    {
                        await statusUpdate!.Invoke($"Category Extracted: {categoryName}", percentDone);
                    });

                    setId++;
                }
            }

            statusUpdate?.Invoke("------------------------End------------------------", percentDone);
            return categories;
        }

        public async Task<List<SubCategoryModel>> RetrieveSubCategoriesAsync(Stream stream, Func<string, int, Task> statusUpdate)
        {
            List<SubCategoryModel> subCategories = new List<SubCategoryModel>();

            using var package = new ExcelPackage(stream);
            var ws = package.Workbook.Worksheets[0];

            int startCol = 1;


            if (string.IsNullOrEmpty(ws.Cells[1, 2].Value?.ToString()) &&
                 string.IsNullOrEmpty(ws.Cells[1, 3].Value?.ToString()))
            {
                startCol = 2;
            }

            int startRow = FindRowWithMaxNonEmptyCells(ws);
            int currentCount = 0;
            int totalTask = (ws.Dimension.Rows - startRow) * (ws.Dimension.Columns - startCol);
            int percentDone = 0;

            var setId = 0;

            for (int col = startCol; col <= ws.Dimension.Columns; col++)
            {
                var categoryHeader = ws.Cells[startRow, col].Value?.ToString();

                if (!string.IsNullOrEmpty(categoryHeader))
                {
                    await Task.Run(async () =>
                    {
                        await statusUpdate!.Invoke($"------ {categoryHeader} ------", percentDone);
                    });
                }

                for (int row = startRow + 1; row <= ws.Dimension.Rows + 1; row++)
                {
                    currentCount++;
                    percentDone = (int)((double)currentCount / totalTask * 100);
                    percentDone = Math.Min(percentDone, 100);

                    var cellValue = ws.Cells[row, col].Value?.ToString();
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        subCategories.Add(new SubCategoryModel { SubCategoryName = cellValue, CategoryId = setId });
                        await Task.Run(async () =>
                        {
                            await statusUpdate!.Invoke($"Sub Category Extracted: {cellValue}", percentDone);
                        });
                    }
                }

                setId++;
            }

            statusUpdate?.Invoke("------------------------End------------------------", percentDone);
            return subCategories;
        }

        private int FindRowWithMaxNonEmptyCells(ExcelWorksheet ws)
        {
            int indexRow = 0;
            int maxNonEmptyCells = 0;

            for (int row = 1; row <= ws.Dimension.Rows; row++)
            {
                int nonEmptyCells = 0;
                for (int col = 1; col <= ws.Dimension.Columns; col++)
                {
                    if (ws.Cells[row, col].Value != null)
                    {
                        nonEmptyCells++;
                    }
                }

                if (nonEmptyCells > maxNonEmptyCells)
                {
                    maxNonEmptyCells = nonEmptyCells;
                    indexRow = row;
                }
            }

            return indexRow;
        }

    }

}

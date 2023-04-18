using DorisApp.Data.Library.Model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DorisApp.Data.Library.Plugins.Excel
{
    public class ExcelConnector
    {

        public ExcelConnector()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }



        public async Task<List<BrandModel>> RetrieveBrandsAsync(Stream stream, Func<string, int, Task> statusUpdate)
        {
            List<BrandModel> brands = new List<BrandModel>();

            using var package = new ExcelPackage(stream);
            var ws = package.Workbook.Worksheets[0];

            int startCol = 1;

            if ((string.IsNullOrEmpty(ws.Cells[1, 1].Value?.ToString()) &&
                 string.IsNullOrEmpty(ws.Cells[1, 2].Value?.ToString())) ||
                (!string.IsNullOrEmpty(ws.Cells[1, 1].Value?.ToString()) &&
                string.IsNullOrEmpty(ws.Cells[1, 2].Value?.ToString())))
            {
                startCol = 2;
            }

            int startRow = FindRowWithMaxNonEmptyCells(ws);
            int totalTask = (ws.Dimension.Rows - 2) * (ws.Dimension.Columns - 1);
            int processedItem = 0;
            int progressPercentage = 0;
            var setId = 0;

            //Verify if sheet have Brand Name and Image header
            bool BrandNameHeaderVerified = false;
            bool ImageHeaderVerified = false;

            int brandCol = -1;
            int imgCol = -1;

            for (int col = startCol; col <= ws.Dimension.Columns + 1; col++)
            {
                var cellValue = ws.Cells[startRow, col].Value.ToString().ToLower().Replace(" ", string.Empty);

                if (cellValue.Contains("brandname") || cellValue.Contains("brandnames"))
                {
                    BrandNameHeaderVerified = true;
                    brandCol = col;
                }

                if (cellValue.Contains("image") || cellValue.Contains("images"))
                {
                    ImageHeaderVerified = true;
                    imgCol = col;
                }

            }

            if (BrandNameHeaderVerified && ImageHeaderVerified)
            {
                for (int row = startRow + 1; row <= ws.Dimension.Rows; row++)
                {
                    processedItem++;
                    progressPercentage = (processedItem * 100) / totalTask;

                    await Task.Run(() => {

                        string brandName = ws.Cells[row, brandCol].Value?.ToString()!;
                        string image = ws.Cells[row, imgCol].Value?.ToString()!;

                        if (!string.IsNullOrEmpty(brandName))
                        {
                            brands.Add(new BrandModel { Id = setId, BrandName = brandName, StoredImageName = image });
                            statusUpdate?.Invoke($"Brand Extracted: {brandName}", progressPercentage);
                            setId++;
                        }

                    });

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

            if ((string.IsNullOrEmpty(ws.Cells[1, 1].Value?.ToString()) &&
                string.IsNullOrEmpty(ws.Cells[1, 2].Value?.ToString())) ||
                (!string.IsNullOrEmpty(ws.Cells[1, 1].Value?.ToString()) &&
                string.IsNullOrEmpty(ws.Cells[1, 2].Value?.ToString())))
            {
                startCol = 2;
            }

            int startRow = FindRowWithMaxNonEmptyCells(ws);
            int currentCount = 0;
            int totalTask = (ws.Dimension.Rows) * (ws.Dimension.Columns);
            int percentDone = 0;

            await Task.Run(() =>
            {
                var setId = 0;

                for (int col = startCol; col <= ws.Dimension.Columns; col++)
                {
                    currentCount++;
                    percentDone = (int)((double)currentCount / totalTask * 100);
                    percentDone = Math.Min(percentDone, 100); 

                    string categoryName = ws.Cells[startRow, col].Value?.ToString()!;
                    if (!string.IsNullOrEmpty(categoryName))
                    {
                        categories.Add(new CategoryModel { Id=setId, CategoryName = categoryName });
                        statusUpdate?.Invoke($"Category Extracted: {categoryName}", percentDone);
                        setId++;
                    }
                }
            });

            statusUpdate?.Invoke("------------------------End------------------------", percentDone);
            return categories;
        }

        public async Task<List<SubCategoryModel>> RetrieveSubCategoriesAsync(Stream stream, Func<string, int, Task> statusUpdate)
        {
            List<SubCategoryModel> subCategories = new List<SubCategoryModel>();

            using var package = new ExcelPackage(stream);
            var ws = package.Workbook.Worksheets[0];

            int startCol = 1;

            if ((string.IsNullOrEmpty(ws.Cells[1, 1].Value?.ToString()) &&
                string.IsNullOrEmpty(ws.Cells[1, 2].Value?.ToString())) ||
                (!string.IsNullOrEmpty(ws.Cells[1, 1].Value?.ToString()) &&
                string.IsNullOrEmpty(ws.Cells[1, 2].Value?.ToString())))
            {
                startCol = 2;
            }

            int startRow = FindRowWithMaxNonEmptyCells(ws);
            int currentCount = 0;
            int totalTask = (ws.Dimension.Rows - 2) * (ws.Dimension.Columns);
            int percentDone = 0;

            await Task.Run(() =>
            {
                var setId = 0;

                for (int col = startCol; col <= ws.Dimension.Columns; col++)
                {
                    statusUpdate?.Invoke($"------ {ws.Cells[startRow, col].Value?.ToString()} ------", percentDone);

                    for (int row = startRow + 1; row <= ws.Dimension.Rows + 1; row++)
                    {
                        currentCount++;
                        percentDone = (int)((double)currentCount / totalTask * 100);
                        percentDone = Math.Min(percentDone, 100); 

                        var cellValue = ws.Cells[row, col].Value?.ToString();
                        if (!string.IsNullOrEmpty(cellValue))
                        {
                            subCategories.Add(new SubCategoryModel { SubCategoryName = cellValue, CategoryId = setId });
                            statusUpdate?.Invoke($"Sub Category Extracted: {cellValue}", percentDone);
                        }
                    }

                    setId++;
                }
            });
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

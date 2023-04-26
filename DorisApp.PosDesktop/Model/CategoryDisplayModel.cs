using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DorisApp.PosDesktop.Model
{
    public class CategoryDisplayModel
    {
        public string CategoryName { get; set; } = string.Empty;
        public List<SubCategorySummaryDTO>? Subcategories { get; set; }

    }
}

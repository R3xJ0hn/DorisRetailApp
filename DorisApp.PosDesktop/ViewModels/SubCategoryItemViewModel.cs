using Caliburn.Micro;
using DorisApp.Data.Library.DTO;
using System;
using System.Threading.Tasks;

namespace DorisApp.PosDesktop.ViewModels
{
    public class SubCategoryItemViewModel : Screen
    {
        private readonly Func<int, int, Task> _getSubcategoryId;
        private SubCategorySummaryDTO _subCategory;
        private bool _isActive;

        public int Id => _subCategory.Id;
        public string SubCategoryName => _subCategory.SubCategoryName!;

        public SubCategoryItemViewModel(Func<int, int, Task> GetSubcategoryId, SubCategorySummaryDTO subCategory)
        {
            _getSubcategoryId = GetSubcategoryId;
            _subCategory = subCategory;
        }

        public void HasClicked()
        {
            IsActiveSubCategory = true;
            _getSubcategoryId.Invoke(_subCategory.CategoryId, _subCategory.Id);
        }

        public bool IsActiveSubCategory
        {
            get => _isActive;
            set
            {
                _isActive = value;
                NotifyOfPropertyChange(() => IsActiveSubCategory);
            }
        }
        
    }
}

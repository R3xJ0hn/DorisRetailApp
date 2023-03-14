using DorisApp.Data.Library.Model;

namespace DorisApp.WebAPI.DataAccess
{
    public interface ICategoryData
    {
        void AddNewCategory(CategoryModel category, int userId);
        void Dispose();
    }
}
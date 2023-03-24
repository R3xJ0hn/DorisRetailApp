using AutoMapper;
using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;

namespace DorisApp.WebAPI.Helpers
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ProductModel, ProductSummaryDTO>();
        }
    }
}

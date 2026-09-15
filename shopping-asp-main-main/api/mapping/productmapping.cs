using core.Dto;
using core.Entities;

namespace api.mapping
{
    public class productmapping:AutoMapper.Profile
    {
        public productmapping()
        {
            CreateMap<product, ProductDto>().ForMember(x => x.CategoryId,
           op => op.MapFrom(src => src.CategoryId))

   
    .ReverseMap();

            CreateMap<photo, PhotoDto>().ReverseMap();

            CreateMap<AddproductDto, product>()
                .ForMember(m => m.photos, op => op.Ignore())
                .ReverseMap();


            CreateMap<updateproductDto, product>()
               .ForMember(m => m.photos, op => op.Ignore())
               .ReverseMap();
            CreateMap<ProductDto, updateproductDto>().ReverseMap();
        }
    }
}

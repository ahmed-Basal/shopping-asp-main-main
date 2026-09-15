using core.Dto;

namespace api.mapping
{
    public class CategoryMapper:AutoMapper.Profile
    {
        public CategoryMapper()
        {
            CreateMap<CategoryDto,core.Entities.category > ().ReverseMap();
            CreateMap<updateRecordCategory, core.Entities.category>().ReverseMap();

        }
    }
}

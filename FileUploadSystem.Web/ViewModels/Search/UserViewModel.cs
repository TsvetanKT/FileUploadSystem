namespace FileUploadSystem.Web.ViewModels.Search
{
    using FileUploadSystem.Models;
    using FileUploadSystem.Web.Infrastructure.Mapping;

    public class UserViewModel : IMapFrom<User>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public void CreateMappings(AutoMapper.IConfiguration configuration)
        {
            configuration.CreateMap<User, UserViewModel>()
                .ForMember(u => u.Id, opt => opt.MapFrom(v => v.Id))
                .ForMember(u => u.UserName, opt => opt.MapFrom(v => v.UserName))
                .ReverseMap();
        }
    }
}
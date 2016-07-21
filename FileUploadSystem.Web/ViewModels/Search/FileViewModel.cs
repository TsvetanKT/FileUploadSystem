namespace FileUploadSystem.Web.ViewModels.Search
{
    using AutoMapper;
    using FileUploadSystem.Models;
    using FileUploadSystem.Web.Infrastructure.Mapping;
    using System.ComponentModel.DataAnnotations;

    public class FileViewModel : IMapFrom<FileEntity>, IHaveCustomMappings
    {
        public int Id { get; set; }

        [Display(Name = "File Name")]
        public string Name { get; set; }

        public string OwnerId { get; set; }

        public string OwnerName { get; set; }

        public long Size { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<FileEntity, FileViewModel>()
                .ForMember(m => m.Name, opt => opt.MapFrom(e => e.Name))
                .ForMember(m => m.OwnerId, opt => opt.MapFrom(t => t.Owner.Id))
                .ForMember(m => m.OwnerName, opt => opt.MapFrom(t => t.Owner.UserName))
                .ForMember(m => m.Size, opt => opt.MapFrom
                    (e => (e is BinaryFile)
                        ? (e as BinaryFile).Size
                        : -1L))        
                .ReverseMap();
        }
    }
}
namespace FileUploadSystem.Web.ViewModels.Upload
{
    using AutoMapper;
    using FileUploadSystem.Models;
    using FileUploadSystem.Web.Infrastructure.Mapping;
    using System.ComponentModel.DataAnnotations;

    public class FileViewModel : IMapFrom<FileEntity>, IHaveCustomMappings
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "File Name")]
        [StringLength(255, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string Name { get; set; }

        public long Size { get; set; }

        public int LocationId { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<FileEntity, FileViewModel>()
                .ForMember(m => m.Name, opt => opt.MapFrom(e => e.Name))
                .ForMember(m => m.LocationId, opt => opt.MapFrom(e => e.LocationId))
                .ForMember(m => m.Size, opt => opt.MapFrom
                    (e => (e is BinaryFile)
                        ? (e as BinaryFile).Size
                        : -1L))
                .ReverseMap();
        }
    }
}
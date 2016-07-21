namespace FileUploadSystem.Models
{
    using System.ComponentModel.DataAnnotations;

    public class FileEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "File Name")]
        [StringLength(255, MinimumLength = 1)]
        public string Name { get; set; }

        public int LocationId { get; set; }

        public string OwnerId { get; set; }

        public virtual User Owner { get; set; }

    }
}

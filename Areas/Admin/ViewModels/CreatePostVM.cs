using System.ComponentModel.DataAnnotations;

namespace Indigo.Areas.Admin.ViewModels
{
    public class CreatePostVM
    {
        [Required, MaxLength(30)]
        public string Title { get; set; }
        [Required, MaxLength(60)]
        public string Description { get; set; }
        public bool IsDeleted { get; set; } = default;
        [Required]
        public IFormFile Photo { get; set; }
    }
}

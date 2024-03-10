using System.ComponentModel.DataAnnotations;

namespace AppY.ViewModels
{
    public class Discussion_ViewModel
    {
        public int Id { get; set; }
        [MaxLength(100, ErrorMessage = "Max length for discussion name: 100 chars")]
        public string? Name { get; set; }
        [MaxLength(600, ErrorMessage = "Max length for discussion description: 600 chars")]
        public string? Description { get; set; }
        [MaxLength(20, ErrorMessage = "Max length for shortlink: 20 chars")]
        [MinLength(4, ErrorMessage = "Min length for shortlink: 4 chars")]
        public string? Shortlink { get; set; }
        public int UserId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsPrivate { get; set; }
        [DataType(DataType.ImageUrl)]
        public string? AvatarUrl { get; set; }
        public string? Password { get; set; }
    }
}

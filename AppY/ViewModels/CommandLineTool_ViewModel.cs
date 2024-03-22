using System.ComponentModel.DataAnnotations;

namespace AppY.ViewModels
{
    public class CommandLineTool_ViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "Please shortly describe what actually this command is going to do")]
        [MinLength(4, ErrorMessage = "Description may contain at least 4 characters")]
        [MaxLength(240, ErrorMessage = "Please describe the command shortly")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Enter Command's call name")]
        [MaxLength(40, ErrorMessage = "Max length for Command's call name is 40 chars")]
        public string? Command { get; set; }
        [Required(ErrorMessage = "Please enter Command's code")]
        [MaxLength(4000)]
        public string? Code { get; set; }
        public int DiscussionId { get; set; }
        public bool IsDeleted { get; set; }
    }
}

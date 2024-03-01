using System.ComponentModel.DataAnnotations;

namespace AppY.ViewModels
{
    public class EditUserInfo_ViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "Pseudoname is required")]
        [MaxLength(40, ErrorMessage = "Max length for pseudoname: 40 chars")]
        public string? PseudoName { get; set; }
        [Required(ErrorMessage = "Shortname is required")]
        [MinLength(4, ErrorMessage = "Min length is 4 chars")]
        [MaxLength(20, ErrorMessage = "Max length for shortname: 20 chars")]
        public string? ShortName { get; set; }
        [MaxLength(450, ErrorMessage = "Max length for account description: 450 chars")]
        public string? Description { get; set; }
    }
}

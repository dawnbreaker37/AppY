using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AppY.Models
{
    public class User : IdentityUser<int>
    {
        [MaxLength(40)]
        public string? PseudoName { get; set; }
        [MaxLength(20)]
        public string? ShortName { get; set; }
        [MaxLength(6)]
        public string? ReserveCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDisabled { get; set; }
    }
}

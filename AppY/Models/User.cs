using Microsoft.AspNetCore.Identity;

namespace AppY.Models
{
    public class User : IdentityUser<int>
    {
        public string? Searchname { get; set; }
    }
}

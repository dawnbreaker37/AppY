using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppY.Models
{
    public class SecretChat
    {
        public int Id { get; set; }
        [MaxLength(40)]
        public string? Name { get; set; }
        public string? Encryption { get; set; }
        public int InitiatorId { get; set; }
        public List<User>? Users { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace AppY.ViewModels
{
    public class EditUserPrivacySettings
    {
        [Required(ErrorMessage = "Unable to edit settings without user information")]
        public int Id { get; set; }
        public bool IsPrivate { get; set; }
        public int AreMessagesAutoDeletable { get; set; }
    }
}

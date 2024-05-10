using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppY.Models
{
    public class User : IdentityUser<int>
    {
        [MaxLength(450)]
        public string? Description { get; set; }
        [MaxLength(40)]
        public string? PseudoName { get; set; }
        [MaxLength(20)]
        [MinLength(4)]
        public string? ShortName { get; set; }
        [MaxLength(6)]
        public string? ReserveCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPrivate { get; set; }
        public bool HideLastSeenInfo { get; set; }
        public int AreMessagesAutoDeletable { get; set; }
        public string? AvatarStickerUrl { get; set; }
        public string? AvatarBgColor { get; set; }
        public string? AvatarFgColor { get; set; }
        public int EcoModeOnAt { get; set; }
        [DataType(DataType.ImageUrl)]
        public string? AvatarUrl { get; set; }
        public DateTime? PasswordChanged { get; set; }
        public DateTime LastSeen { get; set; }
        public bool IsDisabled { get; set; }
        public List<NotificationModel>? Notifications { get; set; }
        public List<Discussion>? Discussions { get; set; }
        public List<Chat>? Chats { get; set; }
        public List<DiscussionMessage>? DiscussionMessages { get; set; }
        public List<ScheduledMessage>? ScheduledMessages { get; set; }
        public List<DiscussionUsers>? DiscussionUsers { get; set; }
        public List<LinkedAccount>? LinkedAccounts { get; set; }
        public List<ChatUsers>? ChatUsers { get; set; }
        public List<SecretChat>? SecretChats { get; set; }
        public List<SecretChatUsers>? SecretChatUsers { get; set; }
        public List<SavedMessageContent>? SavedMessageContents { get; set; }
        [NotMapped]
        public string? UnpicturedAvatarInfo { get; set; }
        [NotMapped]
        public string? LastSeenText { get; set; }
    }
}

using AppY.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppY.Data
{
    public class Context : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public Context(DbContextOptions<Context> Options) : base(Options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<NotificationModel> Notifications { get; set; }
        public DbSet<NotificationCategory> NotificationCategories { get; set; }
        public DbSet<Discussion> Discussions { get; set; }
        public DbSet<DiscussionUsers> DiscussionUsers { get; set; }
        public DbSet<DiscussionMessage> DiscussionMessages { get; set; }
        public DbSet<DiscussionMessageAnswer> DiscussionMessageAnswers { get; set; }
        public DbSet<DiscussionMessageImage> DiscussionMessageImages { get; set; }
        public DbSet<DiscussionMessageReaction> DiscussionMessageReactions { get; set; }
        public DbSet<CommandTool> Commands { get; set; }
        public DbSet<ScheduledMessage> ScheduledMessages { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatUsers> ChatUsers { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<LinkedAccount> LinkedAccounts { get; set; }
    }
}

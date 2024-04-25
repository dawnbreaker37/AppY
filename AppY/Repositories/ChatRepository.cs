using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AppY.Repositories
{
    public class ChatRepository : Base<Chat>, IChat
    {
        private readonly Context _context;

        public ChatRepository(Context context) : base(context)
        {
            _context = context;
        }

        public async Task<List<int>?> ChatUserIdsAsync(int Id)
        {
            if (Id > 0) return await _context.ChatUsers.AsNoTracking().Where(c => c.ChatId == Id && !c.IsDeleted && !c.IsBlocked).Select(c => c.UserId).ToListAsync();
            else return null;
        }

        public async Task<int> ChatSecondUserIdAsync(int Id, int UserId)
        {
            if (Id > 0 && UserId > 0) return await _context.ChatUsers.AsNoTracking().Where(c => c.ChatId == Id && c.UserId != UserId && !c.IsBlocked && !c.IsDeleted).Select(c => c.UserId).FirstOrDefaultAsync();
            else return 0;
        }

        public async Task<bool> CheckUserAvailabilityInChat(int Id, int UserId)
        {
            if (Id > 0 && UserId > 0) return await _context.ChatUsers.AsNoTracking().AnyAsync(c => c.ChatId == Id && c.UserId == UserId && !c.IsBlocked);
            else return false;
        }

        public async Task<int> CreateChatAsync(Chat_ViewModel Model)
        {
            if(Model.CreatorId > 0 && Model.SecondUserId > 0)
            {
                Chat chat = new Chat()
                {
                    CreatorId = Model.CreatorId,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false,
                    Name = Model.Name
                };

                await _context.AddAsync(chat);
                await _context.SaveChangesAsync();

                ChatUsers chatUser = new ChatUsers
                {
                    ChatId = chat.Id,
                    IsBlocked = false,
                    IsDeleted = false,
                    IsPinned = false,
                    JoinedAt = DateTime.Now,
                    UserId = Model.CreatorId
                };
                ChatUsers chatUser2 = new ChatUsers
                {
                    ChatId = chat.Id,
                    IsBlocked = false,
                    IsDeleted = false,
                    IsPinned = false,
                    JoinedAt = DateTime.Now,
                    UserId = Model.SecondUserId
                };
                await _context.AddAsync(chatUser);
                await _context.AddAsync(chatUser2);
                await _context.SaveChangesAsync();

                return chat.Id;
            }
            return 0;
        }

        public async Task<int> FindChatAvailability(int User1, int User2, bool CreateIfNot)
        {
            if(User1 > 0 && User2 > 0)
            {
                int ChatId = 0;
                if (!CreateIfNot) ChatId = await _context.Chats.AsNoTracking().Where(c => !c.IsDeleted && (c.ChatUsers != null) && ((c.CreatorId == User1 && User2 == c.ChatUsers.Where(c => c.UserId == User2).Select(c => c.UserId).FirstOrDefault()) || (c.CreatorId == User2 && User1 == c.ChatUsers.Where(c => c.UserId == User1).Select(c => c.UserId).FirstOrDefault()))).Select(c => c.Id).FirstOrDefaultAsync();
                else
                {
                    ChatId = await _context.Chats.AsNoTracking().Where(c => !c.IsDeleted && (c.ChatUsers != null) && ((c.CreatorId == User1 && User2 == c.ChatUsers.Where(c => c.UserId == User2).Select(c => c.UserId).FirstOrDefault()) || (c.CreatorId == User2 && User1 == c.ChatUsers.Where(c => c.UserId == User1).Select(c => c.UserId).FirstOrDefault()))).Select(c => c.Id).FirstOrDefaultAsync();
                    if (ChatId > 0) return ChatId;
                    else
                    {
                        Chat_ViewModel Model = new Chat_ViewModel()
                        {
                            CreatorId = User1,
                            SecondUserId = User2,
                            Name = null,
                            Description = null
                        };

                        int Result = await CreateChatAsync(Model);
                        if (Result > 0) return Result;
                    }
                }
            }
            return 0;
        }

        public async Task<Chat?> GetChatInfoAsync(int Id)
        {
            if (Id > 0) return await _context.Chats.AsNoTracking().Select(c => new Chat { Id = c.Id, CreatedAt = c.CreatedAt, Name = c.Name, CreatorId = c.CreatorId, IsDeleted = c.IsDeleted }).FirstOrDefaultAsync(c => c.Id == Id && !c.IsDeleted);
            else return null;
        }

        public async Task<int> EditChatInfoAsync(Chat_ViewModel Model)
        {
            if(!String.IsNullOrWhiteSpace(Model.Name) && Model.Id > 0)
            {
                if (String.IsNullOrWhiteSpace(Model.Shortname)) Model.Shortname = null;
                if (String.IsNullOrWhiteSpace(Model.Description)) Model.Description = null;
                int Result = await _context.Chats.AsNoTracking().Where(c => c.Id == Model.Id && !c.IsDeleted).ExecuteUpdateAsync(c => c.SetProperty(c => c.Name, Model.Name).SetProperty(c => c.Description, Model.Description).SetProperty(c => c.Shortname, Model.Shortname));
                if (Result > 0) return Result;
            }
            return 0;
        }

        public async Task<bool> CheckShortnameAvailability(int Id, string? Shortname)
        {
            if (!String.IsNullOrWhiteSpace(Shortname) && Id > 0)
            {
                bool Result = await _context.Chats.AsNoTracking().AnyAsync(c => c.Id != Id && c.Shortname != null && c.Shortname.ToLower() == Shortname.ToLower());
                if (Result) return false;
                else return true;
            }
            else return false;
        }

        public async Task<int> ClearChatHistoryAsync(int Id, int UserId)
        {
            if (Id > 0 && UserId > 0)
            {
                bool CheckUserAvailability = await CheckUserAvailabilityInChat(Id, UserId);
                if (CheckUserAvailability)
                {
                    int Result = await _context.ChatMessages.AsNoTracking().Where(c => c.ChatId == Id && !c.IsDeleted).ExecuteUpdateAsync(c => c.SetProperty(c => c.IsDeleted, true));
                    if (Result > 0) return Id;
                }
            }
            return 0;
        }

        public IQueryable<ChatUsers>? GetUserChats(int Id)
        {
            if (Id > 0)
            {
                return _context.ChatUsers.AsNoTracking().Where(c => c.UserId == Id && !c.IsBlocked && (!c.DeletedAt.HasValue || c.DeletedAt.Value.AddDays(5) >= DateTime.Now)).Select(c => new ChatUsers { Id = c.Id, ChatId = c.ChatId, ChatName = c.Chat!.Name, IsPinned = c.IsPinned, DeletedAt = c.DeletedAt, IsDeleted = c.IsDeleted, IsMuted = c.IsMuted }).OrderBy(c => c.DeletedAt).ThenByDescending(c => c.IsPinned);
            }
            else return null;
        }

        public async Task<int> DeleteChatAsync(int Id, int UserId)
        {
            if (Id > 0 && UserId > 0)
            {
                bool CheckUserAvailability = await CheckUserAvailabilityInChat(Id, UserId);
                if (CheckUserAvailability)
                {
                    int Result = await _context.ChatUsers.AsNoTracking().Where(c => c.ChatId == Id).ExecuteUpdateAsync(c => c.SetProperty(c => c.IsDeleted, true).SetProperty(c => c.DeletedAt, DateTime.Now));
                    if (Result > 0)
                    {
                        await _context.Chats.AsNoTracking().Where(c => c.Id == Id).ExecuteUpdateAsync(c => c.SetProperty(c => c.IsDeleted, true));
                        return Id;
                    }
                }
            }
            return 0;
        }

        public async Task<int> RestoreChatAsync(int Id, int UserId)
        {
            if(Id > 0 && UserId > 0)
            {
                bool CheckUserAvailability = await CheckUserAvailabilityInChat(Id, UserId);
                if(CheckUserAvailability)
                {
                    DateTime? NullDate = null;
                    int Result = await _context.ChatUsers.AsNoTracking().Where(c => c.ChatId == Id && c.IsDeleted && (c.DeletedAt.HasValue && c.DeletedAt.Value.AddDays(5) >= DateTime.Now)).ExecuteUpdateAsync(c => c.SetProperty(c => c.IsDeleted, false).SetProperty(c => c.DeletedAt, NullDate));
                    if (Result > 0)
                    {
                        await _context.Chats.AsNoTracking().Where(c => c.Id == Id).ExecuteUpdateAsync(c => c.SetProperty(c => c.IsDeleted, false));
                        return Id;
                    }
                }
            }
            return 0;
        }

        public async Task<int> PinTheChatAsync(int Id, int UserId)
        {
            if (Id > 0 && UserId > 0)
            {
                int Result = await _context.ChatUsers.AsNoTracking().Where(c => c.ChatId == Id && !c.IsDeleted && c.UserId == UserId).ExecuteUpdateAsync(c => c.SetProperty(c => c.IsPinned, true));
                if (Result > 0) return Result;
            }
            return 0;
        }

        public async Task<int> UnpinTheChatAsync(int Id, int UserId)
        {
            if (Id > 0 && UserId > 0)
            {
                int Result = await _context.ChatUsers.AsNoTracking().Where(c => c.ChatId == Id && !c.IsDeleted && c.UserId == UserId).ExecuteUpdateAsync(c => c.SetProperty(c => c.IsPinned, false));
                if (Result > 0) return Result;
            }
            return 0;
        }

        public async Task<bool> IsChatMutedAsync(int Id, int UserId)
        {
            if (Id > 0 && UserId > 0) return await _context.ChatUsers.AsNoTracking().AnyAsync(c => c.UserId == UserId && c.ChatId == Id && !c.IsDeleted && c.IsMuted);
            else return false;
        }

        public async Task<int> MuteTheChatAsync(int Id, int UserId)
        {
            if (Id > 0 && UserId > 0)
            {
                int Result = await _context.ChatUsers.AsNoTracking().Where(c => c.ChatId == Id && c.UserId == UserId && !c.IsDeleted).ExecuteUpdateAsync(c => c.SetProperty(c => c.IsMuted, true));
                if (Result > 0) return Result;
            }
            return 0;
        }

        public async Task<int> UnmuteTheChatAsync(int Id, int UserId)
        {
            if (Id > 0 && UserId > 0)
            {
                int Result = await _context.ChatUsers.AsNoTracking().Where(c => c.ChatId == Id && c.UserId == UserId && !c.IsDeleted).ExecuteUpdateAsync(c => c.SetProperty(c => c.IsMuted, false));
                if (Result > 0) return Result;
            }
            return 0;
        }

        public IQueryable<ChatUsers>? GetUserChatsShortly(int Id)
        {
            if (Id > 0) return _context.ChatUsers.AsNoTracking().Where(c => c.UserId == Id && !c.IsDeleted).Select(c => new ChatUsers { Id = c.Id, ChatId = c.ChatId, ChatName = c.Chat != null ? c.Chat.Name : "New Chat" });
            else return null;
        }
    }
}

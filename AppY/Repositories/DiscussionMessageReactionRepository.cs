using AppY.Abstractions;
using AppY.Data;
using AppY.Models;
using Microsoft.EntityFrameworkCore;

namespace AppY.Repositories
{
    public class DiscussionMessageReactionRepository : ReactionAbstraction
    {
        private readonly Context _context;
        public DiscussionMessageReactionRepository(Context context)
        {
            _context = context;
        }

        public async override Task<int> SetReactionAsync(int Id, int UserId, int ReactionId)
        {
            if(Id > 0 && UserId > 0 && ReactionId > 0)
            {
                await _context.DiscussionMessageReactions.AsNoTracking().Where(d => d.DiscussionMessageId == Id && d.UserId == UserId).ExecuteDeleteAsync();
                DiscussionMessageReaction discussionMessageReaction = new DiscussionMessageReaction
                {
                    DiscussionMessageId = Id,
                    UserId = UserId,
                    ReactionId = ReactionId,
                    IsDeleted = false
                };
                await _context.AddAsync(discussionMessageReaction);
                await _context.SaveChangesAsync();

                return await ReactionsCountAsync(Id);
            }
            return 0;
        }

        public async override Task<int> DeleteReactionAsync(int Id, int UserId)
        {
            if(Id > 0 && UserId > 0)
            {
                int Result = await _context.DiscussionMessageReactions.AsNoTracking().Where(d => d.DiscussionMessageId == Id && d.UserId == UserId).ExecuteDeleteAsync();
                if (Result > 0) return await ReactionsCountAsync(Id); ;
            }
            return 0;
        }

        public override IQueryable<IGrouping<int, DiscussionMessageReaction>>? GetMessageReactions(int Id)
        {
            if (Id > 0) return _context.DiscussionMessageReactions.AsNoTracking().Where(d => d.DiscussionMessageId == Id).Select(d => new DiscussionMessageReaction { UserId = d.UserId, ReactionId = d.ReactionId, ReactionCode = d.Reaction != null ? d.Reaction.ReactionCode : null }).GroupBy(d => d.ReactionId);
            else return null;
        }

        public async override Task<int> ReactionsCountAsync(int Id)
        {
            return await _context.DiscussionMessageReactions.AsNoTracking().CountAsync(dr => dr.DiscussionMessageId == Id);
        }
    }
}

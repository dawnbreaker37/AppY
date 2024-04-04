using AppY.Data;
using AppY.Interfaces;
using AppY.Models;

namespace AppY.Abstractions
{
    public abstract class ReactionAbstraction
    {
        public abstract Task<int> ReactionsCountAsync(int Id);
        public abstract Task<int> SetReactionAsync(int Id, int UserId, int ReactionId);
        public abstract Task<int> DeleteReactionAsync(int Id, int UserId);
        public abstract IQueryable<IGrouping<int, DiscussionMessageReaction>>? GetMessageReactions(int Id);
    }
}

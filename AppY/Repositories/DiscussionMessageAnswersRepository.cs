using AppY.Abstractions;
using AppY.Data;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AppY.Repositories
{
    public class DiscussionMessageAnswersRepository : Answer
    {
        private readonly Context _context;
        public DiscussionMessageAnswersRepository(Context context)
        {
            _context = context;
        }

        public async override Task<int> SendAnAnswerAsync(Answers_ViewModel Model)
        {
            if(!String.IsNullOrWhiteSpace(Model.Text) && Model.MessageId > 0 && Model.UserId > 0 && Model.DiscussionOrChatId > 0)
            {
                bool CheckUsersAvailability = await _context.DiscussionUsers.AsNoTracking().AnyAsync(d => d.UserId == Model.UserId && d.DiscussionId == Model.DiscussionOrChatId && !d.IsDeleted && !d.IsBlocked);
                if (CheckUsersAvailability)
                {
                    DiscussionMessageAnswer answer = new DiscussionMessageAnswer
                    {
                        Text = Model.Text,
                        UserId = Model.UserId,
                        IsDeleted = false,
                        IsEdited = false,
                        MessageId = Model.MessageId,
                        SentAt = DateTime.Now
                    };
                    await _context.AddAsync(answer);
                    await _context.SaveChangesAsync();

                    return answer.Id;
                }
            }
            return 0;
        }

        public async override Task<int> EditAnswerAsync(Answers_ViewModel Model)
        {
            if(Model.Id > 0 && Model.MessageId > 0 && Model.UserId > 0 && !String.IsNullOrWhiteSpace(Model.Text))
            {
                int Result = await _context.DiscussionMessageAnswers.AsNoTracking().Where(d => d.Id == Model.Id && d.UserId == Model.UserId && !d.IsDeleted).ExecuteUpdateAsync(d => d.SetProperty(d => d.Text, Model.Text).SetProperty(d => d.IsEdited, true));
                if (Result > 0) return Model.Id;
            }
            return 0;
        }

        public async override Task<int> DeleteAnswerAsync(int Id, int UserId)
        {
            if (Id > 0 && UserId > 0)
            {
                int Result = await _context.DiscussionMessageAnswers.Where(d => d.Id == Id && d.UserId == UserId && !d.IsDeleted).ExecuteUpdateAsync(d => d.SetProperty(d => d.IsDeleted, true));
                if (Result > 0) return Result;
            }
            return 0;
        }

        public override IQueryable<Answers_ViewModel>? GetAnswers(int Id, int UserId, int SkipCount, int LoadCount)
        {
            if (Id > 0 && UserId > 0 && LoadCount > 0) return _context.DiscussionMessageAnswers.AsNoTracking().Where(d => d.MessageId == Id && !d.IsDeleted).Select(d => new Answers_ViewModel { Id = d.Id, Text = d.Text, CreatedAt = d.SentAt, IsEdited = d.IsEdited, UserId = d.UserId }).Skip(SkipCount).Take(LoadCount).OrderBy(d => d.CreatedAt);
            else return null;
        } 
    }
}

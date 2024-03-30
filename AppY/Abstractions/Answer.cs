using AppY.Repositories;
using AppY.ViewModels;

namespace AppY.Abstractions
{
    public abstract class Answer
    {
        public abstract IQueryable<Answers_ViewModel>? GetAnswers(int Id, int UserId, int SkipCount, int LoadCount);
        public abstract Task<int> SendAnAnswerAsync(Answers_ViewModel Model);
        public abstract Task<int> EditAnswerAsync(Answers_ViewModel Model);
        public abstract Task<int> DeleteAnswerAsync(int Id, int UserId);
    }
}

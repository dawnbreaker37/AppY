using AppY.Models;
using AppY.Repositories;
using AppY.ViewModels;

namespace AppY.Interfaces
{
    public interface ICommandLine
    {
        public IQueryable<CommandTool>? GetDiscussionCommands(int Id);
        public IQueryable<CommandTool>? GetLiveDiscussionCommands(int Id);
        public IQueryable<CommandTool>? GetChatCommands(int Id);
        public Task<string?> CreateCommandAsync(CommandLineTool_ViewModel Model);
        public Task<string?> EditCommandAsync(CommandLineTool_ViewModel Model);
        public Task<string?> DeleteCommandAsync(int Id, int UserId);
    }
}

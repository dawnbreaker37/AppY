using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AppY.Repositories
{
    public class CommandLineTool : Base<CommandTool>, ICommandLine
    {
        private readonly Context _context;
        public CommandLineTool(Context context) : base(context)
        {
            _context = context;
        }

        public async Task<string?> CreateCommandAsync(CommandLineTool_ViewModel Model)
        {
            if(!String.IsNullOrWhiteSpace(Model.Command) && !String.IsNullOrWhiteSpace(Model.Code) && !String.IsNullOrWhiteSpace(Model.Description) && Model.DiscussionId > 0)
            {
                CommandTool commandTool = new CommandTool
                {
                    Code = Model.Code,
                    CommandCall = Model.Command,
                    Description = Model.Description,
                    DiscussionId = Model.DiscussionId,
                    IsDeleted = false
                };
                await _context.AddAsync(commandTool);
                await _context.SaveChangesAsync();

                return Model.Command;
            }
            return null;
        }

        public Task<string?> DeleteCommandAsync(int Id, int UserId)
        {
            throw new NotImplementedException();
        }

        public Task<string?> EditCommandAsync(CommandLineTool_ViewModel Model)
        {
            throw new NotImplementedException();
        }

        public IQueryable<CommandTool>? GetChatCommands(int Id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<CommandTool>? GetDiscussionCommands(int Id)
        {
            if (Id != 0) return _context.Commands.AsNoTracking().Where(d => d.DiscussionId == Id && !d.IsDeleted).Select(d => new CommandTool { Id = d.Id, CommandCall = d.CommandCall, Description = d.Description });
            else return null;
        }

        public IQueryable<CommandTool>? GetLiveDiscussionCommands(int Id)
        {
            throw new NotImplementedException();
        }
    }
}

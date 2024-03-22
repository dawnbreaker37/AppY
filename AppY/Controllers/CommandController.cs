using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppY.Controllers
{
    public class CommandController : Controller
    {
        private readonly Context _context;
        private readonly ICommandLine _commandLine;

        public CommandController(Context context, ICommandLine commandLine)
        {
            _context = context;
            _commandLine = commandLine;
        }

        [HttpGet]
        public async Task<IActionResult> GetDiscussionCommands(int Id)
        {
            IQueryable<CommandTool>? CommandTools_Preview = _commandLine.GetDiscussionCommands(Id);
            if (CommandTools_Preview != null)
            {
                List<CommandTool>? Commands = await CommandTools_Preview.ToListAsync();
                if (Commands.Count > 0) return Json(new { success = true, result = Commands, count = Commands.Count });
                else return Json(new { success = false, alert = "No additional created commands for this discussion" });
            }
            else return Json(new { success = false, alert = "We're sorry, but we're unable to get commands for this discussion now. Please, try to get them later" });
        }
    }
}

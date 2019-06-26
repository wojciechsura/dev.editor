using Dev.Editor.BusinessLogic.Models.Commands;
using Dev.Editor.Common.Commands;
using Dev.Editor.Common.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.Services.Commands
{
    public class CommandRepositoryService : ICommandRepositoryService
    {
        // Private fields -----------------------------------------------------

        private readonly List<CommandModel> commands = new List<CommandModel>();

        // Private methods ----------------------------------------------------

        private bool IncrementalMatch(string target, string text)
        {
            int i = 0, j = 0;

            while (i < target.Length && j < text.Length)
            {
                if (target[i] == text[j])
                    j++;

                i++;
            }

            return j == text.Length;
        }

        // Public methods -----------------------------------------------------

        public CommandRepositoryService()
        {

        }

        public ICommand RegisterCommand(string title, string iconResource, Action<object> action, BaseCondition condition = null)
        {
            var command = new AppCommand(action, condition);
            commands.Add(new CommandModel(title, iconResource, command));
            return command;
        }

        public List<CommandModel> FindMatching(string text)
        {
            return commands
                .Where(c => IncrementalMatch(c.Title, text))
                .ToList();            
        }
    }
}

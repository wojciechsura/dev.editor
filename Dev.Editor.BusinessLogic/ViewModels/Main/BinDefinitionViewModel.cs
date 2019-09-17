using Dev.Editor.BusinessLogic.Models.Configuration.BinDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public class BinDefinitionViewModel
    {
        private readonly BinDefinition binDefinition;

        public BinDefinitionViewModel(BinDefinition binDefinition)
        {
            this.binDefinition = binDefinition;
        }

        public string Name => binDefinition.DefinitionName.Value;
        public string Path => binDefinition.Filename.Value;
    }
}

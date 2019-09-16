using Dev.Editor.BusinessLogic.Services.ImageResources;
using Dev.Editor.BusinessLogic.ViewModels.Tools.Base;
using Dev.Editor.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.Tools.BinDefinitions
{
    public class BinDefinitionsToolViewModel : BaseToolViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly IImageResources imageResources;

        private readonly ImageSource icon;

        // Public methods -----------------------------------------------------

        public BinDefinitionsToolViewModel(IImageResources imageResources)
        {
            this.imageResources = imageResources;

            icon = this.icon = imageResources.GetIconByName("Binary16.png");
        }

        // Public properties --------------------------------------------------

        public override string Title => Strings.Tool_BinDefinitions_Title;

        public override ImageSource Icon => icon;

        public override string Uid => BinaryDefinitionsUid;
    }
}

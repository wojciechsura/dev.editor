using Dev.Editor.BusinessLogic.Services.FileIcons;
using Dev.Editor.BusinessLogic.Services.ImageResources;
using Dev.Editor.BusinessLogic.ViewModels.Tools.Base;
using Dev.Editor.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.Tools.Explorer
{
    public class ExplorerToolViewModel : BaseToolViewModel
    {
        private readonly IFileIconProvider fileIconProvider;
        private readonly IImageResources imageResources;
        private readonly ImageSource icon;

        public ExplorerToolViewModel(IFileIconProvider fileIconProvider, IImageResources imageResources)
        {
            this.fileIconProvider = fileIconProvider;
            this.imageResources = imageResources;

            this.icon = imageResources.GetIconByName("Explorer16.png");
        }

        public override string Title => Strings.Tool_Explorer_Title;

        public override ImageSource Icon => icon;
    }
}

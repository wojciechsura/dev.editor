using Dev.Editor.BusinessLogic.Models.Events;
using Dev.Editor.BusinessLogic.Services.EventBus;
using Dev.Editor.BusinessLogic.Services.ImageResources;
using Dev.Editor.BusinessLogic.ViewModels.Tools.Base;
using Dev.Editor.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.Tools.Project
{
    public class ProjectToolViewModel : BaseToolViewModel, IEventListener<ApplicationActivatedEvent>
    {
        private readonly IEventBus eventBus;
        private readonly IImageResources imageResources;
        private readonly ImageSource icon;

        public ProjectToolViewModel(IToolHandler handler, IEventBus eventBus, IImageResources imageResources)
            : base(handler)
        {
            this.eventBus = eventBus;
            this.imageResources = imageResources;

            eventBus.Register((IEventListener<ApplicationActivatedEvent>)this);

            this.icon = imageResources.GetIconByName("Open16.png");
        }

        public override string Title => Strings.Tool_Project_Title;

        public override ImageSource Icon => icon;

        public override string Uid => ProjectUid;

        public void Receive(ApplicationActivatedEvent @event)
        {
            // TODO
        }
    }
}

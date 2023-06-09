﻿using Dev.Editor.BusinessLogic.Models.Messages;
using Dev.Editor.BusinessLogic.Services.ImageResources;
using Dev.Editor.BusinessLogic.ViewModels.BottomTools.Base;
using Dev.Editor.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.BottomTools.Messages
{
    public class MessagesBottomToolViewModel : BaseBottomToolViewModel
    {
        // Private methods ----------------------------------------------------

        private readonly ObservableCollection<MessageModel> messages = new ObservableCollection<MessageModel>();
        private readonly IMessagesHandler messageHandler;
        private readonly IImageResources imageResources;

        private readonly ImageSource icon;

        // Public methods -----------------------------------------------------

        public MessagesBottomToolViewModel(IMessagesHandler handler, IImageResources imageResources)
            : base(handler)
        {
            this.messageHandler = handler;
            this.imageResources = imageResources;

            icon = imageResources.GetIconByName("Message16.png");
        }

        public void NotifyMessageChosen(MessageModel model)
        {
            if (model.Filename != null)
            {
                messageHandler.OpenFileAndFocus(model.Path);
            }
        }

        public void ClearMessages()
        {
            messages.Clear();
        }

        public void AddMessage(MessageModel message)
        {
            messages.Add(message);
        }

        // Public properties --------------------------------------------------

        public override string Title => Strings.BottomTool_Messages_Title;

        public override ImageSource Icon => icon;

        public override string Uid => MessagesUid;

        public ObservableCollection<MessageModel> Messages => messages;
    }
}

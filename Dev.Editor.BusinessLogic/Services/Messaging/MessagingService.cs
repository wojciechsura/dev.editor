using Dev.Editor.BusinessLogic.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dev.Editor.BusinessLogic.Services.Messaging
{
    class MessagingService : IMessagingService
    {
        public bool AskYesNo(string message, string title = null)
        {
            if (title == null)
                title = Resources.DefaultMessageboxTitle;

            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
                return true;
            else if (result == MessageBoxResult.No)
                return false;
            else
                throw new InvalidOperationException("Invalid result!");
        }

        public bool? AskYesNoCancel(string message, string title = null)
        {
            if (title == null)
                title = Resources.DefaultMessageboxTitle;

            var result = MessageBox.Show(message, title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
                return true;
            else if (result == MessageBoxResult.No)
                return false;
            else if (result == MessageBoxResult.Cancel)
                return null;
            else
                throw new InvalidOperationException("Invalid result!");
        }

        public void Warn(string message, string title = null)
        {
            if (title == null)
                title = Resources.DefaultMessageboxTitle;

            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void ShowError(string message, string title = null)
        {
            if (title == null)
                title = Resources.DefaultMessageboxTitle;

            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

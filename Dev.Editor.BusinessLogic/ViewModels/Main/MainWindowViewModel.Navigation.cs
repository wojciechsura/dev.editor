using Dev.Editor.BusinessLogic.Models.Navigation;
using Dev.Editor.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel
    {
        private const int MIN_NAVIGATION_TEXT_LENGTH = 1;

        // Private methods ----------------------------------------------------

        private void DoNavigate()
        {
            NavigationText = String.Empty;
            navigationItems.Clear();

            access.ShowNavigationPopup();
        }

        // Public methods -----------------------------------------------------

        public void PerformNavigationSearch()
        {
            navigationItems.Clear();
            SelectedNavigationItem = null;

            if (navigationText.Length >= MIN_NAVIGATION_TEXT_LENGTH)
            {
                var commandResults = commandRepositoryService.FindMatching(NavigationText)
                    .OrderBy(r => r.Title)
                    .Select(result => new CommandNavigationModel(result.Title, Strings.Navigation_Group_Commands, imageResources.GetIconByName(result.IconResource), result.Command.CanExecute(null), result.Command));
                foreach (var cr in commandResults)
                    navigationItems.Add(cr);

                var projectItemResults = projectToolViewModel.FindMatching(NavigationText)
                    .OrderBy(r => r.Filename)
                    .Select(result => new FileProjectNavigationModel(result.Path, Strings.Navigation_Group_ProjectFiles, result.Filename, result.Icon, true));
                foreach (var fr in projectItemResults)
                    navigationItems.Add(fr);

                SelectedNavigationItem = navigationItems.FirstOrDefault();
            }
        }

        public void NavigationItemChosen()
        {
            if (selectedNavigationItem == null || !selectedNavigationItem.Enabled)
            {
                messagingService.Beep();
            }
            else if (selectedNavigationItem is CommandNavigationModel commandItem)
            {
                access.HideNavigationPopup();
                commandItem.Command.Execute(null);
            }
            else if (selectedNavigationItem is FileProjectNavigationModel fileItem)
            {
                access.HideNavigationPopup();
                LoadTextDocument(documentsManager.ActiveDocumentTab, fileItem.Path);
            }    
        }
    }
}

using Dev.Editor.BusinessLogic.Models.Configuration.BinDefinitions;
using Dev.Editor.BusinessLogic.Models.Dialogs;
using Dev.Editor.BusinessLogic.Services.Config;
using Dev.Editor.BusinessLogic.Services.Dialogs;
using Dev.Editor.BusinessLogic.Services.ImageResources;
using Dev.Editor.BusinessLogic.Services.Messaging;
using Dev.Editor.BusinessLogic.Services.Paths;
using Dev.Editor.BusinessLogic.ViewModels.Main;
using Dev.Editor.BusinessLogic.ViewModels.Tools.Base;
using Dev.Editor.Common.Commands;
using Dev.Editor.Common.Conditions;
using Dev.Editor.Common.Tools;
using Dev.Editor.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.Tools.BinDefinitions
{
    public class BinDefinitionsToolViewModel : BaseToolViewModel
    {
        private readonly IBinDefinitionsHandler binDefinitionHandler;

        // Private fields -----------------------------------------------------

        private readonly IImageResources imageResources;
        private readonly IConfigurationService configurationService;
        private readonly IDialogService dialogService;
        private readonly IMessagingService messagingService;
        private readonly IPathService pathService;
        private readonly ImageSource icon;

        private IBinDefinitionsToolAccess access;

        private readonly ObservableCollection<BinDefinitionViewModel> binDefinitions;
        private BinDefinitionViewModel selectedBinDefinition;
        private readonly Condition binDefinitionSelected;

        // Private methods ----------------------------------------------------

        private void HandleSelectedBinDefinitionChanged()
        {
            binDefinitionSelected.Value = selectedBinDefinition != null;
        }

        public void DoAddBinaryDefinition()
        {
            var model = new NameDialogModel("", Strings.Dialog_BinDefinition_Title, Strings.Dialog_BinDefinition_Title);

            (bool result, NameDialogResult data) = dialogService.ShowChooseNameDialog(model);
            if (result)
            {
                BinDefinition binDefinition;

                try
                {
                    binDefinition = new BinDefinition();
                    binDefinition.DefinitionName.Value = data.Name;
                    binDefinition.Uid.Value = Guid.NewGuid().ToString();

                    // Create file

                    int i = 0;
                    while (File.Exists($"{pathService.BinDefinitionsPath}\\Definition{i}.bindef"))
                        i++;

                    string filename = $"Definition{i}.bindef";
                    string fullFilename = $"{pathService.BinDefinitionsPath}\\{filename}";

                    File.WriteAllText(fullFilename, string.Format(Strings.NewBinaryDefinitionContents, binDefinition.DefinitionName.Value));

                    binDefinition.Filename.Value = filename;

                    // Save in configuration

                    configurationService.Configuration.BinDefinitions.Add(binDefinition);
                    configurationService.Save();

                    // Insert to viewmodel to display

                    i = 0;
                    while (i < binDefinitions.Count && binDefinitions[i].Name.CompareTo(binDefinition.DefinitionName.Value) < 0)
                        i++;

                    var defViewModel = new BinDefinitionViewModel(binDefinition);
                    binDefinitions.Insert(i, defViewModel);

                    // Select new definition

                    SelectedBinDefinition = defViewModel;

                    // Open definitionFile

                    binDefinitionHandler.OpenTextFile(fullFilename);
                }
                catch (Exception e)
                {
                    messagingService.ShowError(string.Format(Strings.Message_CannotAddBinDefinition, e.Message));
                }
            }
        }

        public void DoRemoveBinaryDefinition()
        {
            if (messagingService.AskYesNo(string.Format(Strings.Message_BinDefinitionDeleteConfirmation, selectedBinDefinition.Name)))
            {
                configurationService.Configuration.BinDefinitions.Remove(selectedBinDefinition.BinDefinition);
                configurationService.Save();

                binDefinitions.Remove(selectedBinDefinition);

                SelectedBinDefinition = null;
            }
        }

        public void DoEditBinaryDefinition()
        {
            binDefinitionHandler.OpenTextFile($"{pathService.BinDefinitionsPath}\\{selectedBinDefinition.BinDefinition.Filename.Value}");
        }

        public void DoBinaryDefinitionSample()
        {
            binDefinitionHandler.NewTextDocument(Strings.BinDefinitionSample);
        }

        // Public methods -----------------------------------------------------

        public BinDefinitionsToolViewModel(IBinDefinitionsHandler handler,
            IImageResources imageResources, 
            IConfigurationService configurationService,
            IDialogService dialogService,
            IMessagingService messagingService,
            IPathService pathService)
            : base(handler)
        {
            this.binDefinitionHandler = handler;
            this.imageResources = imageResources;
            this.configurationService = configurationService;
            this.dialogService = dialogService;
            this.messagingService = messagingService;
            this.pathService = pathService;

            icon = this.icon = imageResources.GetIconByName("Binary16.png");

            // BinDefinitions

            binDefinitions = new ObservableCollection<BinDefinitionViewModel>();
            configurationService.Configuration.BinDefinitions
                .Select(bd => new BinDefinitionViewModel(bd))
                .OrderBy(bd => bd.Name)
                .ForEach(vm => binDefinitions.Add(vm));

            // Condition

            binDefinitionSelected = new Condition(selectedBinDefinition != null);

            // Commands

            AddBinaryDefinition = new AppCommand(obj => DoAddBinaryDefinition());
            RemoveBinaryDefinition = new AppCommand(obj => DoRemoveBinaryDefinition(), binDefinitionSelected);
            EditBinaryDefinition = new AppCommand(obj => DoEditBinaryDefinition(), binDefinitionSelected);
            OpenBinaryDefinitionSample = new AppCommand(obj => DoBinaryDefinitionSample());
        }

        public void BinDefinitionChosen()
        {
            if (selectedBinDefinition != null)
            {
                binDefinitionHandler.RequestOpenBinFile(selectedBinDefinition.BinDefinition);
            }
        }

        // Public properties --------------------------------------------------

        public IBinDefinitionsToolAccess Access
        {
            get => access;
            set
            {
                access = value;
            }
        }

        public override string Title => Strings.Tool_BinDefinitions_Title;

        public override ImageSource Icon => icon;

        public override string Uid => BinaryDefinitionsUid;

        public ObservableCollection<BinDefinitionViewModel> BinDefinitions => binDefinitions;

        public BinDefinitionViewModel SelectedBinDefinition
        {
            get => selectedBinDefinition;
            set
            {
                Set(ref selectedBinDefinition, () => SelectedBinDefinition, value, HandleSelectedBinDefinitionChanged);
            }
        }

        public ICommand AddBinaryDefinition { get; }
        public ICommand RemoveBinaryDefinition { get; }
        public ICommand EditBinaryDefinition { get; }
        public ICommand OpenBinaryDefinitionSample { get; }
    }
}

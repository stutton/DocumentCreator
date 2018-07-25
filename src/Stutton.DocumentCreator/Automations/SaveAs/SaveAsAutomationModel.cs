using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models.Document;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;

namespace Stutton.DocumentCreator.Automations.SaveAs
{
    public class SaveAsAutomationModel : AutomationModelBase
    {
        public override string TypeDisplayName => "Save As";
        public override string Description => "Save the document to a specified location";

        private string _name;
        public override string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private string _savePath;
        public string SavePath
        {
            get => _savePath;
            set => Set(ref _savePath, value);
        }

        #region Browse Command

        private ICommand _browseCommand;
        public ICommand BrowseCommand => _browseCommand ?? (_browseCommand = new RelayCommand(async () => await Browse()));

        private async Task Browse()
        {
            var dialogVm = new MaterialOpenFolderDialogViewModel();
            if ((bool) await DialogHost.Show(dialogVm, MainWindow.RootDialog))
            {
                SavePath = dialogVm.SelectedFolder;
            }
        }

        #endregion

        public override async Task<IResponse> Execute(DocumentModel document, IWorkItem workItem, string documentPath)
        {
            try
            {
                await Task.Run(() => File.Copy(documentPath, SavePath));
                return Response.FromSuccess();
            }
            catch (Exception ex)
            {
                return Response.FromException($"Failed to save document to {SavePath}", ex);
            }
        }
    }
}

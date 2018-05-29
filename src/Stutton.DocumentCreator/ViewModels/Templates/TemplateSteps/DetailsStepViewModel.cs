﻿using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models.Documents.Details;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;

namespace Stutton.DocumentCreator.ViewModels.Templates.TemplateSteps
{
    public class DetailsStepViewModel
    {
        public DocumentDetailsModel Model { get; }

        #region ICommand BrowseCommand

        private ICommand _browseCommand;
        public ICommand BrowseCommand => _browseCommand ?? (_browseCommand = new RelayCommand(async () => await Browse()));

        private async Task Browse()
        {
            var dialogVm = new MaterialOpenFileDialogViewModel();
            if ((bool) await DialogHost.Show(dialogVm, MainWindow.RootDialog))
            {
                Model.TemplateFilePath = dialogVm.SelectedFile;
            }
        }

        #endregion

        public DetailsStepViewModel(DocumentDetailsModel model)
        {
            Model = model;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Services.Document;
using Stutton.DocumentCreator.Services.Telemetry;
using Stutton.DocumentCreator.Services.Templates;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;
using Stutton.DocumentCreator.ViewModels.Documents;
using Stutton.DocumentCreator.ViewModels.Navigation;
using Stutton.DocumentCreator.ViewModels.Templates;

namespace Stutton.DocumentCreator.ViewModels.Pages
{
    public class DocumentsPageViewModel : PageBase
    {
        public const string Key = "DocumentsPage";
        public override string PageKey => Key;
        public override string Title => "Documents";
        public override bool IsOnDemandPage => false;
        public override int PageOrder => 1;

        private readonly ITemplatesService _templatesService;
        private readonly INavigationService _navigationService;
        private readonly ITelemetryService _telemetryService;
        private readonly IDocumentService _documentService;
        private readonly ISnackbarMessageQueue _messageQueue;

        public DocumentsPageViewModel(ITemplatesService templatesService, 
                                      INavigationService navigationService, 
                                      ITelemetryService telemetryService,
                                      IDocumentService documentService,
                                      ISnackbarMessageQueue messageQueue)
        :base(navigationService)
        {
            _templatesService = templatesService ?? throw new ArgumentNullException(nameof(templatesService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _telemetryService = telemetryService ?? throw new ArgumentNullException(nameof(telemetryService));
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
            _messageQueue = messageQueue ?? throw new ArgumentNullException(nameof(messageQueue));
        }

        private ObservableCollection<TemplateCardViewModel> _propertyName;

        public ObservableCollection<TemplateCardViewModel> Templates
        {
            get => _propertyName;
            private set => Set(ref _propertyName, value);
        }

        private ObservableCollection<DocumentCardViewModel> _savedDocuments;

        public ObservableCollection<DocumentCardViewModel> SavedDocuments
        {
            get => _savedDocuments;
            private set => Set(ref _savedDocuments, value);
        }

        #region ICommand CreateDocumentTemplateCommand

        private ICommand _createDocumentTemplateCommand;
        public ICommand CreateDocumentTemplateCommand => _createDocumentTemplateCommand ?? (_createDocumentTemplateCommand = new RelayCommand(CreateDocumentTemplate));

        private void CreateDocumentTemplate()
        {
            _navigationService.NavigateTo(EditTemplatePageViewModel.Key);
        }

        #endregion

        #region ImportTemplate Command

        private ICommand _importTemplateCommand;
        public ICommand ImportTemplateCommand => _importTemplateCommand ?? (_importTemplateCommand = new RelayCommand(async () => await ImportTemplate()));

        private async Task ImportTemplate()
        {
            var openFileDialog = new MaterialOpenFileDialogViewModel("Zip File|*.zip");
            if ((bool) await DialogHost.Show(openFileDialog, MainWindow.RootDialog))
            {
                var importFile = openFileDialog.SelectedFile;
                var response = await _templatesService.ImportDocumentTemplate(importFile);
                if (!response.Success)
                {
                    _telemetryService.TrackFailedResponse(response);
                    await DialogHost.Show(new ErrorMessageDialogViewModel(response.Message, _telemetryService.SessionId), MainWindow.RootDialog);
                    return;
                }

                IsBusy = true;

                await LoadTemplates();

                IsBusy = false;
            }
        }

        #endregion

        public override async Task NavigatedTo(object parameter)
        {
            try
            {
                IsBusy = true;

                _telemetryService.TrackPageView(Key);

                var documentResponse = await _documentService.LoadAllSavedDocumentsAsync();

                if (!documentResponse.Success)
                {
                    _telemetryService.TrackFailedResponse(documentResponse);
                    await DialogHost.Show(new ErrorMessageDialogViewModel(documentResponse.Message, _telemetryService.SessionId));
                }
                else
                {
                    SavedDocuments = new ObservableCollection<DocumentCardViewModel>(
                        documentResponse.Value.Select(d =>
                        {
                            var card = new DocumentCardViewModel(d, _navigationService);
                            card.RequestDeleteMe += SavedDocumentCardOnRequestDeleteMe;
                            return card;
                        }));
                }

                await LoadTemplates();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void CardOnRequestDeleteMe(object sender, EventArgs e)
        {
            try
            {
                IsBusy = true;

                var toDelete = (TemplateCardViewModel) sender;

                var response = await _templatesService.DeleteDocumentTemplate(toDelete.Model);
                if (!response.Success)
                {
                    _telemetryService.TrackFailedResponse(response);
                    await DialogHost.Show(new ErrorMessageDialogViewModel(response.Message, _telemetryService.SessionId), MainWindow.RootDialog);
                    return;
                }

                toDelete.RequestDeleteMe -= CardOnRequestDeleteMe;
                Templates.Remove(toDelete);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void SavedDocumentCardOnRequestDeleteMe(object sender, EventArgs e)
        {
            try
            {
                IsBusy = true;

                var toDelete = (DocumentCardViewModel) sender;

                var response = await _documentService.DeleteSavedDocumentAsync(toDelete.Model);
                if (!response.Success)
                {
                    _telemetryService.TrackFailedResponse(response);
                    await DialogHost.Show(new ErrorMessageDialogViewModel(response.Message, _telemetryService.SessionId), MainWindow.RootDialog);
                    return;
                }

                toDelete.RequestDeleteMe -= SavedDocumentCardOnRequestDeleteMe;
                SavedDocuments.Remove(toDelete);

                _messageQueue.Enqueue($"Deleted {toDelete.Model.FileName}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadTemplates()
        {
            var templateResponse = await _templatesService.GetDocuments();

            if (!templateResponse.Success)
            {
                _telemetryService.TrackFailedResponse(templateResponse);
                await DialogHost.Show(new ErrorMessageDialogViewModel(templateResponse.Message, _telemetryService.SessionId));
                return;
            }

            Templates = new ObservableCollection<TemplateCardViewModel>(
                templateResponse.Value.Select(d =>
                {
                    var card = new TemplateCardViewModel(d, _navigationService, _templatesService, _telemetryService);
                    card.RequestDeleteMe += CardOnRequestDeleteMe;
                    return card;
                }));
        }
    }
}

using System;
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
            _templatesService = templatesService;
            _navigationService = navigationService;
            _telemetryService = telemetryService;
            _documentService = documentService;
            _messageQueue = messageQueue;
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
                    await DialogHost.Show(new ErrorMessageDialogViewModel(documentResponse.Message));
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

                var templateResponse = await _templatesService.GetDocuments();

                if (!templateResponse.Success)
                {
                    _telemetryService.TrackFailedResponse(templateResponse);
                    await DialogHost.Show(new ErrorMessageDialogViewModel(templateResponse.Message));
                    return;
                }

                Templates = new ObservableCollection<TemplateCardViewModel>(
                    templateResponse.Value.Select(d =>
                    {
                        var card = new TemplateCardViewModel(d, _navigationService);
                        card.RequestDeleteMe += CardOnRequestDeleteMe;
                        return card;
                    }));
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
                    await DialogHost.Show(new ErrorMessageDialogViewModel(response.Message), MainWindow.RootDialog);
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
                    await DialogHost.Show(new ErrorMessageDialogViewModel(response.Message), MainWindow.RootDialog);
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
    }
}

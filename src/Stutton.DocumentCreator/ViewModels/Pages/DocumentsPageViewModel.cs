using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Services.Telemetry;
using Stutton.DocumentCreator.Services.Templates;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;
using Stutton.DocumentCreator.ViewModels.Documents;
using Stutton.DocumentCreator.ViewModels.Navigation;

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

        public DocumentsPageViewModel(ITemplatesService templatesService, INavigationService navigationService, ITelemetryService telemetryService)
        {
            _templatesService = templatesService;
            _navigationService = navigationService;
            _telemetryService = telemetryService;
        }

        private ObservableCollection<DocumentCardViewModel> _propertyName;

        public ObservableCollection<DocumentCardViewModel> Documents
        {
            get => _propertyName;
            set => Set(ref _propertyName, value);
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
            _telemetryService.TrackPageView(Key);

            var documentResponse = await _templatesService.GetDocuments();

            if (!documentResponse.Success)
            {
                _telemetryService.TrackFailedResponse(documentResponse);
                await DialogHost.Show(new ErrorMessageDialogViewModel(documentResponse.Message));
                return;
            }

            Documents = new ObservableCollection<DocumentCardViewModel>(
                documentResponse.Value.Select(d => new DocumentCardViewModel(d, _navigationService)));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualStudio.Services.Profile;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Services.Automations;
using Stutton.DocumentCreator.Services.Documents;
using Stutton.DocumentCreator.Services.Fields;
using Stutton.DocumentCreator.Services.Telemetry;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;
using Stutton.DocumentCreator.ViewModels.Documents.DocumentTemplateSteps;
using Stutton.DocumentCreator.ViewModels.Navigation;

namespace Stutton.DocumentCreator.ViewModels.Pages
{
    public class DocumentTemplatePageViewModel : PageBase
    {
        public const string Key = "DocumentTemplatePage";
        public override string PageKey => Key;
        public override string Title => "Document Template";
        public override bool IsOnDemandPage => true;

        private readonly INavigationService _navigationService;
        private readonly IFieldFactoryService _fieldFactoryService;
        private readonly IAutomationFactoryService _automationFactoryService;
        private readonly IDocumentsService _documentsService;
        private readonly ITelemetryService _telemetryService;
        private DocumentModel _model;
        private List<IStep> _steps;

        public DocumentModel Model
        {
            get => _model;
            set => Set(ref _model, value);
        }

        public DocumentTemplatePageViewModel(INavigationService navigationService, 
            IFieldFactoryService fieldFactoryService, 
            IAutomationFactoryService automationFactoryService, 
            IDocumentsService documentsService,
            ITelemetryService telemetryService)
        {
            _navigationService = navigationService;
            _fieldFactoryService = fieldFactoryService;
            _automationFactoryService = automationFactoryService;
            _documentsService = documentsService;
            _telemetryService = telemetryService;
            IsInEditMode = true;
        }

        public List<IStep> Steps
        {
            get => _steps;
            set => Set(ref _steps, value);
        }

        #region ICommand CancelCommand

        private ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new RelayCommand(Cancel));

        private void Cancel()
        {
            _navigationService.NavigateTo(DocumentsPageViewModel.Key);
        }

        #endregion

        #region ICommand FinishCommand

        private ICommand _finishCommand;
        public ICommand FinishCommand => _finishCommand ?? (_finishCommand = new RelayCommand(async () => await Finish()));

        private async Task Finish()
        {
            var response = await _documentsService.SaveDocumentTemplate(Model);
            if (!response.Success)
            {
                _telemetryService.TrackFailedResponse(response);
                await DialogHost.Show(new ErrorMessageDialogViewModel(response.Message), MainWindow.RootDialog);
            }
            await _navigationService.NavigateTo(DocumentsPageViewModel.Key);
        }

        #endregion

        public override async Task NavigatedTo(object parameter)
        {
            _telemetryService.TrackPageView(Key);

            if (parameter == null || !(parameter is DocumentModel model))
            {
                model = new DocumentModel();
            }

            Model = model;

            Steps = new List<IStep>
            {
                new Step{Header = new StepTitleHeader{FirstLevelTitle = "Details"}, Content = new DetailsStepViewModel(Model.Details)},
                new Step{Header = new StepTitleHeader{FirstLevelTitle = "Query"}, Content = new WorkItemQueryStepViewModel(Model.Details.WorkItemQuery)},
                new Step{Header = new StepTitleHeader{FirstLevelTitle = "Fields"}, Content = new FieldsStepViewModel(Model.Fields, _fieldFactoryService)},
                new Step{Header = new StepTitleHeader{FirstLevelTitle = "Automations"}, Content = new AutomationsStepViewModel(Model.Automations, _automationFactoryService)},
                new Step{Header = new StepTitleHeader{FirstLevelTitle = "Finish"}, Content = new SummaryStepViewModel(Model)}
            };
        }
    }
}

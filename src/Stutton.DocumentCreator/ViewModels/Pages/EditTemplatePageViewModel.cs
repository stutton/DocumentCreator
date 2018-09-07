using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models.Template;
using Stutton.DocumentCreator.Services.Automations;
using Stutton.DocumentCreator.Services.Fields;
using Stutton.DocumentCreator.Services.Telemetry;
using Stutton.DocumentCreator.Services.Templates;
using Stutton.DocumentCreator.Services.Vsts;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;
using Stutton.DocumentCreator.ViewModels.Navigation;
using Stutton.DocumentCreator.ViewModels.Templates.TemplateSteps;

namespace Stutton.DocumentCreator.ViewModels.Pages
{
    public class EditTemplatePageViewModel : PageBase
    {
        public const string Key = "EditTemplatePage";
        public override string PageKey => Key;
        public override string Title => "Document Template";
        public override bool IsOnDemandPage => true;

        private readonly INavigationService _navigationService;
        private readonly IFieldTemplateFactoryService _fieldFactoryService;
        private readonly IAutomationFactoryService _automationFactoryService;
        private readonly ITemplatesService _templatesService;
        private readonly ITelemetryService _telemetryService;
        private readonly IVstsService _vstsService;
        private DocumentTemplateModel _model;
        private List<IStep> _steps;

        public DocumentTemplateModel Model
        {
            get => _model;
            set => Set(ref _model, value);
        }

        public EditTemplatePageViewModel(INavigationService navigationService, 
            IFieldTemplateFactoryService fieldFactoryService, 
            IAutomationFactoryService automationFactoryService, 
            ITemplatesService templatesService,
            ITelemetryService telemetryService,
            IVstsService vstsService)
        :base(navigationService)
        {
            _navigationService = navigationService;
            _fieldFactoryService = fieldFactoryService;
            _automationFactoryService = automationFactoryService;
            _templatesService = templatesService;
            _telemetryService = telemetryService;
            _vstsService = vstsService;
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
            var response = await _templatesService.SaveDocumentTemplate(Model);
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
            IsBusy = true;
            _telemetryService.TrackPageView(Key);

            if (parameter == null || !(parameter is DocumentTemplateModel model))
            {
                model = new DocumentTemplateModel();
            }

            Model = model;

            var fieldsVm = new FieldsStepViewModel(Model.Fields, _fieldFactoryService);
            await fieldsVm.InitializeAsync();

            var queryVm = new WorkItemQueryStepViewModel(Model.TemplateDetails.WorkItemQuery, _vstsService);
            await queryVm.Initialize();

            var automationsVm = new AutomationsStepViewModel(Model.Automations, _automationFactoryService, _telemetryService);
            await automationsVm.InitializeAsync();

            Steps = new List<IStep>
            {
                new Step{Header = new StepTitleHeader{FirstLevelTitle = "Details"}, Content = new DetailsStepViewModel(Model.TemplateDetails)},
                new Step{Header = new StepTitleHeader{FirstLevelTitle = "Query"}, Content = queryVm},
                new Step{Header = new StepTitleHeader{FirstLevelTitle = "Fields"}, Content = fieldsVm},
                new Step{Header = new StepTitleHeader{FirstLevelTitle = "Automations"}, Content = automationsVm},
                new Step{Header = new StepTitleHeader{FirstLevelTitle = "Finish"}, Content = new SummaryStepViewModel(Model)}
            };
            IsBusy = false;
        }
    }
}

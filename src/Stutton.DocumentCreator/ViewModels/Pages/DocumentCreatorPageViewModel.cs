using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services.Telemetry;
using Stutton.DocumentCreator.Services.Tfs;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;
using Stutton.DocumentCreator.ViewModels.Documents.Steps;
using Stutton.DocumentCreator.ViewModels.Navigation;

namespace Stutton.DocumentCreator.ViewModels.Pages
{
    public class DocumentCreatorPageViewModel : PageBase
    {
        private readonly INavigationService _navigationService;
        private readonly ITfsService _tfsService;
        private readonly ITelemetryService _telemetryService;
        public const string Key = "DocumentCreatorPage";
        public override string PageKey => Key;
        public override string Title => "Create Document";
        public override bool IsOnDemandPage => true;

        private DocumentModel _document;

        public DocumentModel Document
        {
            get => _document;
            set => Set(ref _document, value);
        }

        private WorkItemStepViewModel _workItemStepVm;

        public WorkItemStepViewModel WorkItemStepVm
        {
            get => _workItemStepVm;
            set => Set(ref _workItemStepVm, value);
        }

        private List<IStep> _steps;

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
            
        }

        #endregion

        #region Finish Command

        private ICommand _finishCommand;
        public ICommand FinishCommand => _finishCommand ?? (_finishCommand = new RelayCommand(async () => await Finish()));

        private async Task Finish()
        {
            
        }

        #endregion

        public DocumentCreatorPageViewModel(INavigationService navigationService, ITfsService tfsService, ITelemetryService telemetryService)
        {
            _navigationService = navigationService;
            _tfsService = tfsService;
            _telemetryService = telemetryService;

            IsInEditMode = true;
        }

        public override async Task NavigatedTo(object parameter)
        {
            IsBusy = true;
            _telemetryService.TrackPageView(Key);

            if (parameter == null || !(parameter is DocumentModel model))
            {
                await DialogHost.Show(new ErrorMessageDialogViewModel("No document selected"), MainWindow.RootDialog);
                await _navigationService.NavigateTo(DocumentsPageViewModel.Key);
                return;
            }

            Document = model;

            WorkItemStepVm = new WorkItemStepViewModel(_tfsService, Document.Details.WorkItemQuery, _telemetryService);
            await WorkItemStepVm.InitializeAsync();

            var fieldsStep = new FieldsStepViewModel(Document.Fields);
            var summaryStep = new SummaryStepViewModel(Document);

            Steps = new List<IStep>
            {
                new Step{Header = new StepTitleHeader{FirstLevelTitle = "Work Item"}, Content = WorkItemStepVm},
                new Step{Header = new StepTitleHeader{FirstLevelTitle = "Fields"}, Content = fieldsStep},
                new Step{Header = new StepTitleHeader{FirstLevelTitle = "Finish"}, Content = summaryStep}
            };
            IsBusy = false;
        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services.Telemetry;
using Stutton.DocumentCreator.Services.Vsts;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;

namespace Stutton.DocumentCreator.ViewModels.Templates.TemplateSteps
{
    public class WorkItemQueryStepViewModel : Observable
    {
        private readonly IVstsService _vstsService;
        private readonly ITelemetryService _telemetryService;

        public WorkItemQueryStepViewModel(WorkItemQueryModel model, IVstsService vstsService, ITelemetryService telemetryService)
        {
            _vstsService = vstsService ?? throw new ArgumentNullException(nameof(vstsService));
            _telemetryService = telemetryService ?? throw new ArgumentNullException(nameof(telemetryService));
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public WorkItemQueryModel Model { get; }

        private ObservableCollection<WorkItemFieldModel> _workItemFields;
        public ObservableCollection<WorkItemFieldModel> WorkItemFields
        {
            get => _workItemFields;
            private set => Set(ref _workItemFields, value);
        }

        #region ICommand AddExpressionCommand

        private ICommand _addExpressionCommand;
        
        public ICommand AddExpressionCommand => _addExpressionCommand ?? (_addExpressionCommand = new RelayCommand(AddExpression));

        private void AddExpression()
        {
            var wiqModel = new WorkItemQueryExpressionModel();
            Model.Expressions.Add(wiqModel);
            wiqModel.IsExpanded = true;
        }

        #endregion

        public async Task Initialize()
        {
            var workItemFieldsResponse = await _vstsService.GetWorkItemFields();
            if (!workItemFieldsResponse.Success)
            {
                _telemetryService.TrackFailedResponse(workItemFieldsResponse);
                await DialogHost.Show(new ErrorMessageDialogViewModel(workItemFieldsResponse.Message, _telemetryService.SessionId), MainWindow.RootDialog);
                return;
            }
            WorkItemFields = new ObservableCollection<WorkItemFieldModel>(workItemFieldsResponse.Value);
        }
    }
}
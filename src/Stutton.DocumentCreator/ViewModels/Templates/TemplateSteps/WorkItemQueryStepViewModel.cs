using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services.Vsts;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;

namespace Stutton.DocumentCreator.ViewModels.Templates.TemplateSteps
{
    public class WorkItemQueryStepViewModel : Observable
    {
        private readonly IVstsService _vstsService;
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
            Model.Expressions.Add(new WorkItemQueryExpressionModel());
        }

        #endregion

        public async Task Initialize()
        {
            var workItemFieldsResponse = await _vstsService.GetWorkItemFields();
            if (!workItemFieldsResponse.Success)
            {
                await DialogHost.Show(new ErrorMessageDialogViewModel(workItemFieldsResponse.Message), MainWindow.RootDialog);
                return;
            }
            WorkItemFields = new ObservableCollection<WorkItemFieldModel>(workItemFieldsResponse.Value);
        }

        public WorkItemQueryStepViewModel(WorkItemQueryModel model, IVstsService vstsService)
        {
            _vstsService = vstsService;
            Model = model;
        }
    }
}

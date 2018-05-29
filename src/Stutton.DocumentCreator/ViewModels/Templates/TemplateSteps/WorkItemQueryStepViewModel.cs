using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services.Tfs;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;

namespace Stutton.DocumentCreator.ViewModels.Templates.TemplateSteps
{
    public class WorkItemQueryStepViewModel : Observable
    {
        private readonly ITfsService _tfsService;
        public WorkItemQueryModel Model { get; }

        private ObservableCollection<string> _workItemFields;
        public ObservableCollection<string> WorkItemFields
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

        #region ICommand AddInValueCommand

        private ICommand _addInValueCommand;
        public ICommand AddInValueCommand => _addInValueCommand ?? (_addInValueCommand = new RelayCommand<WorkItemQueryExpressionModel>(AddInValue));

        private void AddInValue(WorkItemQueryExpressionModel expression)
        {
            expression.Values.Add(new WorkItemQueryInValue());
        }

        #endregion

        public async Task Initialize()
        {
            var workItemFieldsResponse = await _tfsService.GetWorkItemFields();
            if (!workItemFieldsResponse.Success)
            {
                await DialogHost.Show(new ErrorMessageDialogViewModel(workItemFieldsResponse.Message), MainWindow.RootDialog);
                return;
            }
            WorkItemFields = new ObservableCollection<string>(workItemFieldsResponse.Value);
        }

        public WorkItemQueryStepViewModel(WorkItemQueryModel model, ITfsService tfsService)
        {
            _tfsService = tfsService;
            Model = model;
        }
    }
}

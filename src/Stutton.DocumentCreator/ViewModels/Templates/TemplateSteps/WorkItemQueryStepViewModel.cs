using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;

namespace Stutton.DocumentCreator.ViewModels.Templates.TemplateSteps
{
    public class WorkItemQueryStepViewModel
    {
        public WorkItemQueryModel Model { get; }

        #region ICommand AddExpressionCommand

        private ICommand _addExpressionCommand;
        public ICommand AddExpressionCommand => _addExpressionCommand ?? (_addExpressionCommand = new RelayCommand(async () => await AddExpression()));

        private async Task AddExpression()
        {
            //TODO: Get work item fields
            //var workItemFields = _tfsService.GetWorkItemFieldsAsync();
            var workItemFields = new List<string> { "Field 1", "Field 2", "Field 3" };
            var expressionDialogVm = new AddWorkItemExpressionDialogViewModel(workItemFields);
            if ((bool)await DialogHost.Show(expressionDialogVm, "RootDialog"))
            {
                Model.Expressions.Add(expressionDialogVm.Model);
            }
        }

        #endregion

        public WorkItemQueryStepViewModel(WorkItemQueryModel model)
        {
            Model = model;
        }
    }
}

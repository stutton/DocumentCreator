using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.ViewModels.Dialogs
{
    public class AddWorkItemExpressionDialogViewModel : Observable
    {
        public AddWorkItemExpressionDialogViewModel(IEnumerable<string> workItemFields)
        {
            WorkItemFields = new ObservableCollection<string>(workItemFields);
        }

        public WorkItemQueryExpressionModel Model { get; } = new WorkItemQueryExpressionModel();
        
        public ObservableCollection<string> WorkItemFields { get; }

        #region AddInValue Command

        private ICommand _addInValueCommand;
        public ICommand AddInValueCommand => _addInValueCommand ?? (_addInValueCommand = new RelayCommand(AddInValue));

        private void AddInValue()
        {
            Model.Values.Add(string.Empty);
        }

        #endregion

        private string _value;

        public string Value
        {
            get => _value;
            set 
            {
                if (Set(ref _value, value))
                {
                    Model.Values.Clear();
                    Model.Values.Add(value);
                }
            }
        }
    }
}

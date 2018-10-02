using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services.Telemetry;
using Stutton.DocumentCreator.Services.Vsts;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;

namespace Stutton.DocumentCreator.ViewModels.Documents.Steps
{
    public class WorkItemStepViewModel : Observable, IValidatable
    {
        private readonly IVstsService _vstsService;
        private readonly WorkItemQueryModel _query;
        private readonly ITelemetryService _telemetryService;

        private ObservableCollection<IWorkItem> _workItems;
        public ObservableCollection<IWorkItem> WorkItems
        {
            get => _workItems;
            set => Set(ref _workItems, value);
        }

        private IWorkItem _selectedWorkItem;
        public IWorkItem SelectedWorkItem
        {
            get => _selectedWorkItem;
            set
            {
                var selectedItem = _selectedWorkItem;
                if (Set(ref _selectedWorkItem, value))
                {
                    if (selectedItem != null)
                    {
                        selectedItem.Selected = false;
                    }

                    if (_selectedWorkItem != null)
                    {
                        _selectedWorkItem.Selected = true;
                    }
                }
            }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }

        #region WorkItemSelected Command

        private ICommand _workItemSelectedCommand;
        public ICommand WorkItemSelectedCommand => _workItemSelectedCommand ?? (_workItemSelectedCommand = new RelayCommand<IWorkItem>(SelectWorkItem));

        private void SelectWorkItem(IWorkItem workItem)
        {
            SelectedWorkItem = workItem;
        }

        #endregion

        #region OpenWorkItemUrl Command

        private ICommand _openWorkItemUrlCommand;
        public ICommand OpenWorkItemUrlCommand => _openWorkItemUrlCommand ?? (_openWorkItemUrlCommand = new RelayCommand<IWorkItem>(OpenWorkItemUrl));

        private void OpenWorkItemUrl(IWorkItem workItem)
        {
            
        }

        #endregion

        public WorkItemStepViewModel(IVstsService vstsService, WorkItemQueryModel query, ITelemetryService telemetryService)
        {
            _vstsService = vstsService;
            _query = query;
            _telemetryService = telemetryService;
        }

        public async Task InitializeAsync()
        {
            IsBusy = true;

            var response = await _vstsService.GetWorkItemsAsync(_query);
            if (!response.Success)
            {
                _telemetryService.TrackFailedResponse(response);
                var sessionId = _telemetryService.SessionId;
                await DialogHost.Show(new ErrorMessageDialogViewModel(response.Message, sessionId), MainWindow.RootDialog);
                IsBusy = false;
                return;
            }

            WorkItems = new ObservableCollection<IWorkItem>(response.Value);

            IsBusy = false;
        }

        public bool HasValidationErrors => _validationErrors.Any();
        private readonly ObservableCollection<ValidationError> _validationErrors = new ObservableCollection<ValidationError>();
        public IEnumerable<ValidationError> ValidationErrors => _validationErrors;
        public bool Validate()
        {
            _validationErrors.Clear();

            if (SelectedWorkItem == null)
            {
                _validationErrors.Add(new ValidationError("No work item selected"));
                return false;
            }
            return true;
        }
    }
}

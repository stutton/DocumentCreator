using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Properties;
using Stutton.DocumentCreator.Services.Settings;
using Stutton.DocumentCreator.Services.Tfs;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;
using Stutton.DocumentCreator.ViewModels.WorkItems;
using DialogHost = MaterialDesignThemes.Wpf.DialogHost;

namespace Stutton.DocumentCreator.ViewModels.Pages
{
    public class WorkItemsPageViewModel : PageBase
    {
        private readonly ITfsService _tfsService;
        private readonly ISettingsService _settingsService;
        public const string Key = "WorkItemsPage";
        public const int Order = 2;

        public WorkItemsPageViewModel(ITfsService tfsService, ISettingsService settingsService)
        {
            _tfsService = tfsService;
            _settingsService = settingsService;
            ToolBar.Refresh.IsShown = true;
            ToolBar.Refresh.Command = new RelayCommand(async () => await Refresh());
        }
        
        public override string PageKey => Key;
        public override int PageOrder => Order;
        public override string Title => Resources.WorkItemsPage_WorkItems;
        public override bool IsOnDemandPage => false;

        private ObservableCollection<IWorkItem> _workItems;

        public ObservableCollection<IWorkItem> WorkItems
        {
            get => _workItems;
            private set => Set(ref _workItems, value);
        }

        public override async Task NavigatedTo(object parameter)
        {
            await Refresh();
        }

        private async Task Refresh()
        {
            try
            {
                IsBusy = true;
                var settingsResponse = await _settingsService.GetSettings();

                if (!settingsResponse.Success)
                {
                    await DialogHost.Show(new ErrorMessageDialogViewModel(settingsResponse.Message), "RootDialog");
                    return;
                }

                var settings = settingsResponse.Value;

                var tfsResponse = await _tfsService.GetWorkItemsAsync(settings.WorkItemQuery);

                if (!tfsResponse.Success)
                {
                    await DialogHost.Show(new ErrorMessageDialogViewModel(tfsResponse.Message));
                    return;
                }

                WorkItems = new ObservableCollection<IWorkItem>(tfsResponse.Value);
            }
            catch (Exception ex)
            {
                await DialogHost.Show(
                    new ErrorMessageDialogViewModel($"Unknown error getting work items: {ex.Message}"), "RootDialog");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models.Settings;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Properties;
using Stutton.DocumentCreator.Services.Settings;
using Stutton.DocumentCreator.Services.Tfs;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;

namespace Stutton.DocumentCreator.ViewModels.Pages
{
    public class SettingsPageViewModel : PageBase
    {
        private readonly ISettingsService _settingsService;
        private readonly ITfsService _tfsService;
        public const string Key = "SettingsPage";
        public const int Order = 2;
        public override string PageKey => Key;
        public override int PageOrder => Order;
        public override string Title => Resources.SettingsPage_Settings;
        public override bool IsOnDemandPage => false;

        public SettingsPageViewModel(ISettingsService settingsService, ITfsService tfsService)
        {
            _settingsService = settingsService;
            _tfsService = tfsService;
        }

        #region Save Command

        private ICommand _saveCommand;
        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(async () => await SaveAsync()));

        private async Task SaveAsync()
        {
            var response = await _settingsService.SaveSettings(Settings);
            if (!response.Success)
            {
                await DialogHost.Show(new ErrorMessageDialogViewModel($"Failed to save settings: {response.Message}"));
            }
        }

        #endregion

        #region AddExpression Command

        private ICommand _addExpressionCommand;
        public ICommand AddExpressionCommand => _addExpressionCommand ?? (_addExpressionCommand = new RelayCommand(async () => await AddExpression()));

        private async Task AddExpression()
        {
            //TODO: Get work item fields
            //var workItemFields = _tfsService.GetWorkItemFieldsAsync();
            var workItemFields = new List<string> {"Field 1", "Field 2", "Field 3"};
            var expressionDialogVm = new AddWorkItemExpressionDialogViewModel(workItemFields);
            if((bool) await DialogHost.Show(expressionDialogVm, "RootDialog"))
            {
                Settings.WorkItemQuery.Expressions.Add(expressionDialogVm.Model);
            }
        }

        #endregion

        #region DeleteExpression Command

        private ICommand _deleteExpressionCommand;
        public ICommand DeleteExpressionCommand => _deleteExpressionCommand ?? (_deleteExpressionCommand = new RelayCommand<WorkItemQueryExpressionModel>(DeleteExpression));

        private void DeleteExpression(WorkItemQueryExpressionModel commandParameter)
        {
            
        }

        #endregion

        public override async Task NavigatedTo(object parameter)
        {
            var settingsResponse = await _settingsService.GetSettings();
            if (!settingsResponse.Success)
            {
                await DialogHost.Show(new ErrorMessageDialogViewModel(settingsResponse.Message), "RootDialog");
                return;
            }

            Settings = settingsResponse.Value;
        }

        private SettingsModel _settings;

        public SettingsModel Settings
        {
            get => _settings;
            set => Set(ref _settings, value);
        }
    }
}

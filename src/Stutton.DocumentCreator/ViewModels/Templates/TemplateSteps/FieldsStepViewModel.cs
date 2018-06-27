﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Fields;
using Stutton.DocumentCreator.Services.Fields;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;

namespace Stutton.DocumentCreator.ViewModels.Templates.TemplateSteps
{
    public class FieldsStepViewModel : Observable
    {
        private readonly IFieldTemplateFactoryService _fieldFactoryService;

        public FieldsStepViewModel(ObservableCollection<IFieldTemplate> fields, IFieldTemplateFactoryService fieldFactoryService)
        {
            _fieldFactoryService = fieldFactoryService;
            Fields = fields;
            foreach (var field in Fields)
            {
                field.RequestDeleteMe += HandleFieldRequestDeleteMe;
            }
        }

        public ObservableCollection<IFieldTemplate> Fields { get; }

        private ObservableDictionary<string, Type> _availableFieldTypes;
        public ObservableDictionary<string, Type> AvailableFieldTypes
        {
            get => _availableFieldTypes;
            set => Set(ref _availableFieldTypes, value);
        }

        private Type _selectedType;

        public Type SelectedType
        {
            get => _selectedType;
            set
            {
                if (Set(ref _selectedType, value))
                {
                    if (AddFieldCommand is RelayCommand cmd)
                    {
                        cmd.RaiseCanExecuteChanged();
                    }
                }
            }
        }

        #region ICommand AddFieldCommand

        private ICommand _addFieldCommand;

        public ICommand AddFieldCommand =>
            _addFieldCommand ?? (_addFieldCommand = new RelayCommand(async () => await AddField(), () => SelectedType != null));

        private async Task AddField()
        {
            if (SelectedType == null)
            {
                return;
            }
            var response = await _fieldFactoryService.CreateField(SelectedType);
            if (!response.Success)
            {
                await DialogHost.Show(response.Message, MainWindow.RootDialog);
                return;
            }

            response.Value.RequestDeleteMe += HandleFieldRequestDeleteMe;
            Fields.Add(response.Value);
        }

        #endregion

        public async Task InitializeAsync()
        {
            var response = _fieldFactoryService.GetAllFieldKeys();
            if (!response.Success)
            {
                await DialogHost.Show(new ErrorMessageDialogViewModel(response.Message), MainWindow.RootDialog);
                return;
            }
            AvailableFieldTypes = new ObservableDictionary<string, Type>(response.Value);
        }

        private void HandleFieldRequestDeleteMe(object sender, IFieldTemplate field)
        {
            field.RequestDeleteMe -= HandleFieldRequestDeleteMe;
            Fields.Remove(field);
        }
    }
}
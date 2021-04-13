﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models.Document;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Models.Template;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services.Document;
using Stutton.DocumentCreator.Services.Telemetry;
using Stutton.DocumentCreator.Services.Vsts;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;
using Stutton.DocumentCreator.ViewModels.Documents.Steps;
using Stutton.DocumentCreator.ViewModels.Navigation;

namespace Stutton.DocumentCreator.ViewModels.Pages
{
    public class DocumentCreatorPageViewModel : PageBase
    {
        private readonly INavigationService _navigationService;
        private readonly IVstsService _vstsService;
        private readonly ITelemetryService _telemetryService;
        private readonly IDocumentService _documentService;
        private readonly ISnackbarMessageQueue _messageQueue;
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
        public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new RelayCommand(async () => await Cancel()));

        private async Task Cancel()
        {
            await _navigationService.GoBack();
        }

        #endregion

        #region Finish Command

        private ICommand _finishCommand;
        public ICommand FinishCommand => _finishCommand ?? (_finishCommand = new RelayCommand(async () => await Finish()));

        private async Task Finish()
        {
            try
            {
                IsBusy = true;

                var result = await _documentService.CreateDocumentAsync(Document, WorkItemStepVm.SelectedWorkItem);
                if (!result.Success)
                {
                    _telemetryService.TrackFailedResponse(result);
                    await DialogHost.Show(new ErrorMessageDialogViewModel(result.Message, _telemetryService.SessionId), MainWindow.RootDialog);
                    return;
                }

                var documentPath = result.Value;

                var automationResponse = await
                    _documentService.ExecuteAutomationsAsync(Document, WorkItemStepVm.SelectedWorkItem, documentPath);

                if (!automationResponse.Success)
                {
                    _telemetryService.TrackFailedResponse(automationResponse);
                    await DialogHost.Show(new ErrorMessageDialogViewModel(automationResponse.Message, _telemetryService.SessionId), MainWindow.RootDialog);
                    return;
                }

                File.Delete(documentPath);

                await DialogHost.Show(new SuccessMessageDialogViewModel("Document created and automations run"),
                    MainWindow.RootDialog);

                await _navigationService.NavigateTo(DocumentsPageViewModel.Key);
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region Save Command

        private ICommand _saveCommand;
        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(async () => await Save()));

        private async Task Save()
        {
            Document.SelectedWorkItemId = WorkItemStepVm.SelectedWorkItem?.Id;
            var saveName = Document.FileName;
            if (string.IsNullOrEmpty(saveName))
            {
                var dialogVm = new StringPromptDialogViewModel {PromptMessage = "Pick a name for the save file"};
                if (!(bool) await DialogHost.Show(dialogVm))
                {
                    return;
                }
                saveName = dialogVm.InputString;
            }

            var response = await _documentService.SaveDocumentAsync(Document, WorkItemStepVm.SelectedWorkItem, saveName);
            if (!response.Success)
            {
                _telemetryService.TrackFailedResponse(response);
                await DialogHost.Show(new ErrorMessageDialogViewModel(response.Message, _telemetryService.SessionId));
                return;
            }
            _messageQueue.Enqueue("Document saved");
        }

        #endregion

        public DocumentCreatorPageViewModel(INavigationService navigationService, 
                                            IVstsService vstsService, 
                                            ITelemetryService telemetryService, 
                                            IDocumentService documentService, 
                                            ISnackbarMessageQueue messageQueue)
        :base(navigationService)
        {
            _navigationService = navigationService;
            _vstsService = vstsService;
            _telemetryService = telemetryService;
            _documentService = documentService;
            _messageQueue = messageQueue;

            IsInEditMode = true;
            ToolBar.Save.IsShown = true;
            ToolBar.Save.Command = SaveCommand;
        }

        public override async Task NavigatedTo(object parameter)
        {
            IsBusy = true;
            _telemetryService.TrackPageView(Key);

            if (parameter == null)
            {
                await DialogHost.Show(new ErrorMessageDialogViewModel("No document selected"), MainWindow.RootDialog);
                await _navigationService.NavigateTo(DocumentsPageViewModel.Key);
                return;
            }

            if (parameter is DocumentTemplateModel template)
            {
                Document = template.GetNewDocument();
            }
            else if (parameter is DocumentModel doc)
            {
                Document = doc;
            }
            else
            {
                _telemetryService.TrackException(new InvalidOperationException($"Unexpected document type: {parameter.GetType().FullName}"));
                await DialogHost.Show(new ErrorMessageDialogViewModel("Unexpected document type", _telemetryService.SessionId), MainWindow.RootDialog);
                await _navigationService.NavigateTo(DocumentsPageViewModel.Key);
                return;
            }

            WorkItemStepVm = new WorkItemStepViewModel(_vstsService, Document.Details.WorkItemQuery, _telemetryService);
            await WorkItemStepVm.InitializeAsync();
            WorkItemStepVm.SelectedWorkItem =
                WorkItemStepVm.WorkItems.FirstOrDefault(w => w.Id == Document.SelectedWorkItemId);

            var fieldsStep = new FieldsStepViewModel(Document.Fields);
            var summaryStep = new SummaryStepViewModel(Document);

            Steps = new List<IStep>
            {
                new ValidatableStep{Header = new StepTitleHeader{FirstLevelTitle = "Work Item"}, Content = WorkItemStepVm},
                new Step{Header = new StepTitleHeader{FirstLevelTitle = "Fields"}, Content = fieldsStep},
                new Step{Header = new StepTitleHeader{FirstLevelTitle = "Finish"}, Content = summaryStep}
            };
            IsBusy = false;
        }
    }
}

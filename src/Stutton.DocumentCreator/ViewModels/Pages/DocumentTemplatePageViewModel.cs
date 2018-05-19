using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignExtensions.Model;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Documents.DocumentTemplateSteps;
using Stutton.DocumentCreator.ViewModels.Navigation;

namespace Stutton.DocumentCreator.ViewModels.Pages
{
    public class DocumentTemplatePageViewModel : PageBase
    {
        public const string Key = "DocumentTemplatePage";
        public override string PageKey => Key;
        public override string Title => "Document Template";
        public override bool IsOnDemandPage => true;

        private readonly INavigationService _navigationService;
        private DocumentModel _model;
        private List<IStep> _steps;

        public DocumentModel Model
        {
            get => _model;
            set => Set(ref _model, value);
        }

        public DocumentTemplatePageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            IsInEditMode = true;
        }

        public List<IStep> Steps
        {
            get => _steps;
            set => Set(ref _steps, value);
        }

        #region ICommand CancelCommand

        private ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new RelayCommand(Cancel));

        private void Cancel()
        {
            _navigationService.NavigateTo(DocumentsPageViewModel.Key);
        }

        #endregion

        public override Task NavigatedTo(object parameter)
        {
            if (parameter == null || !(parameter is DocumentModel model))
            {
                model = new DocumentModel();
            }

            Model = model;

            Steps = new List<IStep>
            {
                new Step{Header = new StepTitleHeader{FirstLevelTitle = "Details"}, Content = new DetailsStepViewModel(Model.Details)},
                new Step{Header = new StepTitleHeader{FirstLevelTitle = "Fields"}, Content = new FieldsStepViewModel(Model.Fields)},
                new Step{Header = new StepTitleHeader{FirstLevelTitle = "Automations"}, Content = new AutomationsStepViewModel(Model.Automations)},
                new Step{Header = new StepTitleHeader{FirstLevelTitle = "Finish"}, Content = new SummaryStepViewModel(Model)}
            };

            return Task.FromResult(true);
        }
    }
}

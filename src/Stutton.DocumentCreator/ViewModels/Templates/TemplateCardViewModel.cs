using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models.Template;
using Stutton.DocumentCreator.Services.Telemetry;
using Stutton.DocumentCreator.Services.Templates;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;
using Stutton.DocumentCreator.ViewModels.Navigation;
using Stutton.DocumentCreator.ViewModels.Pages;

namespace Stutton.DocumentCreator.ViewModels.Templates
{
    public class TemplateCardViewModel : Observable
    {
        private readonly INavigationService _navigator;
        private readonly ITemplatesService _templatesService;
        private readonly ITelemetryService _telemetryService;

        public event EventHandler<EventArgs> RequestDeleteMe; 

        public TemplateCardViewModel(DocumentTemplateModel model, INavigationService navigator, ITemplatesService templatesService, ITelemetryService telemetryService)
        {
            _navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
            _templatesService = templatesService;
            _telemetryService = telemetryService;
            Model = model;
        }

        public DocumentTemplateModel Model { get; }

        #region ICommand SelectCommand

        private ICommand _selectCommand;
        public ICommand SelectCommand => _selectCommand ?? (_selectCommand = new RelayCommand(Select));

        private void Select()
        {
            _navigator.NavigateTo(DocumentCreatorPageViewModel.Key, Model);
        }

        #endregion

        #region ICommand EditCommand

        private ICommand _editCommand;
        public ICommand EditCommand => _editCommand ?? (_editCommand = new RelayCommand(Edit));

        private void Edit()
        {
            _navigator.NavigateTo(EditTemplatePageViewModel.Key, Model);
        }

        #endregion

        #region Share Command

        private ICommand _shareCommand;
        public ICommand ShareCommand => _shareCommand ?? (_shareCommand = new RelayCommand(async () => await Share()));

        private async Task Share()
        {
            var dialogVm = new MaterialSaveFileDialogViewModel();
            if ((bool)await DialogHost.Show(dialogVm, MainWindow.RootDialog))
            {
                var response = await _templatesService.ShareDocumentTemplate(Model, dialogVm.SelectedFile);
                if (!response.Success)
                {
                    _telemetryService.TrackFailedResponse(response);
                    await DialogHost.Show(new ErrorMessageDialogViewModel(response.Message));
                }
            }
        }

        #endregion

        #region Delete Command

        private ICommand _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new RelayCommand(Delete));

        private void Delete()
        {
            RequestDeleteMe?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}

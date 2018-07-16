using System;
using System.Windows.Input;
using Stutton.DocumentCreator.Models.Template;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Navigation;
using Stutton.DocumentCreator.ViewModels.Pages;

namespace Stutton.DocumentCreator.ViewModels.Templates
{
    public class TemplateCardViewModel : Observable
    {
        private readonly INavigationService _navigator;

        public event EventHandler<EventArgs> RequestDeleteMe; 

        public TemplateCardViewModel(DocumentTemplateModel model, INavigationService navigator)
        {
            _navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
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

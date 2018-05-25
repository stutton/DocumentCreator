using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Navigation;
using Stutton.DocumentCreator.ViewModels.Pages;

namespace Stutton.DocumentCreator.ViewModels.Documents
{
    public class DocumentCardViewModel : Observable
    {
        private readonly INavigationService _navigator;

        public DocumentCardViewModel(DocumentModel model, INavigationService navigator)
        {
            _navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
            Model = model;
        }

        public DocumentModel Model { get; }

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
    }
}

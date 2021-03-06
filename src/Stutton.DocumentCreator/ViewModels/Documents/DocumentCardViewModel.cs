﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Stutton.DocumentCreator.Models.Document;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Navigation;
using Stutton.DocumentCreator.ViewModels.Pages;

namespace Stutton.DocumentCreator.ViewModels.Documents
{
    public class DocumentCardViewModel : Observable
    {
        private readonly INavigationService _navigator;

        public event EventHandler<EventArgs> RequestDeleteMe;

        public DocumentCardViewModel(DocumentModel model, INavigationService navigationService)
        {
            _navigator = navigationService;
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

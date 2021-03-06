﻿using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Services.Image;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;

namespace Stutton.DocumentCreator.Fields.List.Document
{
    public class ListFieldStepModel : Observable
    {
        private readonly IImageService _imageService;
        private readonly IContext _context;
        public event EventHandler<MoveListFieldStepEventArgs> RequestMove;
        public event EventHandler<EventArgs> RequestDeleteMe;

        public ListFieldStepModel(IImageService imageService, IContext context)
        {
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        private int _index;
        public int Index
        {
            get => _index;
            set => Set(ref _index, value);
        }

        private string _text;
        public string Text
        {
            get => _text;
            set => Set(ref _text, value);
        }

        private bool _hasImage;

        public bool HasImage
        {
            get => _hasImage;
            private set => Set(ref _hasImage, value);
        }

        private BitmapSource _image;
        public BitmapSource Image
        {
            get => _image;
            set => Set(ref _image, value);
        }

        #region MoveUp Command

        private ICommand _moveUpCommand;
        public ICommand MoveUpCommand => _moveUpCommand ?? (_moveUpCommand = new RelayCommand(MoveUp));

        private void MoveUp()
        {
            RequestMove?.Invoke(this, new MoveListFieldStepEventArgs(MoveListFieldStepEventArgs.MoveDirection.Up));
        }

        #endregion

        #region MoveDown Command

        private ICommand _moveDownCommand;
        public ICommand MoveDownCommand => _moveDownCommand ?? (_moveDownCommand = new RelayCommand(MoveDown));

        private void MoveDown()
        {
            RequestMove?.Invoke(this, new MoveListFieldStepEventArgs(MoveListFieldStepEventArgs.MoveDirection.Down));
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

        #region AddImage Command

        private ICommand _addImageCommand;
        public ICommand AddImageCommand => _addImageCommand ?? (_addImageCommand = new RelayCommand(async () => await AddImage()));

        private async Task AddImage()
        {
            HasImage = true;

            var dialogVm = new AddImageDialogViewModel();
            var result = (AddImageDialogResult) await DialogHost.Show(dialogVm, MainWindow.RootDialog);
            
            switch (result)
            {
                case AddImageDialogResult.Window:
                    Image = await GetWindowScreenShot();
                    break;
                case AddImageDialogResult.Clipboard:
                    Image = await GetImageFromClipboard();
                    break;
                case AddImageDialogResult.Cancel:
                    return;
                default:
                    return;
            }
        }

        #endregion

        private async Task<BitmapSource> GetWindowScreenShot()
        {
            var response = TakeWindowScreenShot();
            if (!response.Success)
            {
                var dialogVm = new ErrorMessageDialogViewModel(response.Message);
                await DialogHost.Show(dialogVm);
                return null;
            }

            return response.Value;
        }

        private IResponse<BitmapSource> TakeWindowScreenShot()
        {
            if (_context.IsInvokeRequired)
            {
                IResponse<BitmapSource> response = Response<BitmapSource>.FromFailure("Failed to take screenshot");
                _context.Invoke(() => response = TakeWindowScreenShot());
                return response;
            }

            return _imageService.GetWindowCapture();
        }

        private async Task<BitmapSource> GetImageFromClipboard()
        {
            var response = TakeImageFromClipboard();
            if (!response.Success)
            {
                var dialogVm = new ErrorMessageDialogViewModel(response.Message);
                await DialogHost.Show(dialogVm);
                return null;
            }

            return response.Value;
        }

        private IResponse<BitmapSource> TakeImageFromClipboard()
        {
            if (_context.IsInvokeRequired)
            {
                IResponse<BitmapSource> response = Response<BitmapSource>.FromFailure("Failed to take image from clipboard");
                _context.Invoke(() => response = TakeImageFromClipboard());
                return response;
            }

            return _imageService.GetImageFromClipboard();
        }
    }
}

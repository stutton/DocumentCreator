using System;
using System.Runtime.Serialization;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.List.Document
{
    [DataContract(Name = "ListFieldStep")]
    public class ListFieldStepModel : Observable
    {
        public event EventHandler<MoveListFieldStepEventArgs> RequestMove;
        public event EventHandler<EventArgs> RequestDeleteMe;

        public ListFieldStepModel()
        {
            
        }

        private int _index;
        [DataMember]
        public int Index
        {
            get => _index;
            set => Set(ref _index, value);
        }

        private string _text;
        [DataMember]
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
        //TODO: Add bitmap de/serialization
        public BitmapSource Image
        {
            get => _image;
            set => Set(ref _image, value);
        }

        #region MoveUp Command

        private ICommand _moveUpCommand;
        [IgnoreDataMember]
        public ICommand MoveUpCommand => _moveUpCommand ?? (_moveUpCommand = new RelayCommand(MoveUp));

        private void MoveUp()
        {
            RequestMove?.Invoke(this, new MoveListFieldStepEventArgs(MoveListFieldStepEventArgs.MoveDirection.Up));
        }

        #endregion

        #region MoveDown Command

        private ICommand _moveDownCommand;
        [IgnoreDataMember]
        public ICommand MoveDownCommand => _moveDownCommand ?? (_moveDownCommand = new RelayCommand(MoveDown));

        private void MoveDown()
        {
            RequestMove?.Invoke(this, new MoveListFieldStepEventArgs(MoveListFieldStepEventArgs.MoveDirection.Down));
        }

        #endregion

        #region Delete Command

        private ICommand _deleteCommand;
        [IgnoreDataMember]
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new RelayCommand(Delete));

        private void Delete()
        {
            RequestDeleteMe?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region AddImage Command

        private ICommand _addImageCommand;
        [IgnoreDataMember]
        public ICommand AddImageCommand => _addImageCommand ?? (_addImageCommand = new RelayCommand(AddImage));

        private void AddImage()
        {
            HasImage = true;
        }

        #endregion
    }
}

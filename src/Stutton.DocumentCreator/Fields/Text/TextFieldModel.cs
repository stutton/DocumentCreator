using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.Text
{
    [DataContract(Name = "TextField")]
    public class TextFieldModel : Observable, IField
    {
        public const string Key = "TextField";
        public event EventHandler<IField> RequestDeleteMe; 

        private string _replaceWithText;
        private string _textToReplace;

        [DataMember]
        public string ReplaceWithText
        {
            get => _replaceWithText;
            set
            {
                if (Set(ref _replaceWithText, value))
                {
                    RaisePropertyChanged(nameof(Description));
                }
            }
        }

        public string Description => $"Replace '{TextToReplace}' with '{ReplaceWithText}'";

        public string TypeDisplayName => "Text";
        public string FieldKey => Key;

        [DataMember]
        public string TextToReplace
        {
            get => _textToReplace;
            set
            {
                if (Set(ref _textToReplace, value))
                {
                    RaisePropertyChanged(nameof(Description));
                }
            }
        }

        #region Delete Command

        private ICommand _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new RelayCommand(Delete));

        private void Delete()
        {
            RequestDeleteMe?.Invoke(this, this);
        }

        #endregion

        public async Task<IResponse> ModifyDocument(WordprocessingDocument document, IWorkItem workItem, IServiceResolver serviceResolver)
        {
            try
            {
                await Task.Run(() => TextReplacer.SearchAndReplace(document, TextToReplace, ReplaceWithText, false));
                return Response.FromSuccess();
            }
            catch (Exception ex)
            {
                return Response.FromException("Error replacing text", ex);
            }
        }
    }
}
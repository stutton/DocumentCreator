using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.Text.Template
{
    [DataContract(Name = "TextField")]
    public class TextFieldTemplateModel : Observable, IFieldTemplate
    {
        public const string Key = "TextField";
        public event EventHandler<IFieldTemplate> RequestDeleteMe; 
        
        private string _textToReplace;

        public string Description => $"Propmt to replace '{TextToReplace}'";

        public string TypeDisplayName => "Text";
        public string FieldKey => Key;

        private string _name;

        [DataMember]
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

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

        //public async Task<IResponse> ModifyDocument(WordprocessingDocument document, IWorkItem workItem, IServiceResolver serviceResolver)
        //{
        //    try
        //    {
        //        await Task.Run(() => TextReplacer.SearchAndReplace(document, TextToReplace, ReplaceWithText, false));
        //        return Response.FromSuccess();
        //    }
        //    catch (Exception ex)
        //    {
        //        return Response.FromException("Error replacing text", ex);
        //    }
        //}
    }
}
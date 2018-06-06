using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.List
{
    public class ListFieldModel : Observable, IField
    {
        public const string Key = "ListField";
        public string Description => "A list of text and images created during document creation";
        public string TypeDisplayName => "List";
        public string FieldKey => Key;

        private string _textToReplace;
        public string TextToReplace
        {
            get => _textToReplace;
            set => Set(ref _textToReplace, value);
        }

        public event EventHandler<IField> RequestDeleteMe;

        public Task<IResponse> ModifyDocument(WordprocessingDocument document, IWorkItem workItem, IServiceResolver serviceResolver)
        {
            throw new NotImplementedException();
        }
    }
}

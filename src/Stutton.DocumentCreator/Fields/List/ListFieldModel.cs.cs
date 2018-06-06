using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.List
{
    [DataContract(Name = "ListField")]
    public class ListFieldModel : Observable, IField
    {
        public const string Key = "ListField";
        public string Description => "A list of text and images created during document creation";
        public string TypeDisplayName => "List";
        public string FieldKey => Key;

        private string _textToReplace;

        [DataMember]
        public string TextToReplace
        {
            get => _textToReplace;
            set => Set(ref _textToReplace, value);
        }

        public event EventHandler<IField> RequestDeleteMe;

        [IgnoreDataMember]
        public ObservableCollection<ListStepModel> List { get; } = new ObservableCollection<ListStepModel>();

        public Task<IResponse> ModifyDocument(WordprocessingDocument document, IWorkItem workItem, IServiceResolver serviceResolver)
        {
            throw new NotImplementedException();
        }
    }
}

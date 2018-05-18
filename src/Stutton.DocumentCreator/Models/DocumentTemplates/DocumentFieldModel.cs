using System.Runtime.Serialization;
using Stutton.DocumentCreator.Models.DocumentTemplates.FieldTypes;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.DocumentTemplates
{
    [DataContract(Name = "DocumentField")]
    public class DocumentFieldModel : Observable
    {
        private string _name;

        [DataMember]
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private IDocumentFieldType _type;

        [DataMember]
        public IDocumentFieldType Type
        {
            get => _type;
            set => Set(ref _type, value);
        }
    }
}

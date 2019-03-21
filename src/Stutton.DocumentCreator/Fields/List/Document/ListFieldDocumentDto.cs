using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Fields.List.Document
{
    public sealed class ListFieldDocumentDto : FieldDocumentDtoBase
    {
        public override string Name { get; set; }
        public bool UsePlaceholder { get; set; }
        public string PlaceholderText { get; set; }
        public List<ListFieldStepDto> Steps { get; set; }
    }
}

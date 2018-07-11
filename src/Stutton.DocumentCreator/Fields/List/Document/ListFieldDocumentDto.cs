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
        public List<ListFieldStepDto> Steps { get; set; }
    }
}

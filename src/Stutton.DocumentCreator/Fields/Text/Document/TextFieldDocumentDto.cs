using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Fields.Text.Document
{
    public class TextFieldDocumentDto : FieldDocumentDtoBase
    {
        public override string Name { get; set; }
        public string TextToReplace { get; set; }
        public string ReplaceWithText { get; set; }
    }
}

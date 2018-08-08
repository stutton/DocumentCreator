using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Fields.Date.Document
{
    public class DateFieldDocumentDto : FieldDocumentDtoBase
    {
        public override string Name { get; set; }
        public string TextToReplace { get; set; }
    }
}

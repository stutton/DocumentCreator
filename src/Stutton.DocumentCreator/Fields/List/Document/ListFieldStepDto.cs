using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Fields.List.Document
{
    public sealed class ListFieldStepDto
    {
        public int Index { get; set; }
        public string Text { get; set; }
        public bool HasImage { get; set; }
        public byte[] Image { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields
{
    public abstract class FieldDocumentModelBase : Observable
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public abstract string Description { get; }
        public abstract string FieldKey { get; }
        public abstract Task<IResponse> ModifyDocument(WordprocessingDocument document, IWorkItem workItem);
    }
}

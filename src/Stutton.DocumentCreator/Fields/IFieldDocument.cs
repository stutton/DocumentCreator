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
    public interface IFieldDocument
    {
        event EventHandler<IFieldTemplate> RequestDeleteMe;

        string Name { get; }
        string Description { get; }
        string TypeDisplayName { get; }
        string FieldKey { get; }

        Task<IResponse> ModifyDocument(WordprocessingDocument document, IWorkItem workItem);
    }
}

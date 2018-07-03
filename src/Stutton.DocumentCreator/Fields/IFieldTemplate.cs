using System;
using System.Threading.Tasks;
using System.Windows.Input;
using DocumentFormat.OpenXml.Packaging;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields
{
    public interface IFieldTemplate
    {
        event EventHandler<IFieldTemplate> RequestDeleteMe;

        Type DtoType { get; }
        string Name { get; }
        string Description { get; }
        string TypeDisplayName { get; }
        string FieldKey { get; }
        ICommand DeleteCommand { get; }
        IFieldDocument GetDocumentField();
    }
}
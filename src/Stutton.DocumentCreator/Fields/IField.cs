using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using Stutton.DocumentCreator.Services;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields
{
    public interface IField
    {
        string Description { get; }
        string TypeDisplayName { get; }
        string FieldKey { get; }
        string TextToReplace { get; set; }
        Task<IResponse> ModifyDocument(WordprocessingDocument document, IServiceResolver serviceResolver);
    }
}
using System.Threading.Tasks;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields
{
    public interface IField
    {
        string Description { get; }
        string TypeDisplayName { get; }
        string FieldKey { get; }
        string TextToReplace { get; set; }
        Task<IResponse<string>> GetReplaceWithText();
    }
}
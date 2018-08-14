using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Shared
{
    public interface IRequiresInitialization
    {
        Task<IResponse> Initialize();
    }
}

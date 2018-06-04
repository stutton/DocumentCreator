using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Shared;
using Unity;

namespace Stutton.DocumentCreator.Services
{
    public class ServiceResolver : IServiceResolver
    {
        private readonly IUnityContainer _container;

        public ServiceResolver(IUnityContainer container)
        {
            _container = container;
        }
        public IResponse<T> Resolve<T>()
        {
            try
            {
                var result = _container.Resolve<T>();
                if (result == null)
                {
                    return Response<T>.FromFailure($"Unable to resolve '{typeof(T).Name}'");
                }

                return Response<T>.FromSuccess(result);
            }
            catch (Exception ex)
            {
                return Response<T>.FromException($"Failed to resolve '{typeof(T).Name}'", ex);
            }
        }
    }
}

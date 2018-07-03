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
            _container = container ?? throw new ArgumentNullException(nameof(container));
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

        public IResponse<object> Resolve(Type type)
        {
            try
            {
                var result = _container.Resolve(type);
                if (result == null)
                {
                    return Response<object>.FromFailure($"Unable to resolve '{type.Name}'");
                }

                return Response<object>.FromSuccess(result);
            }
            catch (Exception ex)
            {
                return Response<object>.FromException($"Failed to resolve '{type.Name}'", ex);
            }
        }

        public IResponse<bool> IsRegistered<T>()
        {
            try
            {
                return Response<bool>.FromSuccess(_container.IsRegistered<T>());
            }
            catch (Exception ex)
            {
                return Response<bool>.FromException($"Failed to check if '{typeof(T).Name}' is registered", ex);
            }
        }

        public IResponse<bool> IsRegistered(Type type)
        {
            try
            {
                return Response<bool>.FromSuccess(_container.IsRegistered(type));
            }
            catch (Exception ex)
            {
                return Response<bool>.FromException($"Failed to check if '{type.Name}' is registered", ex);
            }
        }
    }
}

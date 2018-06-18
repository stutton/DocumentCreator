using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Fields;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Fields
{
    public class FieldFactoryService : IFieldFactoryService
    {
        private readonly Func<Type, IFieldTemplate> _fieldResolver;
        private readonly IServiceResolver _serviceResolver;

        private Dictionary<string, Type> _fieldTypes;

        public FieldFactoryService(Func<Type, IFieldTemplate> fieldResolver, IServiceResolver serviceResolver)
        {
            _fieldResolver = fieldResolver;
            _serviceResolver = serviceResolver;
        }

        public async Task<IResponse<IFieldTemplate>> CreateField(Type fieldType)
        {
            try
            {
                if (!typeof(IFieldTemplate).IsAssignableFrom(fieldType))
                {
                    return Response<IFieldTemplate>.FromFailure($"Type '{fieldType.Name}' is not an IFieldTemplate");
                }

                var field = _fieldResolver(fieldType);
                if (field == null)
                {
                    return Response<IFieldTemplate>.FromFailure($"Failed to create field of type '{fieldType.Name}'");
                }

                if (field is IRequiresInitialization initializeMe)
                {
                    await initializeMe.Initialize(_serviceResolver);
                }

                return Response<IFieldTemplate>.FromSuccess(field);
            }
            catch (Exception ex)
            {
                return Response<IFieldTemplate>.FromException("Failed to create field for unknown reason", ex);
            }
        }

        public IResponse<Dictionary<string, Type>> GetAllFieldKeys()
        {
            try
            {
                if (_fieldTypes != null)
                {
                    return Response<Dictionary<string, Type>>.FromSuccess(_fieldTypes);
                }

                _fieldTypes = new Dictionary<string, Type>();
                var fieldTypes = typeof(IFieldTemplate).Assembly.GetInheritingTypes<IFieldTemplate>();
                foreach (var fieldType in fieldTypes)
                {
                    if (_fieldResolver(fieldType) is IFieldTemplate field)
                    {
                        _fieldTypes.Add(field.TypeDisplayName, fieldType);
                    }
                }

                return Response<Dictionary<string, Type>>.FromSuccess(_fieldTypes);
            }
            catch (Exception ex)
            {
                return Response<Dictionary<string, Type>>.FromException("Failed to get all field keys", ex);
            }
        }
    }
}

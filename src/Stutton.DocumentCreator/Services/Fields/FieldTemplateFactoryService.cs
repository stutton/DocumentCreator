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
    public class FieldTemplateFactoryService : IFieldTemplateFactoryService
    {
        private readonly Func<Type, FieldTemplateModelBase> _fieldResolver;

        private Dictionary<string, Type> _fieldTypes;

        public FieldTemplateFactoryService(Func<Type, FieldTemplateModelBase> fieldResolver)
        {
            _fieldResolver = fieldResolver;
        }

        public async Task<IResponse<FieldTemplateModelBase>> CreateField(Type fieldType)
        {
            try
            {
                if (!typeof(FieldTemplateModelBase).IsAssignableFrom(fieldType))
                {
                    return Response<FieldTemplateModelBase>.FromFailure($"Type '{fieldType.Name}' is not an IFieldTemplate");
                }

                var field = _fieldResolver(fieldType);
                if (field == null)
                {
                    return Response<FieldTemplateModelBase>.FromFailure($"Failed to create field of type '{fieldType.Name}'");
                }

                if (field is IRequiresInitialization initializeMe)
                {
                    await initializeMe.Initialize();
                }

                return Response<FieldTemplateModelBase>.FromSuccess(field);
            }
            catch (Exception ex)
            {
                return Response<FieldTemplateModelBase>.FromException("Failed to create field for unknown reason", ex);
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
                var fieldTypes = typeof(FieldTemplateModelBase).Assembly.GetInheritingTypes<FieldTemplateModelBase>();
                foreach (var fieldType in fieldTypes)
                {
                    if (_fieldResolver(fieldType) is FieldTemplateModelBase field)
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

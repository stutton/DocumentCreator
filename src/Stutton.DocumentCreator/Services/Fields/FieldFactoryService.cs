using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.Documents.Fields;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Fields
{
    public class FieldFactoryService : IFieldFactoryService
    {
        private readonly Func<Type, IField> _fieldResolver;

        private Dictionary<string, Type> _fieldTypes;

        public FieldFactoryService(Func<Type, IField> fieldResolver)
        {
            _fieldResolver = fieldResolver;
        }

        public IResponse<IField> CreateField(Type fieldType)
        {
            try
            {
                if (!typeof(IField).IsAssignableFrom(fieldType))
                {
                    return Response<IField>.FromFailure($"Type '{fieldType.Name}' is not an IField");
                }

                var field = _fieldResolver(fieldType);
                if (field == null)
                {
                    return Response<IField>.FromFailure($"Failed to create field of type '{fieldType.Name}'");
                }

                return Response<IField>.FromSuccess(field);
            }
            catch (Exception ex)
            {
                return Response<IField>.FromException("Failed to create field for unknown reason", ex);
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
                var fieldTypes = typeof(IField).Assembly.GetInheritingTypes<IField>();
                foreach (var fieldType in fieldTypes)
                {
                    if (_fieldResolver(fieldType) is IField field)
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

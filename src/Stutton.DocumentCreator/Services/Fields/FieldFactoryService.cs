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

        private readonly List<(string key, string typeName, Type type)> _fieldTypes = new List<(string key, string typeName, Type type)>();

        public FieldFactoryService(Func<Type, IField> fieldResolver)
        {
            _fieldResolver = fieldResolver;
        }

        public async Task<IResponse<IField>> CreateField(string fieldKey)
        {
            if (!_fieldTypes.Any())
            {
                await GetAllFieldTypes();
            }

            if (_fieldTypes.All(t => t.key != fieldKey))
            {
                return Response<IField>.FromFailure($"No such field '{fieldKey}'");
            }
            var field = _fieldResolver(_fieldTypes.First(t => t.key == fieldKey).type);
            if (field == null)
            {
                return Response<IField>.FromFailure($"Failed to create field of type '{fieldKey}'");
            }

            return Response<IField>.FromSuccess(field);
        }

        public async Task<IResponse<IEnumerable<(string key, string typeName)>>> GetAllFieldKeys()
        {
            if (!_fieldTypes.Any())
            {
                await GetAllFieldTypes();
            }

            return !_fieldTypes.Any()
                ? Response<IEnumerable<(string key, string typeName)>>.FromFailure("No field types found")
                : Response<IEnumerable<(string key, string typeName)>>.FromSuccess(_fieldTypes.Select(t => (t.key, t.typeName)));
        }

        private Task GetAllFieldTypes()
        {
            return Task.Run(() =>
            {
                var fieldModelTypes = typeof(IField).Assembly.GetInheritingTypes<IField>();
                foreach (var fieldType in fieldModelTypes)
                {
                    if (_fieldResolver(fieldType) is IField field)
                    {
                        _fieldTypes.Add((field.FieldKey, field.TypeDisplayName, field.GetType()));
                    }
                }
            });
        }
    }
}

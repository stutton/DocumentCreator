using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Fields;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Fields
{
    public interface IFieldTemplateFactoryService
    {
        Task<IResponse<FieldTemplateModelBase>> CreateField(Type fieldKey);
        IResponse<Dictionary<string, Type>> GetAllFieldKeys();
    }
}

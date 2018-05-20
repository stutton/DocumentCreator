using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.Documents.Fields;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Fields
{
    public interface IFieldFactoryService
    {
        Task<IResponse<IField>> CreateField(string fieldKey);
        Task<IResponse<IEnumerable<(string key, string typeName)>>> GetAllFieldKeys();
    }
}

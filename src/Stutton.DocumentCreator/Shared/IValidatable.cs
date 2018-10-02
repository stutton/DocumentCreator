using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Shared
{
    public interface IValidatable
    {
        bool HasValidationErrors { get; }
        IEnumerable<ValidationError> ValidationErrors { get; }
        bool Validate();
    }
}

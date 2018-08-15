using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Shared
{
    public interface IContext
    {
        bool IsInvokeRequired { get; }
        void Invoke(Action action);
    }
}

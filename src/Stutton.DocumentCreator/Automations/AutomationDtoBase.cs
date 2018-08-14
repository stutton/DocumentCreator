using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Automations
{
    public abstract class AutomationDtoBase
    {
        public abstract string Name { get; set; }
    }
}

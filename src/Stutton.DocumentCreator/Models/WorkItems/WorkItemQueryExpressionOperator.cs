using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Models.WorkItems
{
    public enum WorkItemQueryExpressionOperator
    {
        Equals,
        GreaterThan,
        LessThan,
        NotEqual,
        Contains,
        In
    }
}

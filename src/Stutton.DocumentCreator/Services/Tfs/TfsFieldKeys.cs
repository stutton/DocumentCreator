using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Services.Tfs
{
    internal static class TfsFieldKeys
    {
        public const string Id = "System.Id";
        public const string Type = "System.WorkItemType";
        public const string AssignedTo = "System.AssignedTo";
        public const string Title = "System.Title";
        public const string Description = "System.Description";
        public const string TestDocAttached = "CSI.TestResultsAttached";
        public const string State = "System.State";
        public const string Area = "System.AreaPath";
    }
}

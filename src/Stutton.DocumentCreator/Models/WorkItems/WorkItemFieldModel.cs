using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.WorkItems
{
    public class WorkItemFieldModel : Observable
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private string _referenceName;

        public string ReferenceName
        {
            get => _referenceName;
            set => Set(ref _referenceName, value);
        }
    }
}

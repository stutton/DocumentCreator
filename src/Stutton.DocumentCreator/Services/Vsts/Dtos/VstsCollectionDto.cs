using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Services.Vsts.Dtos
{
    public class VstsCollectionDto<T>
    {
        public int Count { get; set; }
        public IEnumerable<T> Value { get; set; }
    }
}

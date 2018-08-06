using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Services.Updating
{
    public class CheckForUpdateResult
    {
        public bool UpdateInstalled { get; set; }
        public Version NewVersion { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Services.Updating
{
    public class CheckForUpdateResult
    {
        public static CheckForUpdateResult FromUpdate(Version oldVersion, Version newVersion)
        {
            return new CheckForUpdateResult(oldVersion, newVersion);
        }

        public static CheckForUpdateResult FromNoUpdate()
        {
            return new CheckForUpdateResult();
        }

        private CheckForUpdateResult() { }

        private CheckForUpdateResult(Version oldVersion, Version newVersion)
        {
            UpdateInstalled = true;
            OldVersion = oldVersion;
            NewVersion = newVersion;
        }

        public bool UpdateInstalled { get; }
        public Version NewVersion { get; }
        public Version OldVersion { get; }
    }
}

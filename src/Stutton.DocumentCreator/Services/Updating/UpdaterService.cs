using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet;
using Squirrel;
using Stutton.DocumentCreator.Services.Settings;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Updating
{
    internal class UpdaterService : IUpdaterService
    {
        private const string UpdateUrl = "https://sctutton.blob.core.windows.net/documentcreator";

        public async Task<IResponse<CheckForUpdateResult>> Update()
        {
            using (var mgr = await GetUpdateManager(UpdateUrl))
            {
                var oldVersion = mgr.CurrentlyInstalledVersion();
                var newVersion = await mgr.UpdateApp();
                if (oldVersion.Version < newVersion.Version.Version)
                {
                    return Response<CheckForUpdateResult>.FromSuccess(
                       CheckForUpdateResult.FromUpdate(oldVersion.Version, newVersion.Version.Version));
                }
            }

            return Response<CheckForUpdateResult>.FromSuccess(CheckForUpdateResult.FromNoUpdate());
        }

        private async Task<IUpdateManager> GetUpdateManager(string updateReleasesLocation)
        {
            if (updateReleasesLocation.Contains("github"))
            {
                return await UpdateManager.GitHubUpdateManager(updateReleasesLocation);
            }
            return new UpdateManager(updateReleasesLocation);
        }
    }
}

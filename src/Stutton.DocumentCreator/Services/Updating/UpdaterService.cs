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
        private readonly ISettingsService _settingsService;

        public UpdaterService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task<IResponse<CheckForUpdateResult>> Update()
        {
            var response = await _settingsService.GetSettings();
            if (!response.Success)
            {
                return Response<CheckForUpdateResult>.FromFailure(response.Message, response.Code);
            }

            var updateReleasesLocation = response.Value.UpdateReleasesLocation;
            using (var mgr = await GetUpdateManager(updateReleasesLocation))
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

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
            var result = new CheckForUpdateResult();
            using (var mgr = new UpdateManager(updateReleasesLocation))
            {
                var oldVersion = mgr.CurrentlyInstalledVersion();
                var newVersion = await mgr.UpdateApp();
                if (oldVersion.Version < newVersion.Version.Version)
                {
                    result.NewVersion = newVersion.Version.Version;
                    result.UpdateInstalled = true;
                }
            }

            return Response<CheckForUpdateResult>.FromSuccess(result);
        }
    }
}

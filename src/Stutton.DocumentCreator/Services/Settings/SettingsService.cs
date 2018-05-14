using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Stutton.DocumentCreator.Models.Settings;

namespace Stutton.DocumentCreator.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly string _settingsFileName = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\DocumentCreator\\DocumentCreator.settings";
        private readonly string _settingsDirectoryName = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\DocumentCreator";
        private SettingsModel _settingsCache;

        public async Task<IResponse> SaveSettings(SettingsModel settings)
        {
            try
            {
                _settingsCache = settings;
                var settingsJson = await Task.Run(() => JsonConvert.SerializeObject(settings));
                await Task.Run(() => File.WriteAllText(_settingsFileName, settingsJson));
                return Response.FromSuccess();
            }
            catch (Exception ex)
            {
                return Response.FromFailure($"Failed to save settings: {ex.Message}");
            }
        }

        public async Task<IResponse<SettingsModel>> GetSettings()
        {
            try
            {
                if (_settingsCache != null)
                {
                    return Response<SettingsModel>.FromSuccess(_settingsCache);
                }

                if (!File.Exists(_settingsFileName))
                {
                    _settingsCache = new SettingsModel();
                    Directory.CreateDirectory(_settingsDirectoryName);
                    var response = await SaveSettings(_settingsCache);
                    return Response<SettingsModel>.FromFailure(!response.Success
                        ? response.Message
                        : "No settings file found. Created empty settings file. Please configure the settings on the Settings screen.");
                }

                var settingsJson = await Task.Run(() => File.ReadAllText(_settingsFileName));
                var model = await Task.Run(() => JsonConvert.DeserializeObject<SettingsModel>(settingsJson));

                _settingsCache = model;
                return Response<SettingsModel>.FromSuccess(model);
            }
            catch(Exception ex)
            {
                return Response<SettingsModel>.FromFailure($"Failed to load settings: {ex.Message}");
            }
        }
    }
}

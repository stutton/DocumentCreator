using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Stutton.DocumentCreator.Models.Settings;
using Stutton.DocumentCreator.Shared;
using AppDomain = System.AppDomain;

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
                if (!Directory.Exists(_settingsDirectoryName))
                {
                    Directory.CreateDirectory(_settingsDirectoryName);
                }
                var settingsJson = await Task.Run(() => JsonConvert.SerializeObject(settings, Formatting.Indented));
                await Task.Run(() => File.WriteAllText(_settingsFileName, settingsJson));
                return Response.FromSuccess();
            }
            catch (Exception ex)
            {
                return Response.FromException($"Failed to save settings", ex);
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
                    return Response<SettingsModel>.FromFailure("No settings file found", ResponseCode.FileNotFound);
                }

                var settingsJson = await Task.Run(() => File.ReadAllText(_settingsFileName));
                var model = await Task.Run(() => JsonConvert.DeserializeObject<SettingsModel>(settingsJson));

                _settingsCache = model;
                return Response<SettingsModel>.FromSuccess(model);
            }
            catch(Exception ex)
            {
                return Response<SettingsModel>.FromException($"Failed to load settings", ex);
            }
        }

        public async Task<IResponse<SettingsModel>> GetDefaultSettingsAsync()
        {
            try
            {
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var files = Directory.GetFiles(baseDirectory, "*.settings.defaults");
                if (!files.Any())
                {
                    return Response<SettingsModel>.FromFailure("No default settings file found",
                                                               ResponseCode.FileNotFound);
                }

                var defaultsFilePath = files.First();
                var defaultsJson = await Task.Run(() => File.ReadAllText(defaultsFilePath));
                var defaultsModel = await Task.Run(() => JsonConvert.DeserializeObject<SettingsModel>(defaultsJson));
                return Response<SettingsModel>.FromSuccess(defaultsModel);
            }
            catch (Exception ex)
            {
                return Response<SettingsModel>.FromException($"Failed to load default settings", ex);
            }
        }

        public async Task<IResponse<SettingsModel>> GetSettingsTransformAsync()
        {
            try
            {
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var files = Directory.GetFiles(baseDirectory, "*.settings.transform");
                if (!files.Any())
                {
                    return Response<SettingsModel>.FromFailure("No settings transforms found",
                        ResponseCode.FileNotFound);
                }

                var transformPath = files.First();
                var transformJson = await Task.Run(() => File.ReadAllText(transformPath));
                var transformModel =
                    await Task.Run(() => JsonConvert.DeserializeObject<SettingsModel>(transformJson));
                return Response<SettingsModel>.FromSuccess(transformModel);
            }
            catch (Exception ex)
            {
                return Response<SettingsModel>.FromException($"Failed to load settings transform", ex);
            }
        }
    }
}

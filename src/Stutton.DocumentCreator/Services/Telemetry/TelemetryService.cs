using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Stutton.DocumentCreator.Services.Settings;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Telemetry
{
    public class TelemetryService : ITelemetryService
    {
        private readonly ISettingsService _settingsService;

        private TelemetryClient _telemetryClient;
        private bool _initialized;

        public TelemetryService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task<IResponse> Initialize()
        {
            if (_initialized)
            {
                return Response.FromSuccess();
            }

            var response = await _settingsService.GetSettings();
            if (!response.Success)
            {
                return Response.FromFailure(response.Message);
            }

            var settings = response.Value;
            TelemetryConfiguration.Active.InstrumentationKey = settings.ApplicationInsightsKey;

            _telemetryClient = new TelemetryClient();

            _telemetryClient.Context.User.Id = (Environment.UserName + Environment.MachineName).GetHashCode().ToString();
            _telemetryClient.Context.Session.Id = Guid.NewGuid().ToString();
            _telemetryClient.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
            _telemetryClient.Context.Component.Version = Assembly.GetEntryAssembly().GetName().Version.ToString();

            _telemetryClient.TrackEvent("Application Started");

            _initialized = true;
            return Response.FromSuccess();
        }

        public IResponse TrackPageView(string pageKey)
        {
            if (!_initialized)
            {
                return Response.FromFailure("Telemetry service must be initialized before use");
            }

            _telemetryClient.TrackPageView(pageKey);
            return Response.FromSuccess();
        }

        public IResponse TrackFailedResponse(IResponse response)
        {
            if (!_initialized)
            {
                return Response.FromFailure("Telemetry service must be initialized before use");
            }

            if (response.Success)
            {
                return Response.FromFailure("Response to log did not fail");
            }

            var message = FormatResponseMessage(response);
            
            var ex = response.Code == ResponseCode.Exception
                ? new TelemetryException(message, response.Exception)
                : new TelemetryException(message);

            _telemetryClient.TrackException(ex);

            return Response.FromSuccess();
        }

        public IResponse TrackException(Exception ex)
        {
            if (!_initialized)
            {
                return Response.FromFailure("Telemetry service must be initialized before use");
            }

            _telemetryClient.TrackException(ex);
            return Response.FromSuccess();
        }

        private string FormatResponseMessage(IResponse response)
        {
            return
                $"FAILED RESPONSE at {response.CallerMemberName}:{response.LineNumber}|Code:{response.Code}|{response.Message}|Id:{response.Id.ToString()}";
        }

        private class TelemetryException : Exception
        {
            public TelemetryException(string message)
            :base(message)
            {
            }

            public TelemetryException(string message, Exception wrappedException)
            :base(message, wrappedException)
            {
                
            }
        }

        public void Dispose()
        {
            _telemetryClient.Flush();
        }
    }
}

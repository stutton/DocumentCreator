using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Telemetry
{
    public interface ITelemetryService : IDisposable
    {
        Task<IResponse> Initialize();
        IResponse TrackPageView(string pageKey);
        IResponse TrackFailedResponse(IResponse response);
        IResponse TrackException(Exception ex);
        bool Enabled { get; set; }
    }
}

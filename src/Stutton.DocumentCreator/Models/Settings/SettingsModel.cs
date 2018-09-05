using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.Settings
{
    [DataContract(Name = "Settings")]
    public class SettingsModel : Observable
    {
        private string _tfsUrl;
        private string _tfsDefaultCollection;
        private string _tfsUserName;
        private WorkItemQueryModel _workItemQuery = new WorkItemQueryModel();
        private string _updateReleasesLocation;
        private bool? _sendTelemetryEnabled;

        [DataMember]
        public string TfsUrl
        {
            get => _tfsUrl;
            set => Set(ref _tfsUrl, value);
        }

        [DataMember]
        public string TfsDefaultCollection
        {
            get => _tfsDefaultCollection;
            set =>  Set(ref _tfsDefaultCollection, value);
        }

        [DataMember]
        public string TfsUserName
        {
            get => _tfsUserName;
            set => Set(ref _tfsUserName, value);
        }

        private string _applicationInsightsKey;

        [DataMember]
        public string ApplicationInsightsKey
        {
            get => _applicationInsightsKey;
            set => Set(ref _applicationInsightsKey, value);
        }

        [DataMember]
        public string UpdateReleasesLocation
        {
            get => _updateReleasesLocation;
            set => Set(ref _updateReleasesLocation, value);
        }

        [DataMember]
        public bool? SendTelemetryEnabled
        {
            get => _sendTelemetryEnabled;
            set => Set(ref _sendTelemetryEnabled, value);
        }
    }
}

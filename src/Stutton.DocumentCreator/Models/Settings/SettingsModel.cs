using System.Runtime.Serialization;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.Settings
{
    [DataContract(Name = "Settings")]
    public class SettingsModel : Observable
    {
        private string _tfsUrl;
        private string _tfsDefaultCollection;
        private string _tfsUserName;
        private bool? _sendTelemetryEnabled;
        private bool _useAdalAuth;
        private string _defaultProject;

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
        public bool? SendTelemetryEnabled
        {
            get => _sendTelemetryEnabled;
            set => Set(ref _sendTelemetryEnabled, value);
        }

        [DataMember]
        public bool UseAdalAuth
        {
            get => _useAdalAuth;
            set => Set(ref _useAdalAuth, value);
        }

        [DataMember]
        public string DefaultProject
        {
            get => _defaultProject;
            set => Set(ref _defaultProject, value);
        }
    }
}

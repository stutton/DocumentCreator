using System.Diagnostics;
using System.Reflection;

namespace Stutton.DocumentCreator.ViewModels.Dialogs
{
    public class AboutDialogViewModel
    {
        private string _version;
        public string Version => _version ?? (_version = GetVersion());

        private static string GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.ProductVersion;
        }
    }
}
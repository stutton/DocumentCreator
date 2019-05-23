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
            var productVersion = fileVersionInfo.ProductVersion;
            if (productVersion.Contains("+"))
            {
                productVersion = productVersion.Substring(0, productVersion.IndexOf("+"));
            }
            return productVersion;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.ViewModels.Pages
{
    public class DocumentCreatorPageViewModel : PageBase
    {
        public const string Key = "DocumentCreatorPage";
        public override string PageKey => Key;
        public override string Title => "Create Document";
        public override bool IsOnDemandPage => true;
    }
}

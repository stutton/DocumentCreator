using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Stutton.DocumentCreator.Models.DocumentTemplates;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.ViewModels.Pages
{
    public class DocumentsPageViewModel : PageBase
    {
        public const string Key = "DocumentsPage";
        public override string PageKey => Key;
        public override string Title => "Documents";
        public override bool IsOnDemandPage => false;

        private ObservableCollection<DocumentTemplateModel> _propertyName;

        public ObservableCollection<DocumentTemplateModel> Documents
        {
            get => _propertyName;
            set => Set(ref _propertyName, value);
        }

        #region CreateDocumentCommand

        private ICommand _createDocumentCommand;
        public ICommand CreateDocumentCommand => _createDocumentCommand ?? (_createDocumentCommand = new RelayCommand(CreateDocument));

        private void CreateDocument()
        {

        }

        #endregion
    }
}

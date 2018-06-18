using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DocumentFormat.OpenXml.Packaging;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.List.Document
{
    [DataContract(Name = "ListField")]
    public class ListFieldDocumentModel : Observable, IFieldDocument
    {
        public const string Key = "ListField";

        [IgnoreDataMember]
        public string Description => "List of text and images";

        [IgnoreDataMember]
        public string TypeDisplayName => "Field";

        [IgnoreDataMember]
        public string FieldKey => Key;

        private string _name;
        [DataMember]
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        [DataMember]
        public ObservableCollection<ListFieldStepModel> Steps { get; } = new ObservableCollection<ListFieldStepModel>();

        #region AddStep Command

        private ICommand _addStepCommand;
        [IgnoreDataMember]
        public ICommand AddStepCommand => _addStepCommand ?? (_addStepCommand = new RelayCommand(AddStep));

        private void AddStep()
        {
            Steps.Add(GetNewStep());
        }

        #endregion

        public Task<IResponse> ModifyDocument(WordprocessingDocument document, IWorkItem workItem)
        {
            throw new NotImplementedException();
        }

        private ListFieldStepModel GetNewStep()
        {
            var step = new ListFieldStepModel();
            step.RequestDeleteMe += StepOnRequestDeleteMe;
            step.RequestMove += StepOnRequestMove;
            return step;
        }

        private void StepOnRequestMove(object sender, MoveListFieldStepEventArgs e)
        {
            var step = (ListFieldStepModel) sender;
        }

        private void StepOnRequestDeleteMe(object sender, EventArgs e)
        {
            var step = (ListFieldStepModel) sender;

        }
    }
}

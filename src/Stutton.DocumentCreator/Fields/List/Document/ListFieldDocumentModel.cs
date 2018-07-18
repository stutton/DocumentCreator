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
using Stutton.DocumentCreator.Services.Image;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.List.Document
{
    public class ListFieldDocumentModel : FieldDocumentModelBase
    {
        private readonly IImageService _imageService;
        public const string Key = "ListField";

        public ListFieldDocumentModel(IImageService imageService)
        {
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
        }
        
        public override string Description => "List of text and images";
        public string TypeDisplayName => "Field";
        public override string FieldKey => Key;
        
        public ObservableCollection<ListFieldStepModel> Steps { get; set; } = new ObservableCollection<ListFieldStepModel>();

        #region AddStep Command

        private ICommand _addStepCommand;
        [IgnoreDataMember]
        public ICommand AddStepCommand => _addStepCommand ?? (_addStepCommand = new RelayCommand(AddStep));

        private void AddStep()
        {
            Steps.Add(GetNewStep());
        }

        #endregion

        public override Task<IResponse> ModifyDocument(WordprocessingDocument document, IWorkItem workItem)
        {
            throw new NotImplementedException();
        }

        private ListFieldStepModel GetNewStep()
        {
            var step = new ListFieldStepModel(_imageService);
            step.RequestDeleteMe += StepOnRequestDeleteMe;
            step.RequestMove += StepOnRequestMove;
            step.Index = Steps.Count + 1;
            return step;
        }

        private void StepOnRequestMove(object sender, MoveListFieldStepEventArgs e)
        {
            var step = (ListFieldStepModel) sender;
            var index = Steps.IndexOf(step);
            if (e.Direction == MoveListFieldStepEventArgs.MoveDirection.Up && index != 0)
            {
                Steps.Remove(step);
                Steps.Insert(index - 1, step);
            }

            if (e.Direction == MoveListFieldStepEventArgs.MoveDirection.Down && index != Steps.Count - 1)
            {
                Steps.Remove(step);
                Steps.Insert(index + 1, step);
            }
            UpdateIndexes();
        }

        private void StepOnRequestDeleteMe(object sender, EventArgs e)
        {
            var step = (ListFieldStepModel) sender;
            Steps.Remove(step);
            UpdateIndexes();
        }

        private void UpdateIndexes()
        {
            for (var i = 0; i < Steps.Count; i++)
            {
                Steps[i].Index = i + 1;
            }
        }
    }
}

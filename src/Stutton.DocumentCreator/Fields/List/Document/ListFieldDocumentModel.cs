﻿using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DocumentFormat.OpenXml.Packaging;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services.Image;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.List.Document
{
    public class ListFieldDocumentModel : FieldDocumentModelBase, IExpandable
    {
        private readonly IImageService _imageService;
        private readonly IContext _context;
        public const string Key = "ListField";

        public ListFieldDocumentModel(IImageService imageService, IContext context)
        {
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public override string Description => "List of text and images";
        public override string FieldKey => Key;

        private bool _isExpanded;
        [IgnoreDataMember]
        public bool IsExpanded
        {
            get => _isExpanded;
            set => Set(ref _isExpanded, value);
        }

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

        public override async Task<IResponse> ModifyDocument(WordprocessingDocument document, IWorkItem workItem)
        {
            try
            {
                await Task.Run(() =>
                {
                    AddImageToDocument(document);
                });
                return Response.FromSuccess();
            }
            catch (Exception ex)
            {
                return Response.FromException("Failed to write step to document", ex);
            }
        }

        private void AddImageToDocument(WordprocessingDocument document)
        {
            if (_context.IsInvokeRequired)
            {
                _context.Invoke(() => AddImageToDocument(document));
                return;
            }
            foreach (var step in Steps)
            {
                document.AddNumberedTextToBody(step.Text, step.Index);
                if (step.HasImage)
                {
                    document.AddImageToBody(step.Image);
                }
            }
        }

        private ListFieldStepModel GetNewStep()
        {
            var step = new ListFieldStepModel(_imageService, _context);
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

        private void AddImage(WordprocessingDocument document, BitmapSource image)
        {

        }
    }
}

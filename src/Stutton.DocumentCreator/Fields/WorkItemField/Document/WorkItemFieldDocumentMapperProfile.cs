using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Stutton.DocumentCreator.Fields.WorkItemField.Document
{
    public class WorkItemFieldDocumentMapperProfile : Profile
    {
        public WorkItemFieldDocumentMapperProfile()
        {
            CreateMap<WorkItemFieldDocumentModel, WorkItemFieldDocumentDto>()
                .IncludeBase<FieldDocumentModelBase, FieldDocumentDtoBase>()
                .ReverseMap()
                .ConstructUsingServiceLocator();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Stutton.DocumentCreator.Fields.List.Document;
using Stutton.DocumentCreator.Fields.Text.Document;
using Stutton.DocumentCreator.Fields.UserName.Document;
using Stutton.DocumentCreator.Fields.WorkItemField.Document;
using Stutton.DocumentCreator.Models.Document;

namespace Stutton.DocumentCreator.Fields
{
    public class FieldDocumentMapperProfile : Profile
    {
        public FieldDocumentMapperProfile()
        {
            CreateMap<FieldDocumentModelBase, FieldDocumentDtoBase>().ReverseMap();

            CreateMap<DocumentModel, DocumentDto>().ReverseMap();
        }
    }
}

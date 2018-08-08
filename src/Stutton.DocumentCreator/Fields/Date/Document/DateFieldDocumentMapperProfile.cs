using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Stutton.DocumentCreator.Fields.Date.Document
{
    public class DateFieldDocumentMapperProfile : Profile
    {
        public DateFieldDocumentMapperProfile()
        {
            CreateMap<DateFieldDocumentModel, DateFieldDocumentDto>()
                .IncludeBase<FieldDocumentModelBase, FieldDocumentDtoBase>()
                .ReverseMap()
                .ConstructUsingServiceLocator();
        }
    }
}

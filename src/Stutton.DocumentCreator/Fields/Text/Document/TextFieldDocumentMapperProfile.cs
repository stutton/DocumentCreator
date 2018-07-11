using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Stutton.DocumentCreator.Fields.Text.Document
{
    public class TextFieldDocumentMapperProfile : Profile
    {
        public TextFieldDocumentMapperProfile()
        {
            CreateMap<TextFieldDocumentModel, TextFieldDocumentDto>()
                .IncludeBase<FieldDocumentModelBase, FieldDocumentDtoBase>()
                .ReverseMap()
                .ConstructUsingServiceLocator();
        }
    }
}

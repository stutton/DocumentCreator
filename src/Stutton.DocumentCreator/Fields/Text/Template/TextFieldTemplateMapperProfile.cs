using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Stutton.DocumentCreator.Fields.Text.Template
{
    public class TextFieldTemplateMapperProfile : Profile
    {
        public TextFieldTemplateMapperProfile()
        {
            CreateMap<TextFieldTemplateModel, TextFieldTemplateDto>()
                .IncludeBase<FieldTemplateModelBase, FieldTemplateDtoBase>()
                .ReverseMap()
                .ConstructUsingServiceLocator();
        }
    }
}

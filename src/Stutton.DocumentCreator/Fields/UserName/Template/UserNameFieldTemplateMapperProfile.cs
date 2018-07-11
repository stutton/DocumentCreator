using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Stutton.DocumentCreator.Fields.UserName.Template
{
    public class UserNameFieldTemplateMapperProfile : Profile
    {
        public UserNameFieldTemplateMapperProfile()
        {
            CreateMap<UserNameFieldTemplateModel, UserNameFieldTemplateDto>()
                .IncludeBase<FieldTemplateModelBase, FieldTemplateDtoBase>()
                .ReverseMap()
                .ConstructUsingServiceLocator();
        }
    }
}

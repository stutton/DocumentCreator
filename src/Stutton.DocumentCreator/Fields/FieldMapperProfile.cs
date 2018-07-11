using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Stutton.DocumentCreator.Fields.List.Template;
using Stutton.DocumentCreator.Fields.Text.Template;
using Stutton.DocumentCreator.Fields.UserName.Template;
using Stutton.DocumentCreator.Fields.WorkItemField.Template;
using Stutton.DocumentCreator.Models.Template;

namespace Stutton.DocumentCreator.Fields
{
    public class FieldMapperProfile : Profile
    {
        public FieldMapperProfile()
        {
            CreateMap<FieldTemplateBase, FieldTemplateDtoBase>()
               .Include<ListFieldTemplateModel, ListFieldTemplateDto>()
               .Include<TextFieldTemplateModel, TextFieldTemplateDto>()
               .Include<UserNameFieldTemplateModel, UserNameFieldTemplateDto>()
               .Include<WorkItemFieldTemplateModel, WorkItemFieldTemplateDto>()
               .ReverseMap();
            CreateMap<ListFieldTemplateModel, ListFieldTemplateDto>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<TextFieldTemplateModel, TextFieldTemplateDto>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<UserNameFieldTemplateModel, UserNameFieldTemplateDto>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<WorkItemFieldTemplateModel, WorkItemFieldTemplateDto>().ReverseMap().ConstructUsingServiceLocator();

            CreateMap<DocumentTemplateModel, DocumentTemplateDto>().ReverseMap();
        }
    }
}

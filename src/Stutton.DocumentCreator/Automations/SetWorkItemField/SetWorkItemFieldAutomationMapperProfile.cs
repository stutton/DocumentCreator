using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Stutton.DocumentCreator.Automations.SetWorkItemField
{
    public class SetWorkItemFieldAutomationMapperProfile : Profile
    {
        public SetWorkItemFieldAutomationMapperProfile()
        {
            CreateMap<SetWorkItemFieldAutomationModel, SetWorkItemFieldAutomationDto>()
                .IncludeBase<AutomationModelBase, AutomationDtoBase>()
                .ReverseMap()
                .ConstructUsingServiceLocator();
        }
    }
}

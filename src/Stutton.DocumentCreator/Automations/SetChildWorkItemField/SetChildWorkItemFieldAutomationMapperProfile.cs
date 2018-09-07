using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Stutton.DocumentCreator.Automations.SetChildWorkItemField
{
    public class SetChildWorkItemFieldAutomationMapperProfile : Profile
    {
        public SetChildWorkItemFieldAutomationMapperProfile()
        {
            CreateMap<SetChildWorkItemFieldAutomationModel, SetChildWorkItemFieldAutomationDto>()
                .IncludeBase<AutomationModelBase, AutomationDtoBase>()
                .ReverseMap()
                .ConstructUsingServiceLocator();
        }
    }
}

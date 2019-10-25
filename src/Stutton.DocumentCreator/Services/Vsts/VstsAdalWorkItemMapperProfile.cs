using AutoMapper;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services.Vsts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Services.Vsts
{
    public class VstsAdalWorkItemMapperProfile : Profile
    {
        public VstsAdalWorkItemMapperProfile()
        {
            CreateMap<WorkItemApiResultDto, WorkItemModel>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(
                    dest => dest.Type,
                    opt => opt.MapFrom(src => src.Fields.Type))
                .ForMember(
                    dest => dest.AssignedTo,
                    opt => opt.MapFrom(src => src.Fields.AssignedTo.DisplayName))
                .ForMember(
                    dest => dest.Title,
                    opt => opt.MapFrom(src => src.Fields.Title))
                .ForMember(
                    dest => dest.Description,
                    opt => opt.MapFrom(src => src.Fields.Description))
                .ForMember(
                    dest => dest.State,
                    opt => opt.MapFrom(src => src.Fields.State))
                .ForMember(
                    dest => dest.Area,
                    opt => opt.MapFrom(src => src.Fields.Area))
                .ForMember(
                    dest => dest.Selected,
                    opt => opt.MapFrom(src => false))
                .ForMember(
                    dest => dest.Team,
                    opt => opt.MapFrom(src => src.Fields.Team))
                .ForMember(
                    dest => dest.ChildWorkItems,
                    opt => opt.MapFrom(src => MapChildWorkItems(src.Relations)));
        }

        private static int[] MapChildWorkItems(IEnumerable<WorkItemApiRelationItemDto> relations)
        {
            if(relations == null || !relations.Any())
            {
                return null;
            }

            var children = new List<int>();
            foreach (var relation in relations)
            {
                if(relation.Attributes.Name.Equals("child", StringComparison.InvariantCultureIgnoreCase))
                {
                    var idStr = relation.Url.Split('/').Last();
                    if(int.TryParse(idStr, out var id))
                    {
                        children.Add(id);
                    }
                }
            }

            return children.ToArray();
        }
    }
}

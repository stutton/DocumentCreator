using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using Stutton.DocumentCreator.Models.WorkItems;

namespace Stutton.DocumentCreator.Services.Vsts
{
    public class VstsWorkItemMapperProfile : Profile
    {
        public VstsWorkItemMapperProfile()
        {
            CreateMap<WorkItem, WorkItemModel>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(
                    dest => dest.Type,
                    opt => opt.MapFrom(src => GetFieldOrDefault(src, VstsFields.Type)))
                .ForMember(
                    dest => dest.AssignedTo,
                    opt => opt.MapFrom(src => GetFieldOrDefault(src, VstsFields.AssignedTo)))
                .ForMember(
                    dest => dest.Title,
                    opt => opt.MapFrom(src => GetFieldOrDefault(src, VstsFields.Title)))
                .ForMember(
                    dest => dest.Description,
                    opt => opt.MapFrom(src => GetFieldOrDefault(src, VstsFields.Description)))
                .ForMember(
                    dest => dest.State,
                    opt => opt.MapFrom(src => GetFieldOrDefault(src, VstsFields.State)))
                .ForMember(
                    dest => dest.Area,
                    opt => opt.MapFrom(src => GetFieldOrDefault(src, VstsFields.Area)))
                .ForMember(
                    dest => dest.Selected,
                    opt => opt.MapFrom(src => false))
                .ForMember(
                    dest => dest.Team,
                    opt => opt.MapFrom(src => GetFieldOrDefault(src, VstsFields.TeamProject)))
                .ForMember(
                    dest => dest.ChildWorkItems,
                    opt => opt.MapFrom(src => MapChildWorkItems(src)));
        }

        private static string GetFieldOrDefault(WorkItem src, string key)
        {
            if (src.Fields.ContainsKey(key))
            {
                switch (key)
                {
                    case VstsFields.AssignedTo:
                        var idRef = (IdentityRef) src.Fields[key];
                        return idRef.DisplayName;
                    default:
                        return (string) src.Fields[key];
                }
            }
            return string.Empty;
        }

        private static int[] MapChildWorkItems(WorkItem src)
        {
            if(src.Relations == null || src.Relations.Count == 0)
            {
                return null;
            }
            var children = new List<int>();
            foreach (var relation in src.Relations)
            {
                if (relation.Rel == "System.LinkTypes.Hierarchy-Forward")
                {
                    var idStr = relation.Url.Split('/').Last();
                    if (int.TryParse(idStr, out var id))
                    {
                        children.Add(id);
                    }
                }
            }

            return children.ToArray();
        }

        private static class VstsFields
        {
            public const string Type = "System.WorkItemType";
            public const string AssignedTo = "System.AssignedTo";
            public const string Title = "System.Title";
            public const string Description = "System.Description";
            public const string State = "System.State";
            public const string Area = "System.AreaPath";
            public const string TeamProject = "System.TeamProject";
        }
    }
}

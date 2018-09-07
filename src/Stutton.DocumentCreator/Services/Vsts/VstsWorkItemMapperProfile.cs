using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
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
                    opt => opt.UseValue(false))
                .ForMember(dest => dest.Url,
                           opt => opt.UseValue(default(string)))
                .ForMember(
                    dest => dest.ChildWorkItems,
                    opt => opt.MapFrom(src => MapChildWorkItems(src)));
        }

        private static string GetFieldOrDefault(WorkItem src, string key)
        {
            if (src.Fields.ContainsKey(key))
            {
                return (string) src.Fields[key];
            }

            return string.Empty;
        }

        private static int[] MapChildWorkItems(WorkItem src)
        {
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
            public static string Type => "System.WorkItemType";
            public static string AssignedTo => "System.AssignedTo";
            public static string Title => "System.Title";
            public static string Description => "System.Description";
            public static string State => "System.State";
            public static string Area => "System.AreaPath";
        }
    }
}

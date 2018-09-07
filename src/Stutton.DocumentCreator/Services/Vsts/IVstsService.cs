using System.Collections.Generic;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Vsts
{
    public interface IVstsService
    {
        Task<IResponse<IWorkItem>> GetWorkItemAsync(int id);
        Task<IResponse<IEnumerable<IWorkItem>>> GetWorkItemsAsync(WorkItemQueryModel query);
        Task<IResponse> UpdateWorkItemAsync(int id, string fieldToUpdate, string newValue);
        Task<IResponse> AttachFileToWorkItemAsync(string filePath, int workItemId);
        Task<IResponse<ProfileModel>> GetUserProfileAsync();
        Task<IResponse<IEnumerable<WorkItemFieldModel>>> GetWorkItemFields();
        Task<IResponse<string>> GetWorkItemFieldValue(int id, string field);
        Task<IResponse<IEnumerable<IWorkItem>>> GetChildWorkItems(IWorkItem parent);
    }
}

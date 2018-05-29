using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Tfs
{
    public interface ITfsService
    {
        Task<IResponse<IWorkItem>> GetWorkItemAsync(int id);
        Task<IResponse<IEnumerable<IWorkItem>>> GetWorkItemsAsync(WorkItemQueryModel query);
        Task<IResponse> UpdateWorkItemAsync(int id, string fieldToUpdate, string newValue);
        Task<IResponse> AttachFileToWorkItemAsync(string filePath, int workItemId);
        Task<IResponse<ProfileModel>> GetUserProfileAsync();
        Task<IResponse<IEnumerable<string>>> GetWorkItemFields();
    }
}

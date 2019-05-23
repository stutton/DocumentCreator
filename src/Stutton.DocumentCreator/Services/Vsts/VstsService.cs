using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using AutoMapper;
using Flurl;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Profile;
using Microsoft.VisualStudio.Services.Profile.Client;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Stutton.DocumentCreator.Models;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services.Settings;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Vsts
{
    public class VstsService : IVstsService
    {
        private const string GlobalProfileUrl = "https://app.vssps.visualstudio.com/";

        private readonly ISettingsService _settingsService;
        private readonly IMapper _mapper;

        private VssConnection _connection;
        private VssConnection _profileConnection;

        private IEnumerable<WorkItemFieldModel> _workItemFieldsCache;

        public VstsService(ISettingsService settingsService, IMapper mapper)
        {
            _settingsService = settingsService;
            _mapper = mapper;
        }

        public async Task<IResponse<IWorkItem>> GetWorkItemAsync(int id)
        {
            try
            {
                var connectionResponse = await GetUpdatedVssConnection();

                if (!connectionResponse.Success)
                {
                    return Response<IWorkItem>.FromFailure(connectionResponse.Message);
                }

                var connection = connectionResponse.Value;

                var workItemClient =
                    await connection.GetClientAsync<WorkItemTrackingHttpClient>().ConfigureAwait(false);
                var workItem = await workItemClient.GetWorkItemAsync(id, expand: WorkItemExpand.Relations).ConfigureAwait(false);
                if (workItem?.Id == null)
                {
                    return Response<IWorkItem>.FromFailure($"No work item with ID '{id}' returned");
                }

                var model = _mapper.Map<WorkItemModel>(workItem);
                
                return Response<IWorkItem>.FromSuccess(model);
            }
            catch(Exception ex)
            {
                return Response<IWorkItem>.FromException($"Failure while attempting to retrieve work item '{id}'", ex);
            }
        }

        public async Task<IResponse<IEnumerable<IWorkItem>>> GetWorkItemsAsync(WorkItemQueryModel query)
        {
            try
            {
                var settingsResponse = await _settingsService.GetSettings();

                if (!settingsResponse.Success)
                {
                    return Response< IEnumerable<IWorkItem >>.FromFailure(settingsResponse.Message);
                }

                var settings = settingsResponse.Value;

                var sb = new StringBuilder();
                sb.Append($"SELECT * FROM workitems WHERE [System.AssignedTo] contains '{settings.TfsUserName}'");
                foreach (var expression in query.Expressions.Where(p => p.Field != null))
                {
                    sb.Append(" AND ");

                    sb.Append($"[{expression.Field}] ");

                    sb.Append($"{GetExpressionOperatorString(expression.Operator)} ");

                    if (expression.Operator == WorkItemQueryExpressionOperator.In)
                    {
                        var valueSb = new StringBuilder();
                        valueSb.Append("(");

                        foreach (var value in expression.Values)
                        {
                            valueSb.Append($"'{value.Value}', ");
                        }

                        sb.Append($"{valueSb.ToString().TrimEnd(',', ' ')})");
                    }
                    else
                    {
                        sb.Append($"'{expression.Value}'");
                    }
                }

                var wiql = new Wiql {Query = sb.ToString()};

                var connectionResponse = await GetUpdatedVssConnection();

                if (!connectionResponse.Success)
                {
                    return Response<IEnumerable<IWorkItem>>.FromFailure(connectionResponse.Message);
                }

                var connection = connectionResponse.Value;

                var workItemClient =
                    await connection.GetClientAsync<WorkItemTrackingHttpClient>().ConfigureAwait(false);

                var cancellationSource = new CancellationTokenSource();
                cancellationSource.CancelAfter(TimeSpan.FromSeconds(60));
                var cancellationToken = cancellationSource.Token;

                var queryResult = await workItemClient.QueryByWiqlAsync(wiql, cancellationToken: cancellationToken).ConfigureAwait(false);

                if (!queryResult.WorkItems.Any())
                {
                    return Response<IEnumerable<IWorkItem>>.FromSuccess(new List<IWorkItem>());
                }

                var workItems = await workItemClient.GetWorkItemsAsync(queryResult.WorkItems.Select(w => w.Id),
                    cancellationToken: cancellationToken, expand: WorkItemExpand.Relations).ConfigureAwait(false);

                var result = new List<IWorkItem>();
                foreach (var workItem in workItems)
                {
                    result.Add(_mapper.Map<WorkItemModel>(workItem));
                }

                return Response<IEnumerable<IWorkItem>>.FromSuccess(result);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<IWorkItem>>.FromException($"Failed to get requested work items", ex);
            }
        }

        public Task<IResponse> UpdateWorkItemAsync(int id, string fieldToUpdate, string newValue)
        {
            return UpdateWorkItemAsync(new[] {id}, fieldToUpdate, newValue);
        }

        public async Task<IResponse> UpdateWorkItemAsync(int[] ids, string fieldToUpdate, string newValue)
        {
            try
            {
                if(ids == null || ids.Length == 0)
                {
                    return Response.FromSuccess();
                }

                var connectionResponse = await GetUpdatedVssConnection();
                if (!connectionResponse.Success)
                {
                    return Response<ProfileModel>.FromFailure(connectionResponse.Message);
                }
                var connection = connectionResponse.Value;
                var workItemClient = await connection.GetClientAsync<WorkItemTrackingHttpClient>().ConfigureAwait(false);


                var patchDocument = new JsonPatchDocument
                {
                    new JsonPatchOperation
                    {
                        Operation = Operation.Add,
                        Path = $"/fields/{fieldToUpdate}",
                        Value = newValue
                    }
                };

                foreach (var id in ids)
                {
                    try
                    {
                        await workItemClient.UpdateWorkItemAsync(patchDocument, id).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        throw new WorkItemUpdateException($"Failed to update field '{fieldToUpdate}' on work item '{id}'", ex);
                    }
                }

                return Response.FromSuccess();
            }
            catch (Exception ex)
            {
                return Response.FromException($"Failed to update work item", ex);
            }
        }

        public async Task<IResponse> AttachFileToWorkItemAsync(string filePath, int workItemId)
        {
            try
            {
                var connectionResponse = await GetUpdatedVssConnection();
                if (!connectionResponse.Success)
                {
                    return Response<ProfileModel>.FromFailure(connectionResponse.Message);
                }
                var connection = connectionResponse.Value;
                var workItemClient = await connection.GetClientAsync<WorkItemTrackingHttpClient>().ConfigureAwait(false);

                var fileName = $"{Path.GetFileName(filePath)}.{Path.GetExtension(filePath)}";
                var attachmentReference = await workItemClient.CreateAttachmentAsync(filePath).ConfigureAwait(false);

                var patchDocument = new JsonPatchDocument
                {
                    new JsonPatchOperation
                    {
                        Operation = Operation.Add,
                        Path = "/fields/System.History",
                        Value = $"Attaching file {fileName}"
                    },
                    new JsonPatchOperation
                    {
                        Operation = Operation.Add,
                        Path = "/relations/-",
                        Value = new
                        {
                            rel = "AttachedFile",
                            url = attachmentReference.Url,
                            attributes = new {comment = fileName}
                        }
                    }
                };
                await workItemClient.UpdateWorkItemAsync(patchDocument, workItemId).ConfigureAwait(false);
                return Response.FromSuccess();
            }
            catch (Exception ex)
            {
                return Response.FromException($"Failed to attach document to work item '{workItemId}'", ex);
            }
        }

        public async Task<IResponse<ProfileModel>> GetUserProfileAsync()
        {
            try
            {
                if (_profileConnection == null)
                {
                    _profileConnection = new VssConnection(new Uri(GlobalProfileUrl), new VssClientCredentials());
                }
                
                var profileClient = await _profileConnection.GetClientAsync<ProfileHttpClient>();
                var name = await profileClient.GetDisplayNameAsync(null);
                var avatar = await profileClient.GetAvatarAsync(AvatarSize.Large);

                var result = new ProfileModel {Name = name};
                var imageBytes = avatar.Value;

                BitmapSource image;
                using (var stream = new MemoryStream(imageBytes))
                {
                    image = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                }

                result.ProfilePicture = image;
                return Response<ProfileModel>.FromSuccess(result);
            }
            catch (Exception ex)
            {
                return Response<ProfileModel>.FromException("Failed to get user profile", ex);
            }
        }

        public async Task<IResponse<IEnumerable<WorkItemFieldModel>>> GetWorkItemFields()
        {
            try
            {
                if (_workItemFieldsCache != null)
                {
                    return Response<IEnumerable<WorkItemFieldModel>>.FromSuccess(_workItemFieldsCache);
                }

                var connectionResponse = await GetUpdatedVssConnection();
                if (!connectionResponse.Success)
                {
                    return Response<IEnumerable<WorkItemFieldModel>>.FromFailure(connectionResponse.Message);
                }

                var connection = connectionResponse.Value;
                // Both the GetClientAsync and GetFieldsAsync calls appear to do long running synchronous work
                // so we are pushing them to background threads to prevent locking up the application
                var workItemClient = await Task.Run(async () => await connection.GetClientAsync<WorkItemTrackingHttpClient>().ConfigureAwait(false)).ConfigureAwait(false);
                var result = await Task.Run(async () => await workItemClient.GetFieldsAsync().ConfigureAwait(false)).ConfigureAwait(false);
                _workItemFieldsCache = result.Select(f => new WorkItemFieldModel {Name = f.Name, ReferenceName = f.ReferenceName});
                return Response<IEnumerable<WorkItemFieldModel>>.FromSuccess(_workItemFieldsCache);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<WorkItemFieldModel>>.FromException("Failed to get work item fields", ex);
            }
        }

        public async Task<IResponse<string>> GetWorkItemFieldValue(int id, string field)
        {
            try
            {
                if (field.Equals("System.ID", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Response<string>.FromSuccess(id.ToString());
                }

                var connectionResponse = await GetUpdatedVssConnection();
                if (!connectionResponse.Success)
                {
                    return Response<string>.FromFailure(connectionResponse.Message);
                }

                var connection = connectionResponse.Value;
                var workItemClient =
                    await connection.GetClientAsync<WorkItemTrackingHttpClient>().ConfigureAwait(false);
                var workItem = await workItemClient.GetWorkItemAsync(id, new[] {field});
                if (workItem?.Id == null)
                {
                    return Response<string>.FromFailure($"No work item with ID '{id}' returned");
                }

                var fieldValue = workItem.Fields[field].ToString();
                return Response<string>.FromSuccess(fieldValue);
            }
            catch (Exception ex)
            {
                return Response<string>.FromException($"Failure while attempting to retrieve field '{field}' from work item '{id}'", ex);
            }
        }

        public async Task OpenWorkItemInBrowser(IWorkItem workItem)
        {
            var settingsResponse = await _settingsService.GetSettings();

            if (!settingsResponse.Success)
            {
                return;
            }
            var settings = settingsResponse.Value;
            var tfsUrl = settings.TfsUrl;

            var workItemUrl = Url.Combine(tfsUrl, workItem.Team, "_workitems/edit", workItem.Id.ToString());
            Process.Start(workItemUrl);
        }

        private string GetExpressionOperatorString(WorkItemQueryExpressionOperator op)
        {
            switch (op)
            {
                case WorkItemQueryExpressionOperator.Equals:
                    return "=";
                case WorkItemQueryExpressionOperator.GreaterThan:
                    return ">";
                case WorkItemQueryExpressionOperator.LessThan:
                    return "<";
                case WorkItemQueryExpressionOperator.NotEqual:
                    return "<>"; // Not sure what operator should be used here.
                case WorkItemQueryExpressionOperator.Contains:
                    return "contains";
                case WorkItemQueryExpressionOperator.In:
                    return "in";
                default:
                    throw new ArgumentOutOfRangeException(nameof(op), op, null);
            }
        }

        private async Task<IResponse<VssConnection>> GetUpdatedVssConnection()
        {
            try
            {
                var settingsResponse = await _settingsService.GetSettings();

                if (!settingsResponse.Success)
                {
                    return Response<VssConnection>.FromFailure(settingsResponse.Message);
                }

                var settings = settingsResponse.Value;
                var tfsUrl = settings.TfsUrl;
                var defaultCollection = settingsResponse.Value.TfsDefaultCollection;
                var defaultCollectionUrl = Url.Combine(tfsUrl, defaultCollection);

                var tfsUri = tfsUrl.Contains("dev.azure.com") ? new Uri(tfsUrl) : new Uri(defaultCollectionUrl);

                if (_connection == null || _connection.Uri.AbsoluteUri != defaultCollectionUrl)
                {
                    _connection = new VssConnection(tfsUri, new VssClientCredentials());
                }

                return Response<VssConnection>.FromSuccess(_connection);
            }
            catch (Exception ex)
            {
                return Response<VssConnection>.FromFailure(ex.Message);
            }
        }

        private sealed class WorkItemUpdateException : Exception
        {
            public WorkItemUpdateException(string message)
            :base(message)
            { }

            public WorkItemUpdateException(string message, Exception innerException)
            :base(message, innerException)
            { }
        }
    }
}

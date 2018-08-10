using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Flurl;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Identity.Client;
using Microsoft.VisualStudio.Services.Profile;
using Microsoft.VisualStudio.Services.Profile.Client;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Stutton.DocumentCreator.Models;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services.Settings;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Tfs
{
    public class TfsService : ITfsService
    {
        private const string GlobalProfileUrl = "https://app.vssps.visualstudio.com/";

        private readonly ISettingsService _settingsService;

        private VssConnection _connection;
        private VssConnection _profileConnection;

        public TfsService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
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
                var workItem = await workItemClient.GetWorkItemAsync(id).ConfigureAwait(false);
                if (workItem?.Id == null)
                {
                    //_logger.Warn($"Getting work item '{_workItemId}' returned null");
                    return Response<IWorkItem>.FromFailure($"No work item with ID '{id}' returned");
                }

                var model = TfsWorkItemMapper.MapToModel(workItem);

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
                foreach (var expression in query.Expressions)
                {
                    sb.Append(" AND ");

                    sb.Append($"[{expression.Field.ReferenceName}] ");

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
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                var result = new List<IWorkItem>();
                foreach (var workItem in workItems)
                {
                    result.Add(TfsWorkItemMapper.MapToModel(workItem));
                }

                return Response<IEnumerable<IWorkItem>>.FromSuccess(result);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<IWorkItem>>.FromException($"Failed to get requested work items", ex);
            }
        }

        public async Task<IResponse> UpdateWorkItemAsync(int id, string fieldToUpdate, string newValue)
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

                var patchDocument = new JsonPatchDocument
                {
                    new JsonPatchOperation
                    {
                        Operation = Operation.Add,
                        Path = $"/fields/{fieldToUpdate}",
                        Value = newValue
                    }
                };

                await workItemClient.UpdateWorkItemAsync(patchDocument, id).ConfigureAwait(false);
                return Response.FromSuccess();
            }
            catch (Exception ex)
            {
                return Response.FromException($"Failed to update field '{fieldToUpdate}' to work item '{id}'", ex);
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
                var connectionResponse = await GetUpdatedVssConnection();
                if (!connectionResponse.Success)
                {
                    return Response<IEnumerable<WorkItemFieldModel>>.FromFailure(connectionResponse.Message);
                }

                var connection = connectionResponse.Value;
                var workItemClient = await connection.GetClientAsync<WorkItemTrackingHttpClient>().ConfigureAwait(false);
                
                var result = await workItemClient.GetFieldsAsync().ConfigureAwait(false);
                return Response<IEnumerable<WorkItemFieldModel>>.FromSuccess(result.Select(f => new WorkItemFieldModel{ Name = f.Name, ReferenceName = f.ReferenceName}).ToList());
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<WorkItemFieldModel>>.FromException("Failed to get work item fields", ex);
            }
        }

        public async Task<IResponse<string>> GetWorkItemFieldValue(int id, WorkItemFieldModel field)
        {
            try
            {
                if (field.Name.Equals("ID", StringComparison.InvariantCultureIgnoreCase))
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
                var workItem = await workItemClient.GetWorkItemAsync(id, new[] {field.ReferenceName});
                if (workItem?.Id == null)
                {
                    return Response<string>.FromFailure($"No work item with ID '{id}' returned");
                }

                var fieldValue = workItem.Fields[field.ReferenceName].ToString();
                return Response<string>.FromSuccess(fieldValue);
            }
            catch (Exception ex)
            {
                return Response<string>.FromException($"Failure while attempting to retrieve field '{field}' from work item '{id}'", ex);
            }
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

                if (_connection == null || _connection.Uri.AbsoluteUri != defaultCollectionUrl)
                {
                    _connection = new VssConnection(new Uri(defaultCollectionUrl), new VssClientCredentials());
                }

                return Response<VssConnection>.FromSuccess(_connection);
            }
            catch (Exception ex)
            {
                return Response<VssConnection>.FromFailure(ex.Message);
            }
        }
    }
}

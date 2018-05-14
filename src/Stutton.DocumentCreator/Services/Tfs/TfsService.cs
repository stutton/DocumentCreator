using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.WebApi;
using Stutton.DocumentCreator.Models;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services.Settings;

namespace Stutton.DocumentCreator.Services.Tfs
{
    public class TfsService : ITfsService
    {
        private readonly ISettingsService _settingsService;

        public TfsService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task<IResponse<IWorkItem>> GetWorkItemAsync(int id)
        {
            var connectionResponse = await GetUpdatedVssConnection();

            if (!connectionResponse.Success)
            {
                return Response<IWorkItem>.FromFailure(connectionResponse.Message);
            }

            var connection = connectionResponse.Value;

            var workItemClient = await connection.GetClientAsync<WorkItemTrackingHttpClient>().ConfigureAwait(false);
            var workItem = await workItemClient.GetWorkItemAsync(id).ConfigureAwait(false);
            if (workItem?.Id == null)
            {
                //_logger.Warn($"Getting work item '{_workItemId}' returned null");
                return Response<IWorkItem>.FromFailure($"No work item {id} returned");
            }

            var model = TfsWorkItemMapper.MapToModel(workItem);

            return Response<IWorkItem>.FromSuccess(model);
        }

        public async Task<IResponse<IEnumerable<IWorkItem>>> GetWorkItemsAsync(WorkItemQueryModel query)
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append("SELECT * FROM workitems WHERE ");
                var prependAnd = false;
                foreach (var expression in query.Expressions)
                {
                    if (prependAnd)
                    {
                        sb.Append("AND ");
                    }

                    sb.Append($"[{expression.FieldName}] ");

                    sb.Append($"{GetExpressionOperatorString(expression.Operator)} ");

                    if (expression.Operator == WorkItemQueryExpressionOperator.In)
                    {
                        var valueSb = new StringBuilder();
                        valueSb.Append("(");

                        foreach (var value in expression.Values)
                        {
                            valueSb.Append($"'{value}', ");
                        }

                        sb.Append($"{valueSb.ToString().TrimEnd(',', ' ')})");
                    }
                    else
                    {
                        sb.Append($"'{expression.Values.FirstOrDefault()}' ");
                    }
                    
                    prependAnd = true;
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
                return Response<IEnumerable<IWorkItem>>.FromFailure($"Failed to get work items: {ex.Message}");
            }
        }

        public Task<IResponse> UpdateWorkItemAsync(int id, string fieldToUpdate, string newValue)
        {
            throw new NotImplementedException();
        }

        public Task<IResponse> AttachFileToWorkItemAsync(string filePath, int workItemId)
        {
            throw new NotImplementedException();
        }

        public Task<IResponse<ProfileModel>> GetUserProfileAsync()
        {
            throw new NotImplementedException();
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

                var tfsUrl = settingsResponse.Value.TfsUrl;
                var defaultCollection = settingsResponse.Value.TfsDefaultCollection;

                var defaultCollectionUrl = Url.Combine(tfsUrl, defaultCollection);

                var connection = new VssConnection(new Uri(defaultCollectionUrl), new VssClientCredentials());
                return Response<VssConnection>.FromSuccess(connection);
            }
            catch (Exception ex)
            {
                return Response<VssConnection>.FromFailure(ex.Message);
            }
        }
    }
}

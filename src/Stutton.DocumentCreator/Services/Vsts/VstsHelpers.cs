using Stutton.DocumentCreator.Models.WorkItems;
using System;

namespace Stutton.DocumentCreator.Services.Vsts
{
    internal static class VstsHelpers
    {
        public static string GetExpressionOperatorString(WorkItemQueryExpressionOperator op)
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
                    return "<>";
                case WorkItemQueryExpressionOperator.Contains:
                    return "contains";
                case WorkItemQueryExpressionOperator.In:
                    return "in";
                default:
                    throw new ArgumentOutOfRangeException(nameof(op), op, null);
            }
        }
    }
}

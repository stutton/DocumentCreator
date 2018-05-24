using System;
using Stutton.DocumentCreator.Services;

namespace Stutton.DocumentCreator.Shared
{
    public interface IResponse
    {
        bool Success { get; }
        ResponseCode Code { get; }
        string Message { get; }
        string CallerMemberName { get; }
        int LineNumber { get; }
        Exception Exception { get; }
        Guid Id { get; }
    }

    public interface IResponse<out T> : IResponse
    {
        T Value { get; }
    }
}

using Stutton.DocumentCreator.Services;

namespace Stutton.DocumentCreator.Shared
{
    public interface IResponse
    {
        bool Success { get; }
        ResponseCode Code { get; }
        string Message { get; }
    }

    public interface IResponse<out T>
    {
        bool Success { get; }
        ResponseCode Code { get; }
        string Message { get; }
        T Value { get; }
    }
}

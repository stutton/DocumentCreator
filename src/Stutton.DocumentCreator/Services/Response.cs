namespace Stutton.DocumentCreator.Services
{
    public class Response : IResponse
    {
        public static Response FromSuccess()
        {
            return new Response(true, string.Empty, ResponseCode.Ok);
        }

        public static Response FromFailure(string message)
        {
            return new Response(false, message, ResponseCode.Error);
        }

        public static Response FromFailure(string message, ResponseCode code)
        {
            return new Response(false, message, code);
        }

        private Response(bool success, string message, ResponseCode code)
        {
            Success = success;
            Message = message;
            Code = code;
        }

        public bool Success { get; }
        public ResponseCode Code { get; }
        public string Message { get; }
    }
    public class Response<T> : IResponse<T>
    {
        public static Response<T> FromSuccess(T value)
        {
            return new Response<T>(true, string.Empty, ResponseCode.Ok, value);
        }

        public static Response<T> FromFailure(string message)
        {
            return new Response<T>(false, message, ResponseCode.Error, default(T));
        }

        public static Response<T> FromFailure(string message, ResponseCode code)
        {
            return new Response<T>(false, message, code, default(T));
        }

        private Response(bool success, string message, ResponseCode code, T value)
        {
            Success = success;
            Message = message;
            Code = code;
            Value = value;
        }

        public bool Success { get; }
        public ResponseCode Code { get; }
        public string Message { get; }
        public T Value { get; }
    }
}

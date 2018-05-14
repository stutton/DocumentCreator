namespace Stutton.DocumentCreator.Services
{
    public class Response : IResponse
    {
        public static Response FromSuccess()
        {
            return new Response(true, string.Empty);
        }

        public static Response FromFailure(string message)
        {
            return new Response(false, message);
        }

        private Response(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public bool Success { get; }
        public string Message { get; }
    }
    public class Response<T> : IResponse<T>
    {
        public static Response<T> FromSuccess(T value)
        {
            return new Response<T>(true, string.Empty, value);
        }

        public static Response<T> FromFailure(string message)
        {
            return new Response<T>(false, message, default(T));
        }

        private Response(bool success, string message, T value)
        {
            Success = success;
            Message = message;
            Value = value;
        }

        public bool Success { get; }
        public string Message { get; }
        public T Value { get; }
    }
}

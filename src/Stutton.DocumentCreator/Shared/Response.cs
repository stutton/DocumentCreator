using System;
using System.Runtime.CompilerServices;
using Stutton.DocumentCreator.Services;

namespace Stutton.DocumentCreator.Shared
{
    public class Response : IResponse
    {
        public static Response FromSuccess()
        {
            return new Response(true, string.Empty, ResponseCode.Ok, "None", 0, null);
        }

        public static Response FromFailure(string message, 
            [CallerMemberName] string callerMemberName = "Unknown", 
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return new Response(false, message, ResponseCode.Error, callerMemberName, callerLineNumber, null);
        }

        public static Response FromFailure(string message, 
            ResponseCode code, 
            [CallerMemberName] string callerMemberName = "Unknown", 
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return new Response(false, message, code, callerMemberName, callerLineNumber, null);
        }

        public static Response FromException(string message,
            Exception exception,
            [CallerMemberName] string callerMemberName = "Unknown",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return new Response(false, message, ResponseCode.Exception, callerMemberName, callerLineNumber, exception);
        }

        private Response(bool success, string message, ResponseCode code, string callerMemberName, int callerLineNumber, Exception exception)
        {
            Success = success;
            Message = message;
            Code = code;
            CallerMemberName = callerMemberName;
            LineNumber = callerLineNumber;
            Exception = exception;
        }

        public bool Success { get; }
        public ResponseCode Code { get; }
        public string Message { get; }
        public string CallerMemberName { get; }
        public int LineNumber { get; }
        public Exception Exception { get; }
        public Guid Id { get; } = Guid.NewGuid();
    }
    public class Response<T> : IResponse<T>
    {
        public static Response<T> FromSuccess(T value)
        {
            return new Response<T>(true, string.Empty, ResponseCode.Ok, value, "None", 0, null);
        }

        public static Response<T> FromFailure(string message,
            [CallerMemberName] string callerMemberName = "Unknown",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return new Response<T>(false, message, ResponseCode.Error, default(T), callerMemberName, callerLineNumber, null);
        }

        public static Response<T> FromFailure(string message, 
            ResponseCode code,
            [CallerMemberName] string callerMemberName = "Unknown",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return new Response<T>(false, message, code, default(T), callerMemberName, callerLineNumber, null);
        }

        public static Response<T> FromException(string message,
            Exception exception,
            [CallerMemberName] string callerMemberName = "Unknown",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return new Response<T>(false, message, ResponseCode.Exception, default(T), callerMemberName, callerLineNumber, exception);
        }

        private Response(bool success, string message, ResponseCode code, T value, string callerMemberName, int callerLineNumber, Exception exception)
        {
            Success = success;
            Message = message;
            Code = code;
            Value = value;
            CallerMemberName = callerMemberName;
            LineNumber = callerLineNumber;
            Exception = exception;
        }

        public bool Success { get; }
        public ResponseCode Code { get; }
        public string Message { get; }
        public T Value { get; }
        public string CallerMemberName { get; }
        public int LineNumber { get; }
        public Exception Exception { get; }
        public Guid Id { get; } = Guid.NewGuid();
    }
}

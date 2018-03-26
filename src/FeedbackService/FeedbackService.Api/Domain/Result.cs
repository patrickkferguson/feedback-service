using System.Collections.Generic;
using System.Linq;

namespace FeedbackService.Api.Domain
{
    public class Result
    {
        protected Result()
        {
        }

        public bool Success { get; protected set; }

        public ErrorCodes ErrorCodes { get; protected set; }

        public string[] Errors { get; protected set; }

        public static Result SuccessResult()
        {
            return new Result()
            {
                Success = true,
                ErrorCodes = ErrorCodes.None
            };
        }

        public static Result ErrorResult(ErrorCodes errorCodes)
        {
            return ErrorResult(errorCodes, new string[0]);
        }

        public static Result ErrorResult(ErrorCodes errorCodes, IEnumerable<string> errors)
        {
            return new Result()
            {
                Success = false,
                ErrorCodes = errorCodes,
                Errors = errors.ToArray()
            };
        }
    }

    public class Result<T> : Result
    {
        private Result()
        {
        }

        public T Data { get; private set; }

        public static Result SuccessResult(T data)
        {
            return new Result<T>()
            {
                Success = true,
                ErrorCodes = ErrorCodes.None,
                Data = data
            };
        }
    }
}

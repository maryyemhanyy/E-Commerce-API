using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Common
{
    public class Result
    {
        public bool IsSuccess { get;}
        public IReadOnlyList<Error> Errors { get; }

        protected Result(bool isSuccess, IReadOnlyList<Error> errors)
        {
            IsSuccess = isSuccess;
            Errors = errors;
        }

        public static Result Ok() => new Result(true, Array.Empty<Error>());
        public static Result Fail(Error error) => new Result(false, new[] { error });
        public static Result Fail(IReadOnlyList<Error> errors) => new Result(false, errors);


    }
    public class Result<T> : Result
    {
        private readonly T _value;
        public T data => IsSuccess ? _value : default;
        private Result( T value) : base(true, Array.Empty<Error>())
        {
            _value = value;
        }
        private Result(Error error) : base(false, new[] { error })
        {
            _value = default!;
        }
        private Result(IReadOnlyList<Error> errors) : base(false, errors)
        {
            _value = default!;
        }
        public static Result<T> Ok(T value) => new Result<T>(value);
        public static new Result<T> Fail(Error error) => new Result<T>(error);
        public static new Result<T> Fail(IReadOnlyList<Error> errors) => new Result<T>(errors);
    }
}

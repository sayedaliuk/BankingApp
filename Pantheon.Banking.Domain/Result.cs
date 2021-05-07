using System.Collections.Generic;

namespace Pantheon.Banking.Domain
{
    public class Result
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }

        public Result(bool success, IEnumerable<string> errors = null)
        {
            this.Success = success;
            this.Errors = errors ?? new string[0];
        }
    }

    public class Result<T> : Result
    {
        public T Data { get; set; }

        public Result(bool success, T data, IEnumerable<string> errors = null)
            : base(success, errors)
        {
            this.Data = data;
        }
    }
}

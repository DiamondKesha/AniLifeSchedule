namespace AniLifeSchedule.Models.Wrapper
{
    public class Result : IResult
    {
        public List<string> Messages { get; set; } = new List<string>();

        public bool Succeeded { get; set; }

        public static Result Success()
        {
            return new Result { Succeeded = true };
        }
        public static Result Success(string message)
        {
            return new Result { Succeeded = true, Messages = new List<string> { message } };
        }

        public static Result Success(List<string> messages)
        {
            return new Result { Succeeded = true, Messages = messages };
        }

        public static Result Fail()
        {
            return new Result { Succeeded = false};
        }

        public static Result Fail(string message)
        {
            return new Result { Succeeded = false, Messages = new List<string> { message } };
        }

        public static Result Fail(List<string> messages)
        {
            return new Result { Succeeded = false, Messages = messages };
        }
    }

    public class Result<T> : Result, IResult<T>
    {

        public T Data { get; set; }

        public static Result<T> Success(T data)
        {
            return new Result<T> { Succeeded = true, Data = data };
        }

        public static Result<T> Success(T data, string message)
        {
            return new Result<T> { Succeeded = true, Data = data, Messages = new List<string> { message } };
        }

        public static Result<T> Success(T data, List<string> messages)
        {
            return new Result<T> { Succeeded = true, Data = data, Messages = messages };
        }

        public static Result<T> Fail(T data)
        {
            return new Result<T> { Succeeded = false, Data = data };
        }

        public static Result<T> Fail(T data, string message)
        {
            return new Result<T> { Succeeded = false, Data = data, Messages = new List<string> { message } };
        }

        public static Result<T> Fail(T data, List<string> messages)
        {
            return new Result<T> { Succeeded = false, Data = data, Messages = messages };
        }
    }
}

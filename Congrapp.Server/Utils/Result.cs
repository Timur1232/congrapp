namespace Congrapp.Server.Utils;

public class Result<T, E> 
{
    public Result(T value, E error, bool isValid)
    {
        Value = value;
        Error = error;
        IsValid = isValid;
    }

    public T Value { get; }
    public E Error { get; }
    public bool IsValid { get; }
    
    public static Result<T, E> Ok(T value)
    {
        return new Result<T, E>(value, default!, true);
    }

    public static Result<T, E> Err(E error)
    {
        return new Result<T, E>(default!, error, false);
    }
}
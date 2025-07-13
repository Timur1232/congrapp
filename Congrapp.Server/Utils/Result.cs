namespace Congrapp.Server.Utils;

public class Result<T, E>(T value, E error, bool isValid)
{
    public T Value { get; } = value;
    public E Error { get; } = error;
    public bool IsValid { get; } = isValid;

    public static Result<T, E> Ok(T value)
    {
        return new Result<T, E>(value, default!, true);
    }

    public static Result<T, E> Err(E error)
    {
        return new Result<T, E>(default!, error, false);
    }
}
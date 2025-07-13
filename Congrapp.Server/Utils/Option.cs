namespace Congrapp.Server.Utils;

public class Option<T>(T value, bool isValid)
{
    public T Value { get; } = value;
    public bool IsValid { get; } = isValid;

    public static Option<T> Some(T value)
    {
        return new Option<T>(value, true);
    }

    public static Option<T> None()
    {
        return new Option<T>(default!, false);
    }
}
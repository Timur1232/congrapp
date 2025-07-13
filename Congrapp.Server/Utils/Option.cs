namespace Congrapp.Server.Utils;

public class Option<T>
{
    public Option(T value, bool isValid)
    {
        Value = value;
        IsValid = isValid;
    }

    public T Value { get; }
    public bool IsValid { get; }

    public static Option<T> Some(T value)
    {
        return new Option<T>(value, true);
    }

    public static Option<T> None()
    {
        return new Option<T>(default!, false);
    }
}
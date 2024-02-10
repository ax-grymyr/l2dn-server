namespace L2Dn.GameServer.Utilities;

public static class Objects
{
    public static void requireNonNull(object? obj, string error)
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj), error);
    }

    public static void requireNonNull(object? obj)
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));
    }
}
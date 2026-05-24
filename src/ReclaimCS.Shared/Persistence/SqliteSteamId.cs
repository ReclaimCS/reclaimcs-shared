namespace ReclaimCS.Shared.Persistence;

public static class SqliteSteamId
{
    public static long ToSigned(ulong value)
    {
        return unchecked((long)value);
    }
}

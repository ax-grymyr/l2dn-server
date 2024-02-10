namespace L2Dn.AuthServer.Model;

internal sealed class GameServerInfoComparer: IComparer<GameServerInfo>
{
    public int Compare(GameServerInfo? x, GameServerInfo? y)
    {
        if (ReferenceEquals(x, y))
            return 0;
        
        if (ReferenceEquals(null, y))
            return 1;
        
        if (ReferenceEquals(null, x))
            return -1;
        
        return x.ServerId.CompareTo(y.ServerId);
    }
}
namespace L2Dn.GameServer.Enums;

public enum PlayerAction
{
    ADMIN_COMMAND,
    ADMIN_POINT_PICKING,
    ADMIN_SHOW_TERRITORY,
    MERCENARY_CONFIRM,
    OFFLINE_PLAY
}

public static class PlayerActionUtil
{
    public static int getMask(this PlayerAction value)
    {
        return 1 << (int)value;
    }
}
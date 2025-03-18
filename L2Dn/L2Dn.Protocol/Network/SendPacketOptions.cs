namespace L2Dn.Network;

[Flags]
public enum SendPacketOptions
{
    None = 0,
    CloseAfterSending = 1,
    DontEncrypt = 2,
    NoPadding = 4,
}
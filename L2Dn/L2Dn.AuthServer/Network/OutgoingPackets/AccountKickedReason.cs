namespace L2Dn.AuthServer.Network.OutgoingPackets;

internal enum AccountKickedReason
{
    DataStealer = 0x01,
    GenericViolation = 0x08,
    SuspendedFor7Days = 0x10,
    PermanentlyBanned = 0x20,
}

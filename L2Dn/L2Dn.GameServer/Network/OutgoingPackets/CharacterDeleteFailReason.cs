namespace L2Dn.GameServer.Network.OutgoingPackets;

internal enum CharacterDeleteFailReason
{
    None,
    Unknown,
    PledgeMember,
    PledgeMaster,
    ProhibitCharDeletion,
    Commission,
    Mentor,
    Mentee,
    Mail
}
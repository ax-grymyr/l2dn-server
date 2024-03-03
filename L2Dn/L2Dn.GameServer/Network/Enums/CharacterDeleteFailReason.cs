namespace L2Dn.GameServer.Network.OutgoingPackets;

public enum CharacterDeleteFailReason
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
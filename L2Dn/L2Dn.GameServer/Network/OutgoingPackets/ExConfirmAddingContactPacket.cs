using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExConfirmAddingContactPacket: IOutgoingPacket
{
    private readonly string _charName;
    private readonly bool _added;
	
    public ExConfirmAddingContactPacket(string charName, bool added)
    {
        _charName = charName;
        _added = added;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_AGIT_AUCTION_CMD); // TODO: wrong code?
        
        writer.WriteString(_charName);
        writer.WriteInt32(_added);
    }
}
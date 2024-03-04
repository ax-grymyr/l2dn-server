using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowContactListPacket: IOutgoingPacket
{
    private readonly Set<String> _contacts;
	
    public ExShowContactListPacket(Player player)
    {
        _contacts = player.getContactList().getAllContacts();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RECEIVE_SHOW_POST_FRIEND);
        
        writer.WriteInt32(_contacts.size());
        foreach (string contact in _contacts)
        {
            writer.WriteString(contact);
        }
    }
}
using L2Dn.GameServer.InstanceManagers;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExCursedWeaponListPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CURSED_WEAPON_LIST);
        
        ICollection<int> ids = CursedWeaponsManager.getInstance().getCursedWeaponsIds();
        writer.WriteInt32(ids.Count);
        foreach (int id in ids)
        {
            writer.WriteInt32(id);
        }
    }
}
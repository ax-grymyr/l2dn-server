using L2Dn.GameServer.Model;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExCursedWeaponLocationPacket: IOutgoingPacket
{
    private readonly List<CursedWeaponInfo> _cursedWeaponInfo;
	
    public ExCursedWeaponLocationPacket(List<CursedWeaponInfo> cursedWeaponInfo)
    {
        _cursedWeaponInfo = cursedWeaponInfo;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CURSED_WEAPON_LOCATION);
        
        if (!_cursedWeaponInfo.isEmpty())
        {
            writer.WriteInt32(_cursedWeaponInfo.size());
            foreach (CursedWeaponInfo w in _cursedWeaponInfo)
            {
                writer.WriteInt32(w.id);
                writer.WriteInt32(w.activated);
                writer.WriteInt32(w.pos.getX());
                writer.WriteInt32(w.pos.getY());
                writer.WriteInt32(w.pos.getZ());
            }
        }
        else
        {
            writer.WriteInt32(0);
        }
    }
	
    public class CursedWeaponInfo(Location p, int cwId, int status)
    {
        public Location pos = p;
        public int id = cwId;
        public int activated = status; // 0 - not activated ? 1 - activated
    }
}
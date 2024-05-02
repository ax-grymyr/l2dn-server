using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
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
            writer.WriteInt32(_cursedWeaponInfo.Count);
            foreach (CursedWeaponInfo w in _cursedWeaponInfo)
            {
                writer.WriteInt32(w.id);
                writer.WriteInt32(w.activated);
                writer.WriteLocation3D(w.pos);
            }
        }
        else
        {
            writer.WriteInt32(0);
        }
    }

    public class CursedWeaponInfo(Location3D p, int cwId, int status)
    {
        public Location3D pos = p;
        public int id = cwId;
        public int activated = status; // 0 - not activated ? 1 - activated
    }
}
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestCursedWeaponLocationPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        List<ExCursedWeaponLocationPacket.CursedWeaponInfo> list = new();
        foreach (CursedWeapon cw in CursedWeaponsManager.getInstance().getCursedWeapons())
        {
            if (!cw.isActive())
            {
                continue;
            }
			
            Location pos = cw.getWorldPosition();
            if (pos != null)
            {
                list.Add(new ExCursedWeaponLocationPacket.CursedWeaponInfo(pos.ToLocation3D(), cw.getItemId(), cw.isActivated() ? 1 : 0));
            }
        }
		
        // send the ExCursedWeaponLocation
        if (list.Count != 0)
        {
            player.sendPacket(new ExCursedWeaponLocationPacket(list));
        }

        return ValueTask.CompletedTask;
    }
}
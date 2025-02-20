using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Revenges;

public struct RequestExPvpBookShareRevengeReqShareRevengeInfoPacket: IIncomingPacket<GameSession>
{
    private string _victimName;
    private string _killerName;
    private int _type;

    public void ReadContent(PacketBitReader reader)
    {
        _victimName = reader.ReadSizedString();
        _killerName = reader.ReadSizedString();
        _type = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (!_victimName.equals(player.getName()))
            return ValueTask.CompletedTask;

        Player? killer = World.getInstance().getPlayer(_killerName);
        if (killer == null || !killer.isOnline())
        {
            SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_OFFLINE);
            sm.Params.addString(_killerName);
            player.sendPacket(sm);
            return ValueTask.CompletedTask;
        }

        RevengeHistoryManager.getInstance().requestHelp(player, killer, _type);

        return ValueTask.CompletedTask;
    }
}
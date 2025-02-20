using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Henna;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.NewHenna;

public struct RequestNewHennaUnequipInfoPacket: IIncomingPacket<GameSession>
{
    private int _hennaId;

    public void ReadContent(PacketBitReader reader)
    {
        _hennaId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null || _hennaId == 0)
            return ValueTask.CompletedTask;

        Henna? henna = null;
        for (int slot = 1; slot <= 4; slot++)
        {
            Henna? equipedHenna = player.getHenna(slot);
            if (equipedHenna != null && equipedHenna.getDyeId() == _hennaId)
            {
                henna = equipedHenna;
                break;
            }
        }

        if (henna == null)
        {
            PacketLogger.Instance.Warn("Invalid Henna Id: " + _hennaId + " from " + player);
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        player.sendPacket(new HennaItemRemoveInfoPacket(henna, player));

        return ValueTask.CompletedTask;
    }
}
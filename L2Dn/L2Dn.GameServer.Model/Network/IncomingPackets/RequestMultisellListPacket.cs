using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestMultisellListPacket: IIncomingPacket<GameSession>
{
    private int _multiSellId;

    public void ReadContent(PacketBitReader reader)
    {
        _multiSellId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        MultisellListHolder? multisell = MultisellData.getInstance().getMultisell(_multiSellId);
        if (multisell == null)
        {
            PacketLogger.Instance.Warn("RequestMultisellList: " + player + " requested non-existent list " +
                                       _multiSellId + ".");

            return ValueTask.CompletedTask;
        }

        MultisellData.getInstance().separateAndSend(_multiSellId, player, null, false, null, null, 4);
        return ValueTask.CompletedTask;
    }
}
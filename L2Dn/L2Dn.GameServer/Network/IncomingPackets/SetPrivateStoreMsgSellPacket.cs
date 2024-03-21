using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct SetPrivateStoreMsgSellPacket: IIncomingPacket<GameSession>
{
    private const int MAX_MSG_LENGTH = 29;
	
    private string _storeMsg;

    public void ReadContent(PacketBitReader reader)
    {
        _storeMsg = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null || player.getSellList() == null)
            return ValueTask.CompletedTask;
		
        if (_storeMsg != null && _storeMsg.Length > MAX_MSG_LENGTH)
        {
            Util.handleIllegalPlayerAction(player, player + " tried to overflow private store sell message",
                Config.DEFAULT_PUNISH);
            
            return ValueTask.CompletedTask;
        }
		
        player.getSellList().setTitle(_storeMsg ?? string.Empty);
        player.sendPacket(new PrivateStoreMsgSellPacket(player));

        return ValueTask.CompletedTask;
    }
}
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestChangeNicknameColorPacket: IIncomingPacket<GameSession>
{
    private static readonly int[] COLORS =
    {
        0x9393FF, // Pink
        0x7C49FC, // Rose Pink
        0x97F8FC, // Lemon Yellow
        0xFA9AEE, // Lilac
        0xFF5D93, // Cobalt Violet
        0x00FCA0, // Mint Green
        0xA0A601, // Peacock Green
        0x7898AF, // Yellow Ochre
        0x486295, // Chocolate
        0x999999, // Silver
    };
	
    private int _colorNum;
    private int _itemId;
    private string _title;

    public void ReadContent(PacketBitReader reader)
    {
        _colorNum = reader.ReadInt32();
        _title = reader.ReadString();
        _itemId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_colorNum < 0 || _colorNum >= COLORS.Length)
            return ValueTask.CompletedTask;
		
        Item item = player.getInventory().getItemByItemId(_itemId);
        if (item == null || item.getEtcItem() == null || item.getEtcItem().getHandlerName() == null ||
            !item.getEtcItem().getHandlerName().equalsIgnoreCase("NicknameColor"))
        {
            return ValueTask.CompletedTask;
        }

        if (player.destroyItem("Consume", item, 1, null, true))
        {
            player.setTitle(_title);
            player.getAppearance().setTitleColor(new Color(COLORS[_colorNum]));
            player.broadcastUserInfo();
        }
        
        return ValueTask.CompletedTask;
    }
}
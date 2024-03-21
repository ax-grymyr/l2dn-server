using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestChangeNicknameEmotePacket: IIncomingPacket<GameSession>
{
    private const int ESPECIAL_COLOR_TITLE_EMOTE = 95892;
    private const int ESPECIAL_COLOR_TITLE_SEALED = 94764;
    private const int ESPECIAL_STYLISH_COLOR_TITLE = 49662;
    private static readonly int[] COLORS =
    {
        0x9393FF, // Pink 1
        0x7C49FC, // Rose Pink 2
        0x97F8FC, // Yellow 3
        0xFA9AEE, // Lilac 4
        0xFF5D93, // Cobalt Violet 5
        0x00FCA0, // Mint Green 6
        0xA0A601, // Peacock Green 7
        0x7898AF, // Ochre 8
        0x486295, // Chocolate 9
        0x999999, // Silver 10 ** good here
        0xF3DC09, // SkyBlue 11
        0x05D3F6, // Gold 12
        0x3CB1F4, // Orange 13
        0xF383F3, // Pink 14
        0x0909F3, // Red 15
        0xF3DC09, // SkyBlue 16
        0x000000, // dummy
    };

    private int _colorNum;
    private int _itemId;
    private String _title;

    public void ReadContent(PacketBitReader reader)
    {
        _itemId = reader.ReadInt32();
        _colorNum = reader.ReadInt32();
        _title = reader.ReadSizedString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Item item = player.getInventory().getItemByItemId(_itemId);
        if (item == null || item.getEtcItem() == null || item.getEtcItem().getHandlerName() == null ||
            !item.getEtcItem().getHandlerName().equalsIgnoreCase("NicknameColor"))
        {
            return ValueTask.CompletedTask;
        }

        if (_colorNum < 0 || _colorNum >= COLORS.Length)
            return ValueTask.CompletedTask;
		
        if (_title.contains("{"))
        {
            player.sendMessage("Cannot use this type of characters {}");
            return ValueTask.CompletedTask;
        }

        if ((_itemId == ESPECIAL_COLOR_TITLE_EMOTE || _itemId == ESPECIAL_COLOR_TITLE_SEALED ||
             _itemId == ESPECIAL_STYLISH_COLOR_TITLE) && player.destroyItem("Consume", item, 1, null, true))
        {
            player.setTitle(_title);
            player.getAppearance().setTitleColor(new Color(COLORS[_colorNum - 1]));
            player.broadcastUserInfo();
            player.sendPacket(SystemMessageId.YOUR_TITLE_HAS_BEEN_CHANGED);
            return ValueTask.CompletedTask;
        }

        if (player.destroyItem("Consume", item, 1, null, true))
        {
            int skyblue = _colorNum - 2;
            if (skyblue > 11 && player.getLevel() >= 90)
            {
                skyblue = 15;
            }
			
            player.setTitle(_title);
            player.sendPacket(SystemMessageId.YOUR_TITLE_HAS_BEEN_CHANGED);
            player.getAppearance().setTitleColor(new Color(COLORS[skyblue]));
            player.broadcastUserInfo();
        }
        
        return ValueTask.CompletedTask;
    }
}
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Network.OutgoingPackets.WorldExchange;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.WorldExchange;

public struct ExWorldExchangeItemListPacket: IIncomingPacket<GameSession>
{
    private WorldExchangeItemSubType _category;
    private WorldExchangeSortType _sortType;
    private List<int>? _itemIdList;

    public void ReadContent(PacketBitReader reader)
    {
        _category = (WorldExchangeItemSubType)reader.ReadInt16();
        _sortType = (WorldExchangeSortType)reader.ReadByte();
        reader.ReadInt32(); // page
        
        int size = reader.ReadInt32();
        if (size > 0 && size < 20000)
        {
            _itemIdList = new List<int>(size);
            for (int i = 0; i < size; i++)
                _itemIdList.Add(reader.ReadInt32());
        }
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        if (!Config.ENABLE_WORLD_EXCHANGE)
            return ValueTask.CompletedTask;

        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        string lang = Config.MULTILANG_ENABLE
            ? player.getLang() != null ? player.getLang() : Config.WORLD_EXCHANGE_DEFAULT_LANG
            : Config.WORLD_EXCHANGE_DEFAULT_LANG;
        
        if (_itemIdList == null)
        {
            List<WorldExchangeHolder> holders = WorldExchangeManager.getInstance()
                .getItemBids(player.getObjectId(), _category, _sortType, lang);
            
            player.sendPacket(new WorldExchangeItemListPacket(holders, _category));
        }
        else
        {
            WorldExchangeManager.getInstance().addCategoryType(_itemIdList, (int)_category);
            List<WorldExchangeHolder> holders =
                WorldExchangeManager.getInstance().getItemBids(_itemIdList, _sortType, lang);
            
            player.sendPacket(new WorldExchangeItemListPacket(holders, _category));
        }

        return ValueTask.CompletedTask;
    }
}
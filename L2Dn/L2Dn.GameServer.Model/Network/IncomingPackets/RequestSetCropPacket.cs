using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestSetCropPacket: IIncomingPacket<GameSession>
{
    private const int BATCH_LENGTH = 21; // length of the one item

    private int _manorId;
    private List<CropProcure>? _items;

    public void ReadContent(PacketBitReader reader)
    {
        _manorId = reader.ReadInt32();
        int count = reader.ReadInt32();
        if (count <= 0 || count > Config.MAX_ITEM_IN_PACKET || count * BATCH_LENGTH != reader.Length)
        {
            return;
        }

        _items = new(count);
        for (int i = 0; i < count; i++)
        {
            int itemId = reader.ReadInt32();
            long sales = reader.ReadInt64();
            long price = reader.ReadInt64();
            int type = reader.ReadByte();
            if (itemId < 1 || sales < 0 || price < 0)
            {
                _items = null;
                return;
            }

            if (sales > 0)
            {
                _items.Add(new CropProcure(itemId, sales, type, sales, price));
            }
        }
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        if (_items is null || _items.Count == 0)
            return ValueTask.CompletedTask;

        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        CastleManorManager manor = CastleManorManager.getInstance();
        if (!manor.isModifiablePeriod())
        {
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        // Check player privileges
        Clan? clan = player.getClan();
        Npc? lastFolk = player.getLastFolkNPC();
        if (clan == null || clan.getCastleId() != _manorId ||
            !player.hasClanPrivilege(ClanPrivilege.CS_MANOR_ADMIN) || lastFolk == null || !lastFolk.canInteract(player))
        {
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        // Filter crops with start amount lower than 0 and incorrect price
        List<CropProcure> list = new(_items.Count);
        foreach (CropProcure cp in _items)
        {
            Seed? s = manor.getSeedByCrop(cp.getId(), _manorId);
            if (s != null && cp.getStartAmount() <= s.getCropLimit() && cp.getPrice() >= s.getCropMinPrice() &&
                cp.getPrice() <= s.getCropMaxPrice())
            {
                list.Add(cp);
            }
        }

        // Save crop list
        manor.setNextCropProcure(list, _manorId);

        return ValueTask.CompletedTask;
    }
}
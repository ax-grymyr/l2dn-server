using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Commission;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Commission;

public struct RequestCommissionRegisterPacket: IIncomingPacket<GameSession>
{
    private int _itemObjectId;
    private long _pricePerUnit;
    private long _itemCount;
    private int _durationType; // -1 = None, 0 = 1 Day, 1 = 3 Days, 2 = 5 Days, 3 = 7 Days, 4 = 15 Days, 5 = 30 Days;
    private int _feeDiscountType; // 0 = none, 1 = 30% discount, 2 = 100% discount;

    public void ReadContent(PacketBitReader reader)
    {
        _itemObjectId = reader.ReadInt32();
        reader.ReadString(); // Item Name they use it for search we will use server side available names.
        _pricePerUnit = reader.ReadInt64();
        _itemCount = reader.ReadInt64();
        _durationType = reader.ReadInt32();
        _feeDiscountType = reader.ReadInt16();
        // packet.readShort(); // Unknown IDS;
        // packet.readInt(); // Unknown
        // packet.readInt(); // Unknown
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_feeDiscountType < 0 || _feeDiscountType > 2)
        {
            PacketLogger.Instance.Warn(player + " sent incorrect commission discount type: " + _feeDiscountType + ".");
            return ValueTask.CompletedTask;
        }
		
        if (_feeDiscountType == 1 && player.getInventory().getItemByItemId(22351) == null)
        {
            PacketLogger.Instance.Warn(player + ": Auction House Fee 30% Voucher not found in inventory.");
            return ValueTask.CompletedTask;
        }
        
        if (_feeDiscountType == 2 && player.getInventory().getItemByItemId(22352) == null)
        {
            PacketLogger.Instance.Warn(player + ": Auction House Fee 100% Voucher not found in inventory.");
            return ValueTask.CompletedTask;
        }
		
        if (_durationType < 0 || _durationType > 5)
        {
            PacketLogger.Instance.Warn(player + " sent incorrect commission duration type: " + _durationType + ".");
            return ValueTask.CompletedTask;
        }
		
        if (_durationType == 4 && player.getInventory().getItemByItemId(22353) == null)
        {
            PacketLogger.Instance.Warn(player + ": Auction House (15-day) Extension not found in inventory.");
            return ValueTask.CompletedTask;
        }
        
        if (_durationType == 5 && player.getInventory().getItemByItemId(22354) == null)
        {
            PacketLogger.Instance.Warn(player + ": Auction House (30-day) Extension not found in inventory.");
            return ValueTask.CompletedTask;
        }
		
        if (!ItemCommissionManager.isPlayerAllowedToInteract(player))
        {
            player.sendPacket(ExCloseCommissionPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        ItemCommissionManager.getInstance().registerItem(player, _itemObjectId, _itemCount, _pricePerUnit,
            _durationType, (byte)Math.Min(_feeDiscountType * 30 * _feeDiscountType, 100));
        
        return ValueTask.CompletedTask;
    }
}
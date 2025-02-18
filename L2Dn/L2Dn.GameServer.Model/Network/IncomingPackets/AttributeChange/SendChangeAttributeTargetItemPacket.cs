using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.AttributeChange;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.AttributeChange;

public struct SendChangeAttributeTargetItemPacket: IIncomingPacket<GameSession>
{
    private int _crystalItemId;
    private int _itemObjId;

    public void ReadContent(PacketBitReader reader)
    {
        _crystalItemId = reader.ReadInt32();
        _itemObjId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Item? item = player.getInventory().getItemByObjectId(_itemObjId);
        if (item == null || !item.isWeapon())
        {
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        player.sendPacket(new ExChangeAttributeInfoPacket(_crystalItemId, item));

        return ValueTask.CompletedTask;
    }
}
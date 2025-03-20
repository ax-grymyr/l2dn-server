using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Options;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestConfirmRefinerItemPacket: IIncomingPacket<GameSession>
{
    private int _targetItemObjId;
    private int _refinerItemObjId;

    public void ReadContent(PacketBitReader reader)
    {
        _targetItemObjId = reader.ReadInt32();
        _refinerItemObjId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Item? targetItem = player.getInventory().getItemByObjectId(_targetItemObjId);
        if (targetItem == null)
            return ValueTask.CompletedTask;

        Item? refinerItem = player.getInventory().getItemByObjectId(_refinerItemObjId);
        if (refinerItem == null)
            return ValueTask.CompletedTask;

        VariationFee? fee = VariationData.getInstance().getFee(targetItem.Id, refinerItem.Id);
        if (fee == null || !RefinePacketHelper.isValid(player, targetItem, refinerItem))
        {
            player.sendPacket(SystemMessageId.THIS_IS_NOT_A_SUITABLE_ITEM);
            return ValueTask.CompletedTask;
        }

        player.sendPacket(new ExPutIntensiveResultForVariationMakePacket(_refinerItemObjId, refinerItem.Id, 1));

        return ValueTask.CompletedTask;
    }
}
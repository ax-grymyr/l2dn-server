using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct MyTargetSelectedPacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _color;
	
    /**
     * @param player
     * @param target
     */
    public MyTargetSelectedPacket(Player player, Creature target)
    {
        _objectId = target is ControllableAirShip ? ((ControllableAirShip) target).getHelmObjectId() : target.ObjectId;
        _color = target.isAutoAttackable(player) ? player.getLevel() - target.getLevel() : 0;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.MY_TARGET_SELECTED);
        
        writer.WriteInt32(1); // Grand Crusade
        writer.WriteInt32(_objectId);
        writer.WriteInt16((short)_color);
        writer.WriteInt32(0);
    }
}
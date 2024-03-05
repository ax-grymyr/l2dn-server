using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.EquipmentUpgrade;

public readonly struct ExUpgradeSystemResultPacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _success;
	
    public ExUpgradeSystemResultPacket(int objectId, int success)
    {
        _objectId = objectId;
        _success = success;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_UPGRADE_SYSTEM_RESULT);
        
        writer.WriteInt16((short)_success);
        writer.WriteInt32(_objectId);
    }
}
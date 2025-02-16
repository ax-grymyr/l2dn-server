
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public struct StatusUpdatePacket: IOutgoingPacket
{
    private readonly Map<StatusUpdateType, int> _updates;
    private readonly int _objectId;
    private readonly bool _isPlayable;
    private int _casterObjectId = 0;
    private bool _isVisible = false;
	
    /**
     * Create {@link StatusUpdate} packet for given {@link WorldObject}.
     * @param object
     */
    public StatusUpdatePacket(WorldObject obj)
    {
        _updates = new();
        _objectId = obj.ObjectId;
        _isPlayable = obj.isPlayable();
    }
	
    public void addUpdate(StatusUpdateType type, int level)
    {
        _updates.put(type, level);
        if (_isPlayable)
        {
            switch (type)
            {
                case StatusUpdateType.CUR_HP:
                case StatusUpdateType.CUR_MP:
                case StatusUpdateType.CUR_CP:
                case StatusUpdateType.CUR_DP:
                case StatusUpdateType.CUR_BP:
                {
                    _isVisible = true;
                    break;
                }
            }
        }
    }
	
    public void addCaster(WorldObject obj)
    {
        _casterObjectId = obj.ObjectId;
    }
	
    public bool hasUpdates()
    {
        return _updates.Count != 0;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.STATUS_UPDATE);
        writer.WriteInt32(_objectId); // casterId
        writer.WriteInt32(_isVisible ? _casterObjectId : 0);
        writer.WriteByte(_isVisible);
        writer.WriteByte((byte)_updates.Count);
        foreach (var entry in _updates)
        {
            writer.WriteByte((byte)entry.Key);
            writer.WriteInt32(entry.Value);
        }
    }
}
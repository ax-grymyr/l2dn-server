using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExColosseumFenceInfoPacket: IOutgoingPacket
{
    private readonly int _objId;
    private readonly int _x;
    private readonly int _y;
    private readonly int _z;
    private readonly int _width;
    private readonly int _length;
    private readonly int _clientState;

    public ExColosseumFenceInfoPacket(Fence fence)
        : this(fence.ObjectId, fence.getX(), fence.getY(), fence.getZ(), fence.getWidth(), fence.getLength(),
            fence.getState())
    {
    }

    public ExColosseumFenceInfoPacket(int objId, double x, double y, double z, int width, int length, FenceState state)
    {
        _objId = objId;
        _x = (int)x;
        _y = (int)y;
        _z = (int)z;
        _width = width;
        _length = length;
        _clientState = (int)state;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_COLOSSEUM_FENCE_INFO);
        writer.WriteInt32(_objId);
        writer.WriteInt32(_clientState);
        writer.WriteInt32(_x);
        writer.WriteInt32(_y);
        writer.WriteInt32(_z);
        writer.WriteInt32(_width);
        writer.WriteInt32(_length);
    }
}
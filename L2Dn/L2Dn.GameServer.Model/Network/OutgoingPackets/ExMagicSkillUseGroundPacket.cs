using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExMagicSkillUseGroundPacket: IOutgoingPacket
{
    private readonly int _playerObjectId;
    private readonly int _skillId;
    private readonly Location3D _location;

    public ExMagicSkillUseGroundPacket(int playerObjectId, int skillId, Location3D location)
    {
        _playerObjectId = playerObjectId;
        _skillId = skillId;
        _location = location;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_MAGIC_SKILL_USE_GROUND);

        writer.WriteInt32(_playerObjectId);
        writer.WriteInt32(_skillId);
        writer.WriteLocation3D(_location);
    }
}
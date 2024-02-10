using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct MagicSkillUsePacket(int objectId, Location location): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0x48); // packet code

        writer.WriteInt32(0); // Casting bar type: 0 - default, 1 - default up, 2 - blue, 3 - green, 4 - red.
        writer.WriteInt32(objectId);
        writer.WriteInt32(objectId);
        writer.WriteInt32(60018); // teleport
        writer.WriteInt32(1);
        writer.WriteInt32(1000); // hit time in ms
        writer.WriteInt32(-1);
        writer.WriteInt32(0);
        writer.WriteInt32(location.X);
        writer.WriteInt32(location.Y);
        writer.WriteInt32(location.Z);
        writer.WriteInt16(0); // _isGroundTargetSkill ? 65535 : 0
        // if (_groundLocation == null)
        // {
        writer.WriteInt16(0);
        // }
        // else
        // {
        //     writeShort(1);
        //     writer.WriteInt32(_groundLocation.getX());
        //     writer.WriteInt32(_groundLocation.getY());
        //     writer.WriteInt32(_groundLocation.getZ());
        // }
        writer.WriteInt32(location.X); // target location
        writer.WriteInt32(location.Y);
        writer.WriteInt32(location.Z);
        writer.WriteInt32(0); // _actionId >= 0 // 1 when ID from RequestActionUse is used
        writer.WriteInt32(0); // _actionId >= 0 ? _actionId : 0 // ID from RequestActionUse. Used to set cooldown on summon skills.
        //if (_groundLocation == null)
        //{
            writer.WriteInt32(-1); // 306
        //}
    }
}

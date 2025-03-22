using System.Collections.Immutable;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Sayune;

public readonly struct ExFlyMovePacket(Player player, SayuneType type, int mapId, ImmutableArray<SayuneEntry> locations)
    : IOutgoingPacket
{
    private readonly int _objectId = player.ObjectId;

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_FLY_MOVE);

        writer.WriteInt32(_objectId);
        writer.WriteInt32((int)type);
        writer.WriteInt32(0); // ??
        writer.WriteInt32(mapId);
        if (locations.IsDefaultOrEmpty)
            writer.WriteInt32(0);
        else
        {
            writer.WriteInt32(locations.Length);
            foreach (SayuneEntry sayuneEntry in locations)
            {
                writer.WriteInt32(sayuneEntry.Id);
                writer.WriteInt32(0); // ??
                writer.WriteLocation3D(sayuneEntry.Location);
            }
        }
    }
}
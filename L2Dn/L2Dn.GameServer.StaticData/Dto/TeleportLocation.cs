using System.Collections.Immutable;
using L2Dn.GameServer.Constants;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.StaticData.Xml.Teleporters;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Dto;

public sealed class TeleportLocation
{
    public TeleportLocation(int id, XmlTeleportLocation location)
    {
        Id = id;
        Location = new Location3D(location.X, location.Y, location.Z);
        Name = location.NameSpecified ? location.Name : string.Empty;
        NpcStringId = location.NpcStringIdSpecified ? (NpcStringId)location.NpcStringId : null;
        QuestZoneId = location.QuestZoneIdSpecified ? location.QuestZoneId : 0;
        FeeId =  location.FeeItemIdSpecified ? location.FeeItemId : KnownItemId.Adena;
        FeeCount = location.FeeCountSpecified ? location.FeeCount : 0;
        CastleIds = ParseUtil.ParseList<int>(location.CastleId);
    }

    public Location3D Location { get; }
    public int Id { get; }
    public string Name { get; }
    public NpcStringId? NpcStringId { get; }
    public int QuestZoneId { get; }
    public int FeeId { get; }
    public long FeeCount { get; }
    public ImmutableArray<int> CastleIds { get; }
}
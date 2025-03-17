using System.Collections.Frozen;
using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.StaticData.Xml.Zones;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Zones;

internal static class ZoneFactory
{
    private static readonly FrozenDictionary<ZoneType, Func<int, ZoneForm, Zone>> _zoneFactories =
        new ZoneInfo[]
        {
            new(ZoneType.ArenaZone, (id, form) => new ArenaZone(id, form)),
            new(ZoneType.CastleZone, (id, form) => new CastleZone(id, form)),
            new(ZoneType.ClanHallZone, (id, form) => new ClanHallZone(id, form)),
            new(ZoneType.ConditionZone, (id, form) => new ConditionZone(id, form)),
            new(ZoneType.DamageZone, (id, form) => new DamageZone(id, form)),
            new(ZoneType.DerbyTrackZone, (id, form) => new DerbyTrackZone(id, form)),
            new(ZoneType.EffectZone, (id, form) => new EffectZone(id, form)),
            new(ZoneType.FishingZone, (id, form) => new FishingZone(id, form)),
            new(ZoneType.FortZone, (id, form) => new FortZone(id, form)),
            new(ZoneType.HqZone, (id, form) => new HqZone(id, form)),
            new(ZoneType.JailZone, (id, form) => new JailZone(id, form)),
            new(ZoneType.LandingZone, (id, form) => new LandingZone(id, form)),
            new(ZoneType.MotherTreeZone, (id, form) => new MotherTreeZone(id, form)),
            new(ZoneType.NoLandingZone, (id, form) => new NoLandingZone(id, form)),
            new(ZoneType.NoPvPZone, (id, form) => new NoPvPZone(id, form)),
            new(ZoneType.NoRestartZone, (id, form) => new NoRestartZone(id, form)),
            new(ZoneType.NoStoreZone, (id, form) => new NoStoreZone(id, form)),
            new(ZoneType.NoSummonFriendZone, (id, form) => new NoSummonFriendZone(id, form)),
            new(ZoneType.OlympiadStadiumZone, (id, form) => new OlympiadStadiumZone(id, form)),
            new(ZoneType.PeaceZone, (id, form) => new PeaceZone(id, form)),
            new(ZoneType.ResidenceHallTeleportZone, (id, form) => new ResidenceHallTeleportZone(id, form)),
            new(ZoneType.ResidenceTeleportZone, (id, form) => new ResidenceTeleportZone(id, form)),
            new(ZoneType.RespawnZone, (id, form) => new RespawnZone(id, form)),
            new(ZoneType.SayuneZone, (id, form) => new SayuneZone(id, form)),
            new(ZoneType.ScriptZone, (id, form) => new ScriptZone(id, form)),
            new(ZoneType.SiegableHallZone, (id, form) => new SiegableHallZone(id, form)),
            new(ZoneType.SiegeZone, (id, form) => new SiegeZone(id, form)),
            new(ZoneType.SwampZone, (id, form) => new SwampZone(id, form)),
            new(ZoneType.TaxZone, (id, form) => new TaxZone(id, form)),
            new(ZoneType.TeleportZone, (id, form) => new TeleportZone(id, form)),
            new(ZoneType.TimedHuntingZone, (id, form) => new TimedHuntingZone(id, form)),
            new(ZoneType.UndyingZone, (id, form) => new UndyingZone(id, form)),
            new(ZoneType.WaterZone, (id, form) => new WaterZone(id, form)),
        }.ToFrozenDictionary(info => info.ZoneType, info => info.Factory);

    public static Zone Create(XmlZone xmlZone)
    {
        // Create the zone
        if (!_zoneFactories.TryGetValue(xmlZone.Type, out Func<int, ZoneForm, Zone>? factory))
            throw new ArgumentException($"Unknown zone type '{xmlZone.Type}'");

        ZoneForm zoneForm = ZoneForm.Create(xmlZone);
        Zone zone = factory(xmlZone.Id, zoneForm);

        // Check for additional parameters
        xmlZone.Stats.ForEach(stat => zone.setParameter(stat.Name, stat.Value));

        if (zone is ZoneRespawn zoneRespawn)
        {
            xmlZone.Spawns.ForEach(spawn =>
                zoneRespawn.parseLoc(new Location3D(spawn.X, spawn.Y, spawn.Z), spawn.Type));
        }

        if (zone is RespawnZone respawnZone)
            xmlZone.Races.ForEach(el => respawnZone.addRaceRespawnPoint(el.Race, el.Point));

        if (!string.IsNullOrEmpty(xmlZone.Name))
            zone.setName(xmlZone.Name);

        return zone;
    }

    private readonly record struct ZoneInfo(ZoneType ZoneType, Func<int, ZoneForm, Zone> Factory);
}
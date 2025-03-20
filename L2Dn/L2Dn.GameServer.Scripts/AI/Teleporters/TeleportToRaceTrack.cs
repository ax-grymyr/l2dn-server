using System.Collections.Immutable;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Scripts.AI.Teleporters;

public sealed class TeleportToRaceTrack: AbstractScript
{
	// NPC
	private const int RaceManager = 30995;

	// Locations
	private static readonly Location3D _raceTrackTeleport = new(12661, 181687, -3540);

	private static readonly ImmutableDictionary<int, Location3D> _teleporterLocations = new[]
	{
		(30320, new Location3D(-80826, 149775, -3043)), // Richlin
		(30256, new Location3D(-12672, 122776, -3116)), // Bella
		(30059, new Location3D(15670, 142983, -2705)), // Trisha
		(30080, new Location3D(83400, 147943, -3404)), // Clarissa
		(30899, new Location3D(111409, 219364, -3545)), // Flauen
		(30177, new Location3D(82956, 53162, -1495)), // Valentina
		(30848, new Location3D(146331, 25762, -2018)), // Elisa
		(30233, new Location3D(116819, 76994, -2714)), // Esmeralda
		(31275, new Location3D(147930, -55281, -2728)), // Tatiana
		(31210, new Location3D(12882, 181053, -3560)), // Race Track Gatekeeper
	}.ToImmutableDictionary(x => x.Item1, x => x.Item2);

	// Other
	private const string MonsterReturn = "MONSTER_RETURN";

	public TeleportToRaceTrack()
	{
		addTalkId(RaceManager);
		addTalkId(_teleporterLocations.Keys.ToArray());
	}

	public override string? onTalk(Npc npc, Player player)
	{
		if (npc.Id == RaceManager)
		{
			int returnId = player.getVariables().Get(MonsterReturn, -1);
			if (!_teleporterLocations.TryGetValue(returnId, out Location3D location))
				location = _teleporterLocations[30059];

			player.teleToLocation(new Location(location, 0));
			player.getVariables().Remove(MonsterReturn);
		}
		else
		{
			player.teleToLocation(new Location(_raceTrackTeleport, 0));
			player.getVariables().Set(MonsterReturn, npc.Id);
		}

		return base.onTalk(npc, player);
	}
}
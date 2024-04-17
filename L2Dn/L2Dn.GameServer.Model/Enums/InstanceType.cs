using System.Collections.Frozen;

namespace L2Dn.GameServer.Enums;


public enum InstanceType
{
	WorldObject,
	Item,
	Creature,
	Npc,
	Playable,
	Summon,
	Player,
	Folk,
	Merchant,
	Warehouse,
	StaticObject,
	Door,
	TerrainObject,
	EffectPoint,
	CommissionManager,

	// Summons, Pets, Decoys and Traps
	Servitor,
	Pet,
	Cubic,
	Decoy,
	Trap,

	// Attackable
	Attackable,
	Guard,
	Monster,
	Chest,
	ControllableMob,
	FeedableBeast,
	TamedBeast,
	FriendlyMob,
	RaidBoss,
	GrandBoss,
	FriendlyNpc,

	// FlyMobs
	FlyTerrainObject,

	// Vehicles
	Vehicle,
	Boat,
	AirShip,
	Shuttle,
	ControllableAirShip,

	// Siege
	Defender,
	Artefact,
	ControlTower,
	FlameTower,
	SiegeFlag,

	// Fort Siege
	FortCommander,

	// Fort NPCs
	FortLogistics,
	FortManager,

	// City NPCs
	BroadcastingTower,
	Fisherman,
	OlympiadManager,
	PetManager,
	Teleporter,
	VillageMaster,

	// Doormens
	Doorman,
	FortDoorman,

	// Custom
	ClassMaster,
	SchemeBuffer,
	EventMob
}

public static class InstanceTypeUtil
{
	// TODO: this must be simple array
	private static readonly FrozenDictionary<InstanceType, InstanceType?> _parentInstanceTypes =
		new (InstanceType, InstanceType?)[]
		{
			(InstanceType.WorldObject, null),
			(InstanceType.Item, InstanceType.WorldObject),
			(InstanceType.Creature, InstanceType.WorldObject),
			(InstanceType.Npc, InstanceType.Creature),
			(InstanceType.Playable, InstanceType.Creature),
			(InstanceType.Summon, InstanceType.Playable),
			(InstanceType.Player, InstanceType.Playable),
			(InstanceType.Folk, InstanceType.Npc),
			(InstanceType.Merchant, InstanceType.Folk),
			(InstanceType.Warehouse, InstanceType.Folk),
			(InstanceType.StaticObject, InstanceType.Creature),
			(InstanceType.Door, InstanceType.Creature),
			(InstanceType.TerrainObject, InstanceType.Npc),
			(InstanceType.EffectPoint, InstanceType.Npc),
			(InstanceType.CommissionManager, InstanceType.Npc),
			// Summons, Pets, Decoys and Traps
			(InstanceType.Servitor, InstanceType.Summon),
			(InstanceType.Pet, InstanceType.Summon),
			(InstanceType.Cubic, InstanceType.Creature),
			(InstanceType.Decoy, InstanceType.Creature),
			(InstanceType.Trap, InstanceType.Npc),
			// Attackable
			(InstanceType.Attackable, InstanceType.Npc),
			(InstanceType.Guard, InstanceType.Attackable),
			(InstanceType.Monster, InstanceType.Attackable),
			(InstanceType.Chest, InstanceType.Monster),
			(InstanceType.ControllableMob, InstanceType.Monster),
			(InstanceType.FeedableBeast, InstanceType.Monster),
			(InstanceType.TamedBeast, InstanceType.FeedableBeast),
			(InstanceType.FriendlyMob, InstanceType.Attackable),
			(InstanceType.RaidBoss, InstanceType.Monster),
			(InstanceType.GrandBoss, InstanceType.RaidBoss),
			(InstanceType.FriendlyNpc, InstanceType.Attackable),
			// FlyMobs
			(InstanceType.FlyTerrainObject, InstanceType.Npc),
			// Vehicles
			(InstanceType.Vehicle, InstanceType.Creature),
			(InstanceType.Boat, InstanceType.Vehicle),
			(InstanceType.AirShip, InstanceType.Vehicle),
			(InstanceType.Shuttle, InstanceType.Vehicle),
			(InstanceType.ControllableAirShip, InstanceType.AirShip),
			// Siege
			(InstanceType.Defender, InstanceType.Attackable),
			(InstanceType.Artefact, InstanceType.Folk),
			(InstanceType.ControlTower, InstanceType.Npc),
			(InstanceType.FlameTower, InstanceType.Npc),
			(InstanceType.SiegeFlag, InstanceType.Npc),
			// Fort Siege
			(InstanceType.FortCommander, InstanceType.Defender),
			// Fort NPCs
			(InstanceType.FortLogistics, InstanceType.Merchant),
			(InstanceType.FortManager, InstanceType.Merchant),
			// City NPCs
			(InstanceType.BroadcastingTower, InstanceType.Npc),
			(InstanceType.Fisherman, InstanceType.Merchant),
			(InstanceType.OlympiadManager, InstanceType.Npc),
			(InstanceType.PetManager, InstanceType.Merchant),
			(InstanceType.Teleporter, InstanceType.Npc),
			(InstanceType.VillageMaster, InstanceType.Folk),
			// Doormens
			(InstanceType.Doorman, InstanceType.Folk),
			(InstanceType.FortDoorman, InstanceType.Doorman),
			// Custom
			(InstanceType.ClassMaster, InstanceType.Folk),
			(InstanceType.SchemeBuffer, InstanceType.Npc),
			(InstanceType.EventMob, InstanceType.Npc),
		}.ToFrozenDictionary(t => t.Item1, t => t.Item2);

	public static InstanceType? GetParent(this InstanceType instanceType)
	{
		return _parentInstanceTypes.GetValueOrDefault(instanceType);
	}

	public static bool IsType(this InstanceType instanceType, InstanceType other)
	{
		// TODO make mask and use it
		if (instanceType == other)
			return true;

		InstanceType? parent = instanceType.GetParent();
		while (parent != null)
		{
			if (parent.Value == other)
				return true;
			
			parent = parent.Value.GetParent();
		}

		return false;
	}
}
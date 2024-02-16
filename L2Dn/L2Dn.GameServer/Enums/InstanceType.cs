using System.Collections.Immutable;

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
	private static ImmutableSortedDictionary<InstanceType, InstanceType?> _parentInstanceTypes =
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
		}.ToImmutableSortedDictionary(t => t.Item1, t => t.Item2);

	public static InstanceType? GetParent(this InstanceType instanceType)
	{
		return CollectionExtensions.GetValueOrDefault(_parentInstanceTypes, instanceType);
	}
}

//
// public enum InstanceType
// {
// 	WorldObject(null),
// 	Item(WorldObject),
// 	Creature(WorldObject),
// 	Npc(Creature),
// 	Playable(Creature),
// 	Summon(Playable),
// 	Player(Playable),
// 	Folk(Npc),
// 	Merchant(Folk),
// 	Warehouse(Folk),
// 	StaticObject(Creature),
// 	Door(Creature),
// 	TerrainObject(Npc),
// 	EffectPoint(Npc),
// 	CommissionManager(Npc),
// 	// Summons, Pets, Decoys and Traps
// 	Servitor(Summon),
// 	Pet(Summon),
// 	Cubic(Creature),
// 	Decoy(Creature),
// 	Trap(Npc),
// 	// Attackable
// 	Attackable(Npc),
// 	Guard(Attackable),
// 	Monster(Attackable),
// 	Chest(Monster),
// 	ControllableMob(Monster),
// 	FeedableBeast(Monster),
// 	TamedBeast(FeedableBeast),
// 	FriendlyMob(Attackable),
// 	RaidBoss(Monster),
// 	GrandBoss(RaidBoss),
// 	FriendlyNpc(Attackable),
// 	// FlyMobs
// 	FlyTerrainObject(Npc),
// 	// Vehicles
// 	Vehicle(Creature),
// 	Boat(Vehicle),
// 	AirShip(Vehicle),
// 	Shuttle(Vehicle),
// 	ControllableAirShip(AirShip),
// 	// Siege
// 	Defender(Attackable),
// 	Artefact(Folk),
// 	ControlTower(Npc),
// 	FlameTower(Npc),
// 	SiegeFlag(Npc),
// 	// Fort Siege
// 	FortCommander(Defender),
// 	// Fort NPCs
// 	FortLogistics(Merchant),
// 	FortManager(Merchant),
// 	// City NPCs
// 	BroadcastingTower(Npc),
// 	Fisherman(Merchant),
// 	OlympiadManager(Npc),
// 	PetManager(Merchant),
// 	Teleporter(Npc),
// 	VillageMaster(Folk),
// 	// Doormens
// 	Doorman(Folk),
// 	FortDoorman(Doorman),
// 	// Custom
// 	ClassMaster(Folk),
// 	SchemeBuffer(Npc),
// 	EventMob(Npc);
// 	
// 	private final InstanceType _parent;
// 	private final long _typeL;
// 	private final long _typeH;
// 	private final long _maskL;
// 	private final long _maskH;
// 	
// 	private InstanceType(InstanceType parent)
// 	{
// 		_parent = parent;
// 		
// 		final int high = ordinal() - (Long.SIZE - 1);
// 		if (high < 0)
// 		{
// 			_typeL = 1L << ordinal();
// 			_typeH = 0;
// 		}
// 		else
// 		{
// 			_typeL = 0;
// 			_typeH = 1L << high;
// 		}
// 		
// 		if ((_typeL < 0) || (_typeH < 0))
// 		{
// 			throw new Error("Too many instance types, failed to load " + name());
// 		}
// 		
// 		if (parent != null)
// 		{
// 			_maskL = _typeL | parent._maskL;
// 			_maskH = _typeH | parent._maskH;
// 		}
// 		else
// 		{
// 			_maskL = _typeL;
// 			_maskH = _typeH;
// 		}
// 	}
// 	
// 	public InstanceType getParent()
// 	{
// 		return _parent;
// 	}
// 	
// 	public boolean isType(InstanceType it)
// 	{
// 		return ((_maskL & it._typeL) > 0) || ((_maskH & it._typeH) > 0);
// 	}
// 	
// 	public boolean isTypes(InstanceType... it)
// 	{
// 		for (InstanceType i : it)
// 		{
// 			if (isType(i))
// 			{
// 				return true;
// 			}
// 		}
// 		return false;
// 	}
// }

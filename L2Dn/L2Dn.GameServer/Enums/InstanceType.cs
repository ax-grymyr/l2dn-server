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

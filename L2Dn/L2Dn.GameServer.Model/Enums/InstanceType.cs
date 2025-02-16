using L2Dn.Utilities;

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
	private readonly struct InstanceTypeInfo(ulong mask, InstanceType? parent)
	{
		public readonly ulong Mask = mask;
		public readonly InstanceType? Parent = parent;
	}

	private static readonly InstanceTypeInfo[] _instanceTypes = CreateInfo();

	public static InstanceType? GetParent(this InstanceType instanceType) =>
		instanceType >= 0 && (int)instanceType < _instanceTypes.Length
			? _instanceTypes[(int)instanceType].Parent
			: null;

	/// <summary>
	/// Verifies if the instance is of given instance type.
	/// </summary>
	/// <param name="instanceType"></param>
	/// <param name="other"></param>
	/// <returns></returns>
	public static bool IsType(this InstanceType instanceType, InstanceType other)
	{
		if (instanceType >= 0 && (int)instanceType < _instanceTypes.Length)
			return (_instanceTypes[(int)instanceType].Mask & (1UL << (int)other)) != 0;
		
		return instanceType == other;
	}

	private static InstanceTypeInfo[] CreateInfo()
	{
		int count = EnumUtil.GetValues<InstanceType>().Length;
		InstanceType?[] parents = new InstanceType?[count];

		parents[(int)InstanceType.WorldObject] = null;
		parents[(int)InstanceType.Item] = InstanceType.WorldObject;
		parents[(int)InstanceType.Creature] = InstanceType.WorldObject;
		parents[(int)InstanceType.Npc] = InstanceType.Creature;
		parents[(int)InstanceType.Playable] = InstanceType.Creature;
		parents[(int)InstanceType.Summon] = InstanceType.Playable;
		parents[(int)InstanceType.Player] = InstanceType.Playable;
		parents[(int)InstanceType.Folk] = InstanceType.Npc;
		parents[(int)InstanceType.Merchant] = InstanceType.Folk;
		parents[(int)InstanceType.Warehouse] = InstanceType.Folk;
		parents[(int)InstanceType.StaticObject] = InstanceType.Creature;
		parents[(int)InstanceType.Door] = InstanceType.Creature;
		parents[(int)InstanceType.TerrainObject] = InstanceType.Npc;
		parents[(int)InstanceType.EffectPoint] = InstanceType.Npc;
		parents[(int)InstanceType.CommissionManager] = InstanceType.Npc;
		// Summons, Pets, Decoys and Traps
		parents[(int)InstanceType.Servitor] = InstanceType.Summon;
		parents[(int)InstanceType.Pet] = InstanceType.Summon;
		parents[(int)InstanceType.Cubic] = InstanceType.Creature;
		parents[(int)InstanceType.Decoy] = InstanceType.Creature;
		parents[(int)InstanceType.Trap] = InstanceType.Npc;
		// Attackable
		parents[(int)InstanceType.Attackable] = InstanceType.Npc;
		parents[(int)InstanceType.Guard] = InstanceType.Attackable;
		parents[(int)InstanceType.Monster] = InstanceType.Attackable;
		parents[(int)InstanceType.Chest] = InstanceType.Monster;
		parents[(int)InstanceType.ControllableMob] = InstanceType.Monster;
		parents[(int)InstanceType.FeedableBeast] = InstanceType.Monster;
		parents[(int)InstanceType.TamedBeast] = InstanceType.FeedableBeast;
		parents[(int)InstanceType.FriendlyMob] = InstanceType.Attackable;
		parents[(int)InstanceType.RaidBoss] = InstanceType.Monster;
		parents[(int)InstanceType.GrandBoss] = InstanceType.RaidBoss;
		parents[(int)InstanceType.FriendlyNpc] = InstanceType.Attackable;
		// FlyMobs
		parents[(int)InstanceType.FlyTerrainObject] = InstanceType.Npc;
		// Vehicles
		parents[(int)InstanceType.Vehicle] = InstanceType.Creature;
		parents[(int)InstanceType.Boat] = InstanceType.Vehicle;
		parents[(int)InstanceType.AirShip] = InstanceType.Vehicle;
		parents[(int)InstanceType.Shuttle] = InstanceType.Vehicle;
		parents[(int)InstanceType.ControllableAirShip] = InstanceType.AirShip;
		// Siege
		parents[(int)InstanceType.Defender] = InstanceType.Attackable;
		parents[(int)InstanceType.Artefact] = InstanceType.Folk;
		parents[(int)InstanceType.ControlTower] = InstanceType.Npc;
		parents[(int)InstanceType.FlameTower] = InstanceType.Npc;
		parents[(int)InstanceType.SiegeFlag] = InstanceType.Npc;
		// Fort Siege
		parents[(int)InstanceType.FortCommander] = InstanceType.Defender;
		// Fort NPCs
		parents[(int)InstanceType.FortLogistics] = InstanceType.Merchant;
		parents[(int)InstanceType.FortManager] = InstanceType.Merchant;
		// City NPCs
		parents[(int)InstanceType.BroadcastingTower] = InstanceType.Npc;
		parents[(int)InstanceType.Fisherman] = InstanceType.Merchant;
		parents[(int)InstanceType.OlympiadManager] = InstanceType.Npc;
		parents[(int)InstanceType.PetManager] = InstanceType.Merchant;
		parents[(int)InstanceType.Teleporter] = InstanceType.Npc;
		parents[(int)InstanceType.VillageMaster] = InstanceType.Folk;
		// Doormens
		parents[(int)InstanceType.Doorman] = InstanceType.Folk;
		parents[(int)InstanceType.FortDoorman] = InstanceType.Doorman;
		// Custom
		parents[(int)InstanceType.ClassMaster] = InstanceType.Folk;
		parents[(int)InstanceType.SchemeBuffer] = InstanceType.Npc;
		parents[(int)InstanceType.EventMob] = InstanceType.Npc;

		InstanceTypeInfo[] instanceTypes = new InstanceTypeInfo[count];
		
		// Calculate masks
		for (int index = 0; index < parents.Length; ++index)
		{
			InstanceType type = (InstanceType)index;
			InstanceType? parent = parents[index];
			ulong mask = 1UL << index;
			if (parent is not null)
			{
				if (parent.Value > type)
					throw new InvalidOperationException($"Invalid instance type hierarchy: {parent.Value} > {type}");
				
				mask |= instanceTypes[(int)parent.Value].Mask;
			}

			instanceTypes[index] = new InstanceTypeInfo(mask, parent);
		}

		return instanceTypes;
	}
}
using FluentAssertions;
using L2Dn.GameServer.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Tests;

public class InstanceTypeTests
{
    [Fact]
    public void VerifySomeValues()
    {
        InstanceType.WorldObject.GetParent().Should().Be(null);
        InstanceType.WorldObject.IsType(InstanceType.WorldObject).Should().BeTrue();
        InstanceType.WorldObject.IsType(InstanceType.Creature).Should().BeFalse();
        InstanceType.WorldObject.IsType(InstanceType.Npc).Should().BeFalse();

        InstanceType.Item.GetParent().Should().Be(InstanceType.WorldObject);
        InstanceType.Item.IsType(InstanceType.WorldObject).Should().BeTrue();
        InstanceType.Item.IsType(InstanceType.Item).Should().BeTrue();
        
        InstanceType.FortManager.GetParent().Should().Be(InstanceType.Merchant);
        InstanceType.FortManager.IsType(InstanceType.WorldObject).Should().BeTrue();
        InstanceType.FortManager.IsType(InstanceType.Creature).Should().BeTrue();
        InstanceType.FortManager.IsType(InstanceType.Npc).Should().BeTrue();
        InstanceType.FortManager.IsType(InstanceType.Folk).Should().BeTrue();
        InstanceType.FortManager.IsType(InstanceType.Merchant).Should().BeTrue();
        InstanceType.FortManager.IsType(InstanceType.FortManager).Should().BeTrue();
        InstanceType.FortManager.IsType(InstanceType.Door).Should().BeFalse();
        InstanceType.FortManager.IsType(InstanceType.Warehouse).Should().BeFalse();
   }

    [Fact]
    public void VerifyHierarchy()
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

	    for (int index = 0; index < count; index++)
	    {
			InstanceType type = (InstanceType)index;
			InstanceType? parent = parents[index];

			type.GetParent().Should().Be(parent);
			type.IsType(type).Should().BeTrue();
			while (parent is not null)
			{
				type.IsType(parent.Value).Should().BeTrue();
				parent = parent.Value.GetParent();
			}

			foreach (InstanceType other in EnumUtil.GetValues<InstanceType>())
			{
				bool typeIsSelfOrParentOfOther = false;
				for (InstanceType? p = type; p != null; p = p.Value.GetParent())
				{
					typeIsSelfOrParentOfOther |= other == p.Value;
				}

				type.IsType(other).Should().Be(typeIsSelfOrParentOfOther, "{0} is self or parent of {1}", other, type);
			}
	    }
    }
}
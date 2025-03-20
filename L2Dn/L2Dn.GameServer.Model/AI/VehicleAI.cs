using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.AI;

public abstract class VehicleAI: CreatureAI
{
	/**
	 * Simple AI for vehicles
	 * @param vehicle
	 */
	protected VehicleAI(Vehicle vehicle): base(vehicle)
	{
	}

	protected override void onIntentionAttack(Creature target)
	{
	}

	protected override void onIntentionCast(Skill skill, WorldObject? target, Item? item, bool forceUse, bool dontMove)
	{
	}

	protected override void onIntentionFollow(Creature target)
	{
	}

	protected override void onIntentionPickUp(WorldObject item)
	{
	}

	protected override void onIntentionInteract(WorldObject obj)
	{
	}

	protected override void onEvtAttacked(Creature attacker)
	{
	}

	protected override void onEvtAggression(Creature target, int aggro)
	{
	}

	protected override void onEvtActionBlocked(Creature attacker)
	{
	}

	protected override void onEvtRooted(Creature attacker)
	{
	}

	protected override void onEvtForgetObject(WorldObject @object)
	{
	}

	protected override void onEvtCancel()
	{
	}

	protected override void onEvtDead()
	{
	}

	protected override void onEvtFakeDeath()
	{
	}

	protected override void onEvtFinishCasting()
	{
	}

	protected override void clientActionFailed()
	{
	}

	public override void moveToPawn(WorldObject pawn, int offset)
	{
	}

	protected override void clientStoppedMoving()
	{
	}
}
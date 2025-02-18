using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.AI;

public class DoorAI : CreatureAI
{
	public DoorAI(Door door): base(door)
	{
	}

	protected override void onIntentionIdle()
	{
	}

	protected override void onIntentionActive()
	{
	}

	protected override void onIntentionRest()
	{
	}

	protected override void onIntentionAttack(Creature target)
	{
	}

	protected override void onIntentionCast(Skill skill, WorldObject? target, Item? item, bool forceUse, bool dontMove)
	{
	}

	protected override void onIntentionMoveTo(Location3D destination)
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

	public override void onEvtThink()
	{
	}

	protected override void onEvtAttacked(Creature attacker)
	{
		ThreadPool.execute(new onEventAttackedDoorTask(this, (Door)_actor, attacker));
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

	protected override void onEvtReadyToAct()
	{
	}

	protected override void onEvtArrived()
	{
	}

	protected override void onEvtArrivedRevalidate()
	{
	}

	protected override void onEvtArrivedBlocked(Location location)
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

	private class onEventAttackedDoorTask : Runnable
	{
		private readonly DoorAI _self;
		private readonly Door _door;
		private readonly Creature _attacker;

		public onEventAttackedDoorTask(DoorAI self, Door door, Creature attacker)
		{
			_self = self;
			_door = door;
			_attacker = attacker;
		}

		public void run()
		{
			World.getInstance().forEachVisibleObject<Defender>(_door, guard =>
			{
				if (_self._actor.IsInsideRadius3D(guard, guard.getTemplate().getClanHelpRange()))
				{
					guard.getAI().notifyEvent(CtrlEvent.EVT_AGGRESSION, _attacker, 15);
				}
			});
		}
	}
}
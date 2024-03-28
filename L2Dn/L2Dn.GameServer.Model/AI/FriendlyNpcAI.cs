using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.AI;

public class FriendlyNpcAI : AttackableAI
{
	public FriendlyNpcAI(Attackable attackable): base(attackable)
	{
	}
	
	protected override void onEvtAttacked(Creature attacker)
	{
	}
	
	protected override void onEvtAggression(Creature target, int aggro)
	{
	}
	
	protected override void onIntentionAttack(Creature target)
	{
		if (target == null)
		{
			clientActionFailed();
			return;
		}
		
		if (getIntention() == CtrlIntention.AI_INTENTION_REST)
		{
			clientActionFailed();
			return;
		}
		
		if (_actor.isAllSkillsDisabled() || _actor.isCastingNow() || _actor.isControlBlocked())
		{
			clientActionFailed();
			return;
		}
		
		// Set the Intention of this AbstractAI to AI_INTENTION_ATTACK
		changeIntention(CtrlIntention.AI_INTENTION_ATTACK, target);
		
		// Set the AI attack target
		setTarget(target);
		
		stopFollow();
		
		// Launch the Think Event
		notifyEvent(CtrlEvent.EVT_THINK, null);
	}
	
	protected override void thinkAttack()
	{
		Attackable npc = getActiveChar();
		if (npc.isCastingNow() || npc.isCoreAIDisabled())
		{
			return;
		}
		
		WorldObject target = getTarget();
		Creature originalAttackTarget = (target != null) && target.isCreature() ? (Creature) target : null;
		// Check if target is dead or if timeout is expired to stop this attack
		if ((originalAttackTarget == null) || originalAttackTarget.isAlikeDead())
		{
			// Stop hating this target after the attack timeout or if target is dead
			if (originalAttackTarget != null)
			{
				npc.stopHating(originalAttackTarget);
			}
			
			// Set the AI Intention to AI_INTENTION_ACTIVE
			setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
			
			npc.setWalking();
			return;
		}
		
		int collision = npc.getTemplate().getCollisionRadius();
		setTarget(originalAttackTarget);
		
		int combinedCollision = collision + originalAttackTarget.getTemplate().getCollisionRadius();
		if (!npc.isMovementDisabled() && (Rnd.get(100) <= 3))
		{
			foreach (Attackable nearby in World.getInstance().getVisibleObjects<Attackable>(npc))
			{
				if (npc.isInsideRadius2D(nearby, collision) && (nearby != originalAttackTarget))
				{
					int newX = combinedCollision + Rnd.get(40);
					if (Rnd.nextBoolean())
					{
						newX += originalAttackTarget.getX();
					}
					else
					{
						newX = originalAttackTarget.getX() - newX;
					}
					int newY = combinedCollision + Rnd.get(40);
					if (Rnd.nextBoolean())
					{
						newY += originalAttackTarget.getY();
					}
					else
					{
						newY = originalAttackTarget.getY() - newY;
					}
					
					if (!npc.isInsideRadius2D(newX, newY, 0, collision))
					{
						int newZ = npc.getZ() + 30;
						if (GeoEngine.getInstance().canMoveToTarget(npc.getX(), npc.getY(), npc.getZ(), newX, newY, newZ, npc.getInstanceWorld()))
						{
							moveTo(newX, newY, newZ);
						}
					}
					return;
				}
			}
		}
		
		// Calculate Archer movement.
		if ((!npc.isMovementDisabled()) && (npc.getAiType() == AIType.ARCHER) && (Rnd.get(100) < 15))
		{
			double distance2 = npc.calculateDistanceSq2D(originalAttackTarget);
			if (Math.Sqrt(distance2) <= (60 + combinedCollision))
			{
				int posX = npc.getX();
				int posY = npc.getY();
				int posZ = npc.getZ() + 30;
				if (originalAttackTarget.getX() < posX)
				{
					posX += 300;
				}
				else
				{
					posX -= 300;
				}
				
				if (originalAttackTarget.getY() < posY)
				{
					posY += 300;
				}
				else
				{
					posY -= 300;
				}
				
				if (GeoEngine.getInstance().canMoveToTarget(npc.getX(), npc.getY(), npc.getZ(), posX, posY, posZ, npc.getInstanceWorld()))
				{
					setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, new Location(posX, posY, posZ, 0));
				}
				return;
			}
		}
		
		double dist = npc.calculateDistance2D(originalAttackTarget);
		int dist2 = (int) dist - collision;
		int range = npc.getPhysicalAttackRange() + combinedCollision;
		if (originalAttackTarget.isMoving())
		{
			range += 50;
			if (npc.isMoving())
			{
				range += 50;
			}
		}
		
		if ((dist2 > range) || !GeoEngine.getInstance().canSeeTarget(npc, originalAttackTarget))
		{
			if (originalAttackTarget.isMoving())
			{
				range -= 100;
			}
			if (range < 5)
			{
				range = 5;
			}
			moveToPawn(originalAttackTarget, range);
			return;
		}
		
		_actor.doAutoAttack(originalAttackTarget);
	}
	
	protected override void thinkCast()
	{
		WorldObject target = getCastTarget();
		if (checkTargetLost(target))
		{
			setCastTarget(null);
			setTarget(null);
			return;
		}
		if (maybeMoveToPawn(target, _actor.getMagicalAttackRange(_skill)))
		{
			return;
		}
		_actor.doCast(_skill, _item, _forceUse, _dontMove);
	}
}
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.AI;

/**
 * AI for controllable mobs
 * @author littlecrow
 */
public class ControllableMobAI : AttackableAI
{
	public const int AI_IDLE = 1;
	public const int AI_NORMAL = 2;
	public const int AI_FORCEATTACK = 3;
	public const int AI_FOLLOW = 4;
	public const int AI_CAST = 5;
	public const int AI_ATTACK_GROUP = 6;
	
	private int _alternateAI;
	
	private bool _isThinking; // to prevent thinking recursively
	private bool _isNotMoving;
	
	private Creature _forcedTarget;
	private MobGroup _targetGroup;
	
	protected void thinkFollow()
	{
		Attackable me = (Attackable) _actor;
		if (!Util.checkIfInRange(MobGroupTable.FOLLOW_RANGE, me, getForcedTarget(), true))
		{
			int signX = Rnd.nextBoolean() ? -1 : 1;
			int signY = Rnd.nextBoolean() ? -1 : 1;
			int randX = Rnd.get(MobGroupTable.FOLLOW_RANGE);
			int randY = Rnd.get(MobGroupTable.FOLLOW_RANGE);
			moveTo(new Location3D(getForcedTarget().getX() + (signX * randX),
				getForcedTarget().getY() + (signY * randY), getForcedTarget().getZ()));
		}
	}

	public override void onEvtThink()
	{
		if (_isThinking)
		{
			return;
		}
		
		setThinking(true);
		
		try
		{
			switch (_alternateAI)
			{
				case AI_IDLE:
				{
					if (getIntention() != CtrlIntention.AI_INTENTION_ACTIVE)
					{
						setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
					}
					break;
				}
				case AI_FOLLOW:
				{
					thinkFollow();
					break;
				}
				case AI_CAST:
				{
					thinkCast();
					break;
				}
				case AI_FORCEATTACK:
				{
					thinkForceAttack();
					break;
				}
				case AI_ATTACK_GROUP:
				{
					thinkAttackGroup();
					break;
				}
				default:
				{
					if (getIntention() == CtrlIntention.AI_INTENTION_ACTIVE)
					{
						thinkActive();
					}
					else if (getIntention() == CtrlIntention.AI_INTENTION_ATTACK)
					{
						thinkAttack();
					}
					break;
				}
			}
		}
		finally
		{
			setThinking(false);
		}
	}
	
	protected override void thinkCast()
	{
		WorldObject target = _skill.getTarget(_actor, _forceUse, _dontMove, false);
		if ((target == null) || !target.isCreature() || ((Creature) target).isAlikeDead())
		{
			target = _skill.getTarget(_actor, findNextRndTarget(), _forceUse, _dontMove, false);
		}
		
		if (target == null)
		{
			return;
		}
		
		setTarget(target);
		
		if (!_actor.isMuted())
		{
			int maxRange = 0;
			// check distant skills
			foreach (Skill sk in _actor.getAllSkills())
			{
				if (Util.checkIfInRange(sk.getCastRange(), _actor, target, true) && !_actor.isSkillDisabled(sk) && (_actor.getCurrentMp() > _actor.getStat().getMpConsume(sk)))
				{
					_actor.doCast(sk);
					return;
				}
				maxRange = Math.Max(maxRange, sk.getCastRange());
			}
			
			if (!_isNotMoving)
			{
				moveToPawn(target, maxRange);
			}
		}
	}
	
	protected void thinkAttackGroup()
	{
		Creature target = getForcedTarget();
		if ((target == null) || target.isAlikeDead())
		{
			// try to get next group target
			setForcedTarget(findNextGroupTarget());
			clientStopMoving(null);
		}
		
		if (target == null)
		{
			return;
		}
		
		setTarget(target);
		// as a response, we put the target in a forcedattack mode
		ControllableMob theTarget = (ControllableMob) target;
		ControllableMobAI ctrlAi = (ControllableMobAI) theTarget.getAI();
		ctrlAi.forceAttack(_actor);
		
		double dist2 = _actor.DistanceSquare2D(target);
		int range = _actor.getPhysicalAttackRange() + _actor.getTemplate().getCollisionRadius() + target.getTemplate().getCollisionRadius();
		int maxRange = range;
		if (!_actor.isMuted() && (dist2 > ((range + 20) * (range + 20))))
		{
			// check distant skills
			foreach (Skill sk in _actor.getAllSkills())
			{
				int castRange = sk.getCastRange();
				if (((castRange * castRange) >= dist2) && !_actor.isSkillDisabled(sk) && (_actor.getCurrentMp() > _actor.getStat().getMpConsume(sk)))
				{
					_actor.doCast(sk);
					return;
				}
				
				maxRange = Math.Max(maxRange, castRange);
			}
			
			if (!_isNotMoving)
			{
				moveToPawn(target, range);
			}
			
			return;
		}
		_actor.doAutoAttack(target);
	}
	
	protected void thinkForceAttack()
	{
		if ((getForcedTarget() == null) || getForcedTarget().isAlikeDead())
		{
			clientStopMoving(null);
			setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
			setAlternateAI(AI_IDLE);
		}
		
		setTarget(getForcedTarget());
		double dist2 = _actor.DistanceSquare2D(getForcedTarget());
		int range = _actor.getPhysicalAttackRange() + _actor.getTemplate().getCollisionRadius() + getForcedTarget().getTemplate().getCollisionRadius();
		int maxRange = range;
		if (!_actor.isMuted() && (dist2 > ((range + 20) * (range + 20))))
		{
			// check distant skills
			foreach (Skill sk in _actor.getAllSkills())
			{
				int castRange = sk.getCastRange();
				if (((castRange * castRange) >= dist2) && !_actor.isSkillDisabled(sk) && (_actor.getCurrentMp() > _actor.getStat().getMpConsume(sk)))
				{
					_actor.doCast(sk);
					return;
				}
				
				maxRange = Math.Max(maxRange, castRange);
			}
			
			if (!_isNotMoving)
			{
				moveToPawn(getForcedTarget(), _actor.getPhysicalAttackRange()/* range */);
			}
			
			return;
		}
		
		_actor.doAutoAttack(getForcedTarget());
	}
	
	protected override void thinkAttack()
	{
		Creature target = getForcedTarget();
		if ((target == null) || target.isAlikeDead())
		{
			if (target != null)
			{
				// stop hating
				Attackable npc = (Attackable) _actor;
				npc.stopHating(target);
			}
			
			setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
		}
		else
		{
			// notify aggression
			Creature finalTarget = target;
			if (((Npc) _actor).getTemplate().getClans() != null)
			{
				World.getInstance().forEachVisibleObject<Npc>(_actor, npc =>
				{
					if (!npc.isInMyClan((Npc) _actor))
					{
						return;
					}
					
					if (_actor.IsInsideRadius3D(npc, npc.getTemplate().getClanHelpRange()))
					{
						npc.getAI().notifyEvent(CtrlEvent.EVT_AGGRESSION, finalTarget, 1);
					}
				});
			}
			
			setTarget(target);
			double dist2 = _actor.DistanceSquare2D(target);
			int range = _actor.getPhysicalAttackRange() + _actor.getTemplate().getCollisionRadius() + target.getTemplate().getCollisionRadius();
			int maxRange = range;
			if (!_actor.isMuted() && (dist2 > ((range + 20) * (range + 20))))
			{
				// check distant skills
				foreach (Skill sk in _actor.getAllSkills())
				{
					int castRange = sk.getCastRange();
					if (((castRange * castRange) >= dist2) && !_actor.isSkillDisabled(sk) && (_actor.getCurrentMp() > _actor.getStat().getMpConsume(sk)))
					{
						_actor.doCast(sk);
						return;
					}
					
					maxRange = Math.Max(maxRange, castRange);
				}
				
				moveToPawn(target, range);
				return;
			}
			
			// Force mobs to attack anybody if confused.
			Creature hated;
			if (_actor.isConfused())
			{
				hated = findNextRndTarget();
			}
			else
			{
				hated = target;
			}
			
			if (hated == null)
			{
				setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
				return;
			}
			
			if (hated != target)
			{
				target = hated;
			}
			
			if (!_actor.isMuted() && (Rnd.get(5) == 3))
			{
				foreach (Skill sk in _actor.getAllSkills())
				{
					int castRange = sk.getCastRange();
					if (((castRange * castRange) >= dist2) && !_actor.isSkillDisabled(sk) && (_actor.getCurrentMp() < _actor.getStat().getMpConsume(sk)))
					{
						_actor.doCast(sk);
						return;
					}
				}
			}
			
			_actor.doAutoAttack(target);
		}
	}
	
	protected override void thinkActive()
	{
		Creature hated;
		if (_actor.isConfused())
		{
			hated = findNextRndTarget();
		}
		else
		{
			WorldObject target = _actor.getTarget();
			hated = (target != null) && target.isCreature() ? (Creature) target : null;
		}
		
		if (hated != null)
		{
			_actor.setRunning();
			setIntention(CtrlIntention.AI_INTENTION_ATTACK, hated);
		}
	}
	
	private bool checkAutoAttackCondition(Creature target)
	{
		if ((target == null) || !_actor.isAttackable())
		{
			return false;
		}
		
		if (target.isNpc() || target.isDoor())
		{
			return false;
		}
		
		Attackable me = (Attackable) _actor;
		if (target.isAlikeDead() || !me.IsInsideRadius2D(target, me.getAggroRange()) || (Math.Abs(_actor.getZ() - target.getZ()) > 100))
		{
			return false;
		}
		
		// Check if the target isn't invulnerable
		if (target.isInvul())
		{
			return false;
		}
		
		// Spawn protection (only against mobs)
		if (target.isPlayer() && ((Player) target).isSpawnProtected())
		{
			return false;
		}
		
		// Check if the target is a Playable and if the target isn't in silent move mode
		if (target.isPlayable() && ((Playable) target).isSilentMovingAffected())
		{
			return false;
		}
		
		if (target.isNpc())
		{
			return false;
		}
		
		return me.isAggressive();
	}
	
	private Creature findNextRndTarget()
	{
		List<Creature> potentialTarget = new();
		World.getInstance().forEachVisibleObject<Creature>(_actor, target =>
		{
			if (Util.checkIfInShortRange(((Attackable) _actor).getAggroRange(), _actor, target, true) && checkAutoAttackCondition(target))
			{
				potentialTarget.add(target);
			}
		});
		
		return !potentialTarget.isEmpty() ? potentialTarget.GetRandomElement() : null;
	}
	
	private ControllableMob findNextGroupTarget()
	{
		return getGroupTarget().getRandomMob();
	}
	
	public ControllableMobAI(ControllableMob controllableMob): base(controllableMob)
	{
		setAlternateAI(AI_IDLE);
	}
	
	public int getAlternateAI()
	{
		return _alternateAI;
	}
	
	public void setAlternateAI(int alternateAi)
	{
		_alternateAI = alternateAi;
	}
	
	public void forceAttack(Creature target)
	{
		setAlternateAI(AI_FORCEATTACK);
		setForcedTarget(target);
	}
	
	public void forceAttackGroup(MobGroup group)
	{
		setForcedTarget(null);
		setGroupTarget(group);
		setAlternateAI(AI_ATTACK_GROUP);
	}
	
	public void stop()
	{
		setAlternateAI(AI_IDLE);
		clientStopMoving(null);
	}

	public void follow(Creature target)
	{
		setAlternateAI(AI_FOLLOW);
		setForcedTarget(target);
	}
	
	public bool isThinking()
	{
		return _isThinking;
	}
	
	public bool isNotMoving()
	{
		return _isNotMoving;
	}
	
	public void setNotMoving(bool isNotMoving)
	{
		_isNotMoving = isNotMoving;
	}
	
	public void setThinking(bool isThinking)
	{
		_isThinking = isThinking;
	}
	
	private Creature getForcedTarget()
	{
		return _forcedTarget;
	}
	
	private MobGroup getGroupTarget()
	{
		return _targetGroup;
	}
	
	private void setForcedTarget(Creature forcedTarget)
	{
		_forcedTarget = forcedTarget;
	}
	
	private void setGroupTarget(MobGroup targetGroup)
	{
		_targetGroup = targetGroup;
	}
}

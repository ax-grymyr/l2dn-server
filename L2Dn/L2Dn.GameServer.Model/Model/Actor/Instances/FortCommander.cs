using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class FortCommander : Defender
{
	private bool _canTalk;
	
	public FortCommander(NpcTemplate template): base(template)
	{
		InstanceType = InstanceType.FortCommander;
		_canTalk = true;
	}
	
	/**
	 * Return True if a siege is in progress and the Creature attacker isn't a Defender.
	 * @param attacker The Creature that the Commander try to attack
	 */
	public override bool isAutoAttackable(Creature attacker)
	{
		if ((attacker == null) || !attacker.isPlayer())
		{
			return false;
		}
		
		// Attackable during siege by all except defenders
		return ((getFort() != null) && (getFort().getResidenceId() > 0) && getFort().getSiege().isInProgress() && !getFort().getSiege().checkIsDefender(attacker.getClan()));
	}
	
	public override void addDamageHate(Creature attacker, long damage, long aggro)
	{
		if (attacker == null)
		{
			return;
		}
		
		if (!(attacker is FortCommander))
		{
			base.addDamageHate(attacker, damage, aggro);
		}
	}
	
	public override bool doDie(Creature killer)
	{
		if (!base.doDie(killer))
		{
			return false;
		}
		
		if (getFort().getSiege().isInProgress())
		{
			getFort().getSiege().killedCommander(this);
		}
		
		return true;
	}
	
	/**
	 * This method forces guard to return to home location previously set
	 */
	public override void returnHome()
	{
		if (!this.IsInsideRadius2D(getSpawn(), 200))
		{
			clearAggroList();
			
			if (hasAI())
			{
				getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, getSpawn().Location.Location3D);
			}
		}
	}
	
	public override void addDamage(Creature creature, int damage, Skill skill)
	{
		Creature attacker = creature;
		Spawn spawn = getSpawn();
		if ((spawn != null) && canTalk())
		{
			List<FortSiegeSpawn> commanders = FortSiegeManager.getInstance().getCommanderSpawnList(getFort().getResidenceId());
			foreach (FortSiegeSpawn spawn2 in commanders)
			{
				if (spawn2.getId() == spawn.getId())
				{
					NpcStringId? npcString = null;
					switch (spawn2.getMessageId())
					{
						case 1:
						{
							npcString = NpcStringId.ATTACKING_THE_ENEMY_S_REINFORCEMENTS_IS_NECESSARY_TIME_TO_DIE;
							break;
						}
						case 2:
						{
							if (attacker.isSummon())
							{
								attacker = ((Summon) attacker).getOwner();
							}
							npcString = NpcStringId.EVERYONE_CONCENTRATE_YOUR_ATTACKS_ON_S1_SHOW_THE_ENEMY_YOUR_RESOLVE;
							break;
						}
						case 3:
						{
							npcString = NpcStringId.FIRE_SPIRIT_UNLEASH_YOUR_POWER_BURN_THE_ENEMY;
							break;
						}
					}
					
					if (npcString != null)
					{
						broadcastSay(ChatType.NPC_SHOUT, npcString.Value, npcString.Value.GetParamCount() == 1 ? attacker.getName() : null);
						setCanTalk(false);
						ThreadPool.schedule(new ScheduleTalkTask(this), 10000);
					}
				}
			}
		}
		base.addDamage(attacker, damage, skill);
	}
	
	private class ScheduleTalkTask : Runnable
	{
		private readonly FortCommander _commander;

		public ScheduleTalkTask(FortCommander commander)
		{
			_commander = commander;
		}
		
		public void run()
		{
			_commander.setCanTalk(true);
		}
	}
	
	void setCanTalk(bool value)
	{
		_canTalk = value;
	}
	
	private bool canTalk()
	{
		return _canTalk;
	}
	
	public override bool hasRandomAnimation()
	{
		return false;
	}
}

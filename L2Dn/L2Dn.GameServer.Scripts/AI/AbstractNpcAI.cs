using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Quests;
using NLog;

namespace L2Dn.GameServer.Scripts.AI;

/**
 * Abstract NPC AI class for datapack based AIs.
 * @author UnAfraid, Zoey76
 */
public abstract class AbstractNpcAI: Quest
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(AbstractNpcAI));
	
	public AbstractNpcAI(): base(-1)
	{
	}
	
	/**
	 * Simple on first talk event handler.
	 */
	public override string onFirstTalk(Npc npc, Player player)
	{
		return npc.getId() + ".html";
	}
	
	/**
	 * Registers the following events to the current script:<br>
	 * <ul>
	 * <li>ON_ATTACK</li>
	 * <li>ON_KILL</li>
	 * <li>ON_SPAWN</li>
	 * <li>ON_SPELL_FINISHED</li>
	 * <li>ON_SKILL_SEE</li>
	 * <li>ON_FACTION_CALL</li>
	 * <li>ON_AGGR_RANGE_ENTER</li>
	 * </ul>
	 * @param mobs
	 */
	public void registerMobs(params int[] mobs)
	{
		addAttackId(mobs);
		addKillId(mobs);
		addSpawnId(mobs);
		addSpellFinishedId(mobs);
		addSkillSeeId(mobs);
		addAggroRangeEnterId(mobs);
		addFactionCallId(mobs);
	}
	
	public void spawnMinions(Npc npc, string spawnName)
	{
		foreach (MinionHolder mh in npc.getParameters().getMinionList(spawnName))
		{
			addMinion((Monster) npc, mh.getId());
		}
	}
}
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Summon Trap effect implementation.
 * @author Zoey76
 */
public class SummonTrap: AbstractEffect
{
	private readonly int _despawnTime;
	private readonly int _npcId;

	public SummonTrap(StatSet @params)
	{
		_despawnTime = @params.getInt("despawnTime", 0);
		_npcId = @params.getInt("npcId", 0);
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
        Player? player = effected.getActingPlayer();
		if (!effected.isPlayer() || player == null || effected.isAlikeDead() || player.inObserverMode())
		{
			return;
		}

		if (_npcId <= 0)
		{
			LOGGER.Warn(GetType().Name + ": Invalid NPC ID:" + _npcId + " in skill ID: " + skill.getId());
			return;
		}

		if (player.inObserverMode() || player.isMounted())
		{
			return;
		}

		// Unsummon previous trap
		if (player.getTrap() != null)
		{
			player.getTrap().unSummon();
		}

		NpcTemplate? npcTemplate = NpcData.getInstance().getTemplate(_npcId);
		if (npcTemplate == null)
		{
			LOGGER.Warn(GetType().Name + ": Spawn of the non-existing Trap ID: " + _npcId + " in skill ID:" + skill.getId());
			return;
		}

		Trap trap = new Trap(npcTemplate, player, _despawnTime);
		trap.setCurrentHp(trap.getMaxHp());
		trap.setCurrentMp(trap.getMaxMp());
		trap.setInvul(true);
		trap.setHeading(player.getHeading());
		trap.spawnMe(player.Location.Location3D);
		player.addSummonedNpc(trap); // player.setTrap(trap);
	}
}
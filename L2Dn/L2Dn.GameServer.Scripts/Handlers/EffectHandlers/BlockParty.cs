using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Punishment;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Block Party effect implementation.
 * @author BiggBoss
 */
public class BlockParty: AbstractEffect
{
	public BlockParty(StatSet @params)
	{
	}

	public override bool canStart(Creature effector, Creature effected, Skill skill)
	{
		return (effected != null) && effected.isPlayer();
	}

	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		PunishmentManager.getInstance().startPunishment(new PunishmentTask(0, effected.getObjectId().ToString(),
			PunishmentAffect.CHARACTER, PunishmentType.PARTY_BAN, null, "Party banned by bot report", "system", true));
	}

	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		PunishmentManager.getInstance().stopPunishment(effected.getObjectId().ToString(), PunishmentAffect.CHARACTER,
			PunishmentType.PARTY_BAN);
	}
}
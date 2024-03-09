using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Punishment;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Block Chat effect implementation.
 * @author BiggBoss
 */
public class BlockChat: AbstractEffect
{
	public BlockChat(StatSet @params)
	{
	}
	
	public override bool canStart(Creature effector, Creature effected, Skill skill)
	{
		return (effected != null) && effected.isPlayer();
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.CHAT_BLOCK.getMask();
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		PunishmentManager.getInstance().startPunishment(new PunishmentTask(0, effected.getObjectId().ToString(),
			PunishmentAffect.CHARACTER, PunishmentType.CHAT_BAN, null, "Chat banned bot report", "system", true));
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		PunishmentManager.getInstance().stopPunishment(effected.getObjectId().ToString(), PunishmentAffect.CHARACTER,
			PunishmentType.CHAT_BAN);
	}
}
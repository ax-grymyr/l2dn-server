using L2Dn.GameServer.Data;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Punishment;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Block Action effect implementation.
 * @author BiggBoss
 */
public class BlockAction: AbstractEffect
{
	private readonly Set<int> _blockedActions = new();

	public BlockAction(StatSet @params)
	{
		string[] actions = @params.getString("blockedActions").Split(",");
		foreach (string action in actions)
		{
			_blockedActions.add(int.Parse(action));
		}
	}

	public override bool canStart(Creature effector, Creature effected, Skill skill)
	{
		return (effected != null) && effected.isPlayer();
	}

	public override bool checkCondition(int id)
	{
		return !_blockedActions.Contains(id);
	}

	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (_blockedActions.Contains(BotReportTable.PARTY_ACTION_BLOCK_ID))
		{
			PunishmentManager.getInstance().startPunishment(new PunishmentTask(0, effected.ObjectId.ToString(),
				PunishmentAffect.CHARACTER, PunishmentType.PARTY_BAN, null, "block action debuff", "system", true));
		}

		if (_blockedActions.Contains(BotReportTable.CHAT_BLOCK_ID))
		{
			PunishmentManager.getInstance().startPunishment(new PunishmentTask(0, effected.ObjectId.ToString(),
				PunishmentAffect.CHARACTER, PunishmentType.CHAT_BAN, null, "block action debuff", "system", true));
		}
	}

	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		if (_blockedActions.Contains(BotReportTable.PARTY_ACTION_BLOCK_ID))
		{
			PunishmentManager.getInstance().stopPunishment(effected.ObjectId.ToString(),
				PunishmentAffect.CHARACTER, PunishmentType.PARTY_BAN);
		}

		if (_blockedActions.Contains(BotReportTable.CHAT_BLOCK_ID))
		{
			PunishmentManager.getInstance().stopPunishment(effected.ObjectId.ToString(),
				PunishmentAffect.CHARACTER, PunishmentType.CHAT_BAN);
		}
	}
}
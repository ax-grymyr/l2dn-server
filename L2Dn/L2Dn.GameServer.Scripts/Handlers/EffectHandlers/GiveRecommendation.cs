using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Give Recommendation effect implementation.
 * @author NosBit
 */
public class GiveRecommendation: AbstractEffect
{
	private readonly int _amount;

	public GiveRecommendation(StatSet @params)
	{
		_amount = @params.getInt("amount", 0);
		if (_amount == 0)
		{
			throw new ArgumentException("amount parameter is missing or set to 0.");
		}
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		Player? target = effected != null && effected.isPlayer() ? (Player) effected : null;
		if (target != null)
		{
			int recommendationsGiven = _amount;
			if (target.getRecomHave() + _amount >= 255)
			{
				recommendationsGiven = 255 - target.getRecomHave();
			}

			if (recommendationsGiven > 0)
			{
				target.setRecomHave(target.getRecomHave() + recommendationsGiven);

				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_OBTAINED_S1_RECOMMENDATION_S);
				sm.Params.addInt(recommendationsGiven);
				target.sendPacket(sm);
				target.updateUserInfo();
				target.sendPacket(new ExVoteSystemInfoPacket(target));
			}
			else
			{
				Player? player = effector != null && effector.isPlayer() ? (Player) effector : null;
				if (player != null)
				{
					player.sendPacket(SystemMessageId.NOTHING_HAPPENED);
				}
			}
		}
	}
}
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Checks Sweeper conditions:
 * <ul>
 * <li>Minimum checks, player not null, skill not null.</li>
 * <li>Checks if the target isn't null, is dead and spoiled.</li>
 * <li>Checks if the sweeper player is the target spoiler, or is in the spoiler party.</li>
 * <li>Checks if the corpse is too old.</li>
 * <li>Checks inventory limit and weight max load won't be exceed after sweep.</li>
 * </ul>
 * If two or more conditions aren't meet at the same time, one message per condition will be shown.
 * @author Zoey76
 */
public class ConditionPlayerCanSweep: Condition
{
	private readonly bool _value;
	
	public ConditionPlayerCanSweep(bool value)
	{
		_value = value;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		bool canSweep = false;
		if (effector.getActingPlayer() != null)
		{
			Player sweeper = effector.getActingPlayer();
			if (skill != null)
			{
				foreach (WorldObject wo in skill.getTargetsAffected(sweeper, effected))
				{
					if ((wo != null) && wo.isAttackable())
					{
						Attackable attackable = (Attackable) wo;
						if (attackable.isDead())
						{
							if (attackable.isSpoiled())
							{
								canSweep = attackable.checkSpoilOwner(sweeper, true);
								if (canSweep)
								{
									canSweep = !attackable.isOldCorpse(sweeper, TimeSpan.FromMilliseconds(Config.CORPSE_CONSUME_SKILL_ALLOWED_TIME_BEFORE_DECAY), true);
								}
								if (canSweep)
								{
									canSweep = sweeper.getInventory().checkInventorySlotsAndWeight(attackable.getSpoilLootItems(), true, true);
								}
							}
							else
							{
								sweeper.sendPacket(SystemMessageId.THE_SWEEPER_HAS_FAILED_AS_THE_TARGET_IS_NOT_SPOILED);
							}
						}
					}
				}
			}
		}
		return _value == canSweep;
	}
}

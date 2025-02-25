using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Actor.Status;

public class SummonStatus : PlayableStatus
{
	public SummonStatus(Summon activeChar): base(activeChar)
	{
	}

	public override void reduceHp(double value, Creature attacker)
	{
		reduceHp(value, attacker, true, false, false);
	}

	public override void reduceHp(double amount, Creature attacker, bool awake, bool isDOT, bool isHPConsumption)
	{
		if (attacker == null || getActiveChar().isDead())
		{
			return;
		}

		Player? attackerPlayer = attacker.getActingPlayer();
		if (attackerPlayer != null && (getActiveChar().getOwner() == null || getActiveChar().getOwner().getDuelId() != attackerPlayer.getDuelId()))
		{
			attackerPlayer.setDuelState(Duel.DUELSTATE_INTERRUPTED);
		}

		double value = amount;
		Player caster = getActiveChar().getTransferingDamageTo();
		if (getActiveChar().getOwner().getParty() != null)
		{
			if (caster != null && Util.checkIfInRange(1000, getActiveChar(), caster, true) && !caster.isDead() && getActiveChar().getParty().getMembers().Contains(caster))
			{
				int transferDmg = (int) value * (int) getActiveChar().getStat().getValue(Stat.TRANSFER_DAMAGE_TO_PLAYER, 0) / 100;
				transferDmg = Math.Min((int) caster.getCurrentHp() - 1, transferDmg);
				if (transferDmg > 0)
				{
					int membersInRange = 0;
					foreach (Player member in caster.getParty().getMembers())
					{
						if (Util.checkIfInRange(1000, member, caster, false) && member != caster)
						{
							membersInRange++;
						}
					}
					if (attacker.isPlayable() && caster.getCurrentCp() > 0)
					{
						if (caster.getCurrentCp() > transferDmg)
						{
							caster.getStatus().reduceCp(transferDmg);
						}
						else
						{
							transferDmg = (int) (transferDmg - caster.getCurrentCp());
							caster.getStatus().reduceCp((int) caster.getCurrentCp());
						}
					}
					if (membersInRange > 0)
					{
						caster.reduceCurrentHp(transferDmg / membersInRange, attacker, null);
						value -= transferDmg;
					}
				}
			}
		}
		else if (caster != null && caster == getActiveChar().getOwner() && Util.checkIfInRange(1000, getActiveChar(), caster, true) && !caster.isDead()) // when no party, transfer only to owner (caster)
		{
			int transferDmg = (int) value * (int) getActiveChar().getStat().getValue(Stat.TRANSFER_DAMAGE_TO_PLAYER, 0) / 100;
			transferDmg = Math.Min((int) caster.getCurrentHp() - 1, transferDmg);
			if (transferDmg > 0)
			{
				if (attacker.isPlayable() && caster.getCurrentCp() > 0)
				{
					if (caster.getCurrentCp() > transferDmg)
					{
						caster.getStatus().reduceCp(transferDmg);
					}
					else
					{
						transferDmg = (int) (transferDmg - caster.getCurrentCp());
						caster.getStatus().reduceCp((int) caster.getCurrentCp());
					}
				}

				caster.reduceCurrentHp(transferDmg, attacker, null);
				value -= transferDmg;
			}
		}

		base.reduceHp(value, attacker, awake, isDOT, isHPConsumption);
	}

	public override Summon getActiveChar()
	{
		return (Summon)base.getActiveChar();
	}
}
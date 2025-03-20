using L2Dn.Extensions;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor.Stats;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Model.Actor.Status;

public class PlayerStatus: PlayableStatus
{
	private double _currentCp; // Current CP of the Player

	public PlayerStatus(Player player)
		: base(player)
	{
	}

	public override void reduceCp(int value)
	{
		if (_currentCp > value)
		{
			setCurrentCp(_currentCp - value);
		}
		else
		{
			setCurrentCp(0);
		}
	}

	public override void reduceHp(double value, Creature attacker)
	{
		reduceHp(value, attacker, null, true, false, false, false);
	}

	public override void reduceHp(double value, Creature? attacker, bool awake, bool isDOT, bool isHPConsumption)
	{
		reduceHp(value, attacker, null, awake, isDOT, isHPConsumption, false);
	}

	public void reduceHp(double value, Creature? attacker, Skill? skill, bool awake, bool isDOT, bool isHPConsumption,
		bool ignoreCP)
	{
		if (getActiveChar().isDead())
		{
			return;
		}

		// If OFFLINE_MODE_NO_DAMAGE is enabled and player is offline,
		// and he is in store/craft mode, no damage is taken.
        GameSession? client = getActiveChar().getClient();
		if (Config.OfflineTrade.OFFLINE_MODE_NO_DAMAGE && client != null &&
            client.IsDetached &&
		    ((Config.OfflineTrade.OFFLINE_TRADE_ENABLE && (getActiveChar().getPrivateStoreType() == PrivateStoreType.SELL ||
		                                      getActiveChar().getPrivateStoreType() == PrivateStoreType.BUY)) ||
		     (Config.OfflineTrade.OFFLINE_CRAFT_ENABLE && (getActiveChar().isCrafting() ||
		                                      getActiveChar().getPrivateStoreType() ==
                                              PrivateStoreType.MANUFACTURE))))
		{
			return;
		}

		if (getActiveChar().isHpBlocked() && !(isDOT || isHPConsumption))
		{
			return;
		}

		if (getActiveChar().isAffected(EffectFlags.DUELIST_FURY) && attacker != null && !attacker.isAffected(EffectFlags.FACEOFF))
		{
			return;
		}

		if (!isHPConsumption)
		{
			if (awake)
			{
				getActiveChar().stopEffectsOnDamage();
			}

			// Attacked players in craft/shops stand up.
			if (getActiveChar().isCrafting() || getActiveChar().isInStoreMode())
			{
				getActiveChar().setPrivateStoreType(PrivateStoreType.NONE);
				getActiveChar().standUp();
				getActiveChar().broadcastUserInfo();
			}
			else if (getActiveChar().isSitting())
			{
				getActiveChar().standUp();
			}

			if (!isDOT)
			{
				if (Formulas.calcStunBreak(getActiveChar()))
				{
					getActiveChar().stopStunning(true);
				}

				if (Formulas.calcRealTargetBreak())
				{
					getActiveChar().getEffectList().stopEffects(AbnormalType.REAL_TARGET);
				}
			}
		}

		double amount = value;
		int fullValue = (int)amount;
		int tDmg = 0;
		int mpDam = 0;
		if (attacker != null && attacker != getActiveChar())
		{
			Player? attackerPlayer = attacker.getActingPlayer();
			if (attackerPlayer != null)
			{
				if (attackerPlayer.isGM() && !attackerPlayer.getAccessLevel().CanGiveDamage)
				{
					return;
				}

				if (getActiveChar().isInDuel())
				{
					if (getActiveChar().getDuelState() == Duel.DUELSTATE_DEAD)
					{
						return;
					}
					else if (getActiveChar().getDuelState() == Duel.DUELSTATE_WINNER)
					{
						return;
					}

					// cancel duel if player got hit by another player, that is not part of the duel
					if (attackerPlayer.getDuelId() != getActiveChar().getDuelId())
					{
						getActiveChar().setDuelState(Duel.DUELSTATE_INTERRUPTED);
					}
				}
			}

			// Check and calculate transfered damage
			Summon? summon = getActiveChar().getFirstServitor();
			if (summon != null && Util.checkIfInRange(1000, getActiveChar(), summon, true))
			{
				tDmg = (int)amount * (int)getActiveChar().getStat().getValue(Stat.TRANSFER_DAMAGE_SUMMON_PERCENT, 0) /
				       100;

				// Only transfer dmg up to current HP, it should not be killed
				tDmg = Math.Min((int)summon.getCurrentHp() - 1, tDmg);
				if (tDmg > 0)
				{
					summon.reduceCurrentHp(tDmg, attacker, null);
					amount -= tDmg;
					fullValue =
						(int)amount; // reduce the announced value here as player will get a message about summon damage
				}
			}

			mpDam = (int)amount * (int)getActiveChar().getStat().getValue(Stat.MANA_SHIELD_PERCENT, 0) / 100;
			if (mpDam > 0)
			{
				mpDam = (int)(amount - mpDam);
				if (mpDam > getActiveChar().getCurrentMp())
				{
					getActiveChar().sendPacket(SystemMessageId.MP_BECAME_0_AND_THE_ARCANE_SHIELD_IS_DISAPPEARING);
					getActiveChar().stopSkillEffects(SkillFinishType.REMOVED, 1556);
					amount = mpDam - getActiveChar().getCurrentMp();
					getActiveChar().setCurrentMp(0);
				}
				else
				{
					getActiveChar().reduceCurrentMp(mpDam);
					SystemMessagePacket smsg =
						new SystemMessagePacket(SystemMessageId.ARCANE_SHIELD_S1_DECREASED_YOUR_MP_INSTEAD_OF_HP);
					smsg.Params.addInt(mpDam);
					getActiveChar().sendPacket(smsg);
					return;
				}
			}

			Player? caster = getActiveChar().getTransferingDamageTo();
            Party? party = getActiveChar().getParty();
            Party? casterParty = caster?.getParty();
			if (caster != null && party != null &&
			    Util.checkIfInRange(1000, getActiveChar(), caster, true) && !caster.isDead() &&
			    getActiveChar() != caster && party.getMembers().Contains(caster) && casterParty != null)
			{
                int transferDmg = (int)amount * (int)getActiveChar().getStat().getValue(Stat.TRANSFER_DAMAGE_TO_PLAYER, 0) / 100;
				transferDmg = Math.Min((int)caster.getCurrentHp() - 1, transferDmg);
				if (transferDmg > 0)
				{
					int membersInRange = 0;
					foreach (Player member in casterParty.getMembers())
					{
						if (Util.checkIfInRange(1000, member, caster, false) && member != caster)
						{
							membersInRange++;
						}
					}

					if ((attacker.isPlayable() || attacker.isFakePlayer()) && caster.getCurrentCp() > 0)
					{
						if (caster.getCurrentCp() > transferDmg)
						{
							caster.getStatus().reduceCp(transferDmg);
						}
						else
						{
							transferDmg = (int)(transferDmg - caster.getCurrentCp());
							caster.getStatus().reduceCp((int)caster.getCurrentCp());
						}
					}

					if (membersInRange > 0)
					{
						caster.reduceCurrentHp(transferDmg / membersInRange, attacker, null);
						amount -= transferDmg;
						fullValue = (int)amount;
					}
				}
			}

			if (!ignoreCP && (attacker.isPlayable() || attacker.isFakePlayer()))
			{
				if (_currentCp >= amount)
				{
					setCurrentCp(_currentCp - amount); // Set Cp to diff of Cp vs value
					amount = 0; // No need to subtract anything from Hp
				}
				else
				{
					amount -= _currentCp; // Get diff from value vs Cp; will apply diff to Hp
					setCurrentCp(0, false); // Set Cp to 0
				}
			}

			if (fullValue > 0 && !isDOT)
			{
				// Send a System Message to the Player
				SystemMessagePacket smsg = new SystemMessagePacket(SystemMessageId.C1_HAS_RECEIVED_S3_DAMAGE_FROM_C2);
				smsg.Params.addString(getActiveChar().getName());

				// Localisation related.
				string targetName = attacker.getName();
				if (Config.MultilingualSupport.MULTILANG_ENABLE && attacker.isNpc())
				{
					string[]? localisation = NpcNameLocalisationData.getInstance()
						.getLocalisation(getActiveChar()?.getLang() ?? "en", attacker.Id);
					if (localisation != null)
					{
						targetName = localisation[0];
					}
				}

				smsg.Params.addString(targetName);
				smsg.Params.addInt(fullValue);
				smsg.Params.addPopup(getActiveChar().ObjectId, attacker.ObjectId, -fullValue);
				getActiveChar().sendPacket(smsg);

				if (tDmg > 0 && summon != null && attackerPlayer != null)
				{
					smsg = new SystemMessagePacket(SystemMessageId
						.YOU_VE_DEALT_S1_DAMAGE_TO_YOUR_TARGET_AND_S2_DAMAGE_TO_THEIR_SERVITOR);
					smsg.Params.addInt(fullValue);
					smsg.Params.addInt(tDmg);
					attackerPlayer.sendPacket(smsg);
				}
			}
		}

		if (amount > 0)
		{
            if (attacker != null)
                getActiveChar().addDamageTaken(attacker, skill != null ? skill.DisplayId : 0, amount);

            double newHp = Math.Max(getCurrentHp() - amount, getActiveChar().isUndying() ? 1 : 0);
			if (newHp <= 0)
			{
				if (getActiveChar().isInDuel())
				{
					getActiveChar().disableAllSkills();
					stopHpMpRegeneration();
					if (attacker != null)
					{
						attacker.getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
						attacker.sendPacket(ActionFailedPacket.STATIC_PACKET);
					}

					// let the DuelManager know of his defeat
					DuelManager.getInstance().onPlayerDefeat(getActiveChar());
					newHp = 1;
				}
				else
				{
					newHp = 0;
				}
			}

			setCurrentHp(newHp);
		}

		if (getActiveChar().getCurrentHp() < 0.5 && !isHPConsumption && !getActiveChar().isUndying())
		{
			getActiveChar().abortAttack();
			getActiveChar().abortCast();

			if (getActiveChar().isInOlympiadMode())
			{
				stopHpMpRegeneration();
				getActiveChar().setDead(true);
				getActiveChar().setIsPendingRevive(true);
				Summon? pet = getActiveChar().getPet();
				if (pet != null)
				{
					pet.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
				}

				getActiveChar().getServitors().Values
					.ForEach(s => s.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE));
				return;
			}

			getActiveChar().doDie(attacker);
		}
	}

	public override double getCurrentCp()
	{
		return _currentCp;
	}

	public override void setCurrentCp(double newCp)
	{
		setCurrentCp(newCp, true);
	}

	public override void setCurrentCp(double value, bool broadcastPacket)
	{
		// Get the Max CP of the Creature
		int currentCp = (int)_currentCp;
		int maxCp = getActiveChar().getStat().getMaxCp();

		lock (this)
		{
			if (getActiveChar().isDead())
			{
				return;
			}

			double newCp = Math.Max(0, value);
			if (newCp >= maxCp)
			{
				// Set the RegenActive flag to false
				_currentCp = maxCp;
				_flagsRegenActive &= ~REGEN_FLAG_CP;

				// Stop the HP/MP/CP Regeneration task
				if (_flagsRegenActive == 0)
				{
					stopHpMpRegeneration();
				}
			}
			else
			{
				// Set the RegenActive flag to true
				_currentCp = newCp;
				_flagsRegenActive |= REGEN_FLAG_CP;

				// Start the HP/MP/CP Regeneration task with Medium priority
				startHpMpRegeneration();
			}
		}

		// Send the Server->Client packet StatusUpdate with current HP and MP to all other Player to inform
		if (currentCp != _currentCp && broadcastPacket)
		{
			getActiveChar().broadcastStatusUpdate();
		}
	}

	protected override void doRegeneration()
	{
		PlayerStat charstat = getActiveChar().getStat();

		// Modify the current CP of the Creature and broadcast Server->Client packet StatusUpdate
		if (_currentCp < charstat.getMaxRecoverableCp())
		{
			setCurrentCp(_currentCp + getActiveChar().getStat().getValue(Stat.REGENERATE_CP_RATE), false);
		}

		// Modify the current HP of the Creature and broadcast Server->Client packet StatusUpdate
		if (getCurrentHp() < charstat.getMaxRecoverableHp())
		{
			setCurrentHp(getCurrentHp() + getActiveChar().getStat().getValue(Stat.REGENERATE_HP_RATE), false);
		}

		// Modify the current MP of the Creature and broadcast Server->Client packet StatusUpdate
		if (getCurrentMp() < charstat.getMaxRecoverableMp())
		{
			setCurrentMp(getCurrentMp() + getActiveChar().getStat().getValue(Stat.REGENERATE_MP_RATE), false);
		}

		getActiveChar().broadcastStatusUpdate(); // send the StatusUpdate packet
	}

	public override Player getActiveChar()
	{
		return (Player)base.getActiveChar();
	}
}
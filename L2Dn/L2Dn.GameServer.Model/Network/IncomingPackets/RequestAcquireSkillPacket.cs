using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Quests;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestAcquireSkillPacket: IIncomingPacket<GameSession>
{
    private int _id;
    private int _level;
    private int _subLevel;
    private AcquireSkillType _skillType;
    private int _subType;

    public void ReadContent(PacketBitReader reader)
    {
        _id = reader.ReadInt32();
        _level = reader.ReadInt16();
        _subLevel = reader.ReadInt16();
        _skillType = (AcquireSkillType)reader.ReadInt32();
        if (_skillType == AcquireSkillType.SUBPLEDGE)
        {
            _subType = reader.ReadInt32();
        }
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;

		if (player.isTransformed() || player.isMounted())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_USE_THE_SKILL_ENHANCING_FUNCTION_IN_THIS_STATE_YOU_CAN_ENHANCE_SKILLS_WHEN_NOT_IN_BATTLE_AND_CANNOT_USE_THE_FUNCTION_WHILE_TRANSFORMED_IN_BATTLE_ON_A_MOUNT_OR_WHILE_THE_SKILL_IS_ON_COOLDOWN);
			return ValueTask.CompletedTask;
		}

		if (_level < 1 || _level > 1000 || _id < 1)
		{
			Util.handleIllegalPlayerAction(player, "Wrong Packet Data in Aquired Skill", Config.General.DEFAULT_PUNISH);
			PacketLogger.Instance.Warn("Recived Wrong Packet Data in Aquired Skill - id: " + _id + " level: " + _level + " for " + player);
			return ValueTask.CompletedTask;
		}

		Npc? trainer = player.getLastFolkNPC();
		if (_skillType != AcquireSkillType.CLASS &&
		    (trainer == null || !trainer.isNpc() || (!trainer.canInteract(player) && !player.isGM())))
			return ValueTask.CompletedTask;

		int skillId = player.getReplacementSkill(_id);
		Skill? existingSkill = player.getKnownSkill(skillId); // Mobius: Keep existing sublevel.
		Skill? skill = SkillData.getInstance().getSkill(skillId, _level, existingSkill?.getSubLevel() ?? 0);
		if (skill == null)
		{
			PacketLogger.Instance.Warn(
				$"{GetType().Name}: {player} is trying to learn a null skill Id: {_id} level: {_level}!");

			return ValueTask.CompletedTask;
		}

		// Hack check. Doesn't apply to all Skill Types
		int prevSkillLevel = player.getSkillLevel(skillId);
		if (_skillType != AcquireSkillType.TRANSFER && _skillType != AcquireSkillType.SUBPLEDGE)
		{
			if (prevSkillLevel == _level)
				return ValueTask.CompletedTask;

			if (prevSkillLevel != _level - 1)
			{
				// The previous level skill has not been learned.
				player.sendPacket(SystemMessageId.THE_PREVIOUS_LEVEL_SKILL_HAS_NOT_BEEN_LEARNED);
				Util.handleIllegalPlayerAction(player,
					$"{player} is requesting skill Id: {_id} level {_level} without knowing it's previous level!",
					IllegalActionPunishmentType.NONE);

				return ValueTask.CompletedTask;
			}
		}

		SkillLearn? s = SkillTreeData.getInstance().getSkillLearn(_skillType, player.getOriginalSkill(_id), _level, player);
		if (s == null)
			return ValueTask.CompletedTask;

		switch (_skillType)
		{
			case AcquireSkillType.CLASS:
			{
				if (checkPlayerSkill(player, trainer, s, _level))
				{
					giveSkill(player, trainer, skill);
				}

				break;
			}
			case AcquireSkillType.TRANSFORM:
			{
				// Hack check.
				if (!canTransform(player))
				{
					player.sendPacket(SystemMessageId.YOU_HAVE_NOT_COMPLETED_THE_NECESSARY_QUEST_FOR_SKILL_ACQUISITION);
					Util.handleIllegalPlayerAction(player,
						player + " is requesting skill Id: " + _id + " level " + _level +
						" without required quests!", IllegalActionPunishmentType.NONE);

					return ValueTask.CompletedTask;
				}

				if (checkPlayerSkill(player, trainer, s, _level))
				{
					giveSkill(player, trainer, skill);
				}

				break;
			}
			case AcquireSkillType.FISHING:
			{
				if (checkPlayerSkill(player, trainer, s, _level))
				{
					giveSkill(player, trainer, skill);
				}

				break;
			}
			case AcquireSkillType.PLEDGE:
			{
                Clan? clan = player.getClan();
				if (clan is null || !player.isClanLeader())
					return ValueTask.CompletedTask;

				int repCost = (int) s.getLevelUpSp(); // Hopefully not greater that max int.
				if (clan.getReputationScore() >= repCost)
				{
					if (Config.Character.LIFE_CRYSTAL_NEEDED)
					{
						int count = 0;
						long playerItemCount = 0;
						foreach (List<ItemHolder> items in s.getRequiredItems())
						{
							count = 0;
							foreach (ItemHolder item in items)
							{
								count++;
								playerItemCount = player.getInventory().getInventoryItemCount(item.getId(), -1);
								if (playerItemCount >= item.getCount() &&
								    player.destroyItemByItemId("PledgeLifeCrystal", item.getId(), item.getCount(),
									    trainer, true))
								{
									break;
								}

								if (count == items.Count)
								{
									player.sendPacket(SystemMessageId.NOT_ENOUGH_ITEMS_TO_LEARN_THE_SKILL);
									VillageMaster.showPledgeSkillList(player);
									return ValueTask.CompletedTask;
								}
							}
						}
					}

					clan.takeReputationScore(repCost);

					SystemMessagePacket cr = new SystemMessagePacket(SystemMessageId.CLAN_REPUTATION_POINTS_S1_2);
					cr.Params.addInt(repCost);
					player.sendPacket(cr);

					clan.addNewSkill(skill);

					clan.broadcastToOnlineMembers(new PledgeSkillListPacket(clan));
					player.sendPacket(new AcquireSkillDonePacket());
					VillageMaster.showPledgeSkillList(player);
				}
				else
				{
					player.sendPacket(SystemMessageId.THE_ATTEMPT_TO_ACQUIRE_THE_SKILL_HAS_FAILED_BECAUSE_OF_AN_INSUFFICIENT_CLAN_REPUTATION);
					VillageMaster.showPledgeSkillList(player);
				}

				break;
			}
			case AcquireSkillType.SUBPLEDGE:
			{
                Clan? clan = player.getClan();
				if (clan is null || !player.isClanLeader() || !player.hasClanPrivilege(ClanPrivilege.CL_TROOPS_FAME))
					return ValueTask.CompletedTask;

				if (clan.getFortId() == 0 && clan.getCastleId() == 0)
					return ValueTask.CompletedTask;

				// Hack check. Check if SubPledge can accept the new skill:
				if (!clan.isLearnableSubPledgeSkill(skill, _subType))
				{
					player.sendPacket(SystemMessageId.THIS_SQUAD_SKILL_HAS_ALREADY_BEEN_LEARNED);
					Util.handleIllegalPlayerAction(player,
						player + " is requesting skill Id: " + _id + " level " + _level +
						" without knowing it's previous level!", IllegalActionPunishmentType.NONE);

					return ValueTask.CompletedTask;
				}

				int repCost = (int) s.getLevelUpSp(); // Hopefully not greater that max int.
				if (clan.getReputationScore() < repCost)
				{
					player.sendPacket(SystemMessageId.THE_ATTEMPT_TO_ACQUIRE_THE_SKILL_HAS_FAILED_BECAUSE_OF_AN_INSUFFICIENT_CLAN_REPUTATION);
					return ValueTask.CompletedTask;
				}

				int count = 0;
				long playerItemCount = 0;
				foreach (List<ItemHolder> items in s.getRequiredItems())
				{
					count = 0;
					foreach (ItemHolder item in items)
					{
						count++;
						playerItemCount = player.getInventory().getInventoryItemCount(item.getId(), -1);
						if (playerItemCount >= item.getCount() && player.destroyItemByItemId("SubSkills", item.getId(), item.getCount(), trainer, true))
						{
							break;
						}

						if (count == items.Count)
						{
							player.sendPacket(SystemMessageId.NOT_ENOUGH_ITEMS_TO_LEARN_THE_SKILL);
							return ValueTask.CompletedTask;
						}
					}
				}

				if (repCost > 0)
				{
					clan.takeReputationScore(repCost);
					SystemMessagePacket cr = new SystemMessagePacket(SystemMessageId.CLAN_REPUTATION_POINTS_S1_2);
					cr.Params.addInt(repCost);
					player.sendPacket(cr);
				}

				clan.addNewSkill(skill, _subType);
				clan.broadcastToOnlineMembers(new PledgeSkillListPacket(clan));
				player.sendPacket(new AcquireSkillDonePacket());
				showSubUnitSkillList(player);
				break;
			}
			case AcquireSkillType.TRANSFER:
			{
				if (checkPlayerSkill(player, trainer, s, _level))
				{
					giveSkill(player, trainer, skill);
				}

				List<SkillLearn> skills = SkillTreeData.getInstance().getAvailableTransferSkills(player);
				if (skills.Count == 0)
				{
					player.sendPacket(SystemMessageId.THERE_ARE_NO_OTHER_SKILLS_TO_LEARN);
				}
				else
				{
					player.sendPacket(new ExAcquirableSkillListByClassPacket(skills, AcquireSkillType.TRANSFER));
				}
				break;
			}
			case AcquireSkillType.SUBCLASS:
			{
				if (player.isSubClassActive())
				{
					player.sendPacket(SystemMessageId.THIS_SKILL_CANNOT_BE_LEARNED_WHILE_IN_THE_SUBCLASS_STATE_PLEASE_TRY_AGAIN_AFTER_CHANGING_TO_THE_MAIN_CLASS);
					Util.handleIllegalPlayerAction(player, player + " is requesting skill Id: " + _id + " level " + _level + " while Sub-Class is active!", IllegalActionPunishmentType.NONE);
					return ValueTask.CompletedTask;
				}

				if (checkPlayerSkill(player, trainer, s, _level))
				{
					PlayerVariables vars = player.getVariables();
					string list = vars.Get("SubSkillList", string.Empty);
					if (prevSkillLevel > 0 && list.contains(_id + "-" + prevSkillLevel))
					{
						list = list.Replace(_id + "-" + prevSkillLevel, _id + "-" + _level);
					}
					else
					{
						if (!string.IsNullOrEmpty(list))
						{
							list += ";";
						}
						list += _id + "-" + _level;
					}

					vars.Set("SubSkillList", list);
					giveSkill(player, trainer, skill, false);
				}
				break;
			}
			case AcquireSkillType.DUALCLASS:
			{
				if (player.isSubClassActive())
				{
					player.sendPacket(SystemMessageId.THIS_SKILL_CANNOT_BE_LEARNED_WHILE_IN_THE_SUBCLASS_STATE_PLEASE_TRY_AGAIN_AFTER_CHANGING_TO_THE_MAIN_CLASS);
					Util.handleIllegalPlayerAction(player, player + " is requesting skill Id: " + _id + " level " + _level + " while Sub-Class is active!", IllegalActionPunishmentType.NONE);
					return ValueTask.CompletedTask;
				}

				if (checkPlayerSkill(player, trainer, s, _level))
				{
					PlayerVariables vars = player.getVariables();
					string list = vars.Get("DualSkillList", string.Empty);
					if (prevSkillLevel > 0 && list.contains(_id + "-" + prevSkillLevel))
					{
						list = list.Replace(_id + "-" + prevSkillLevel, _id + "-" + _level);
					}
					else
					{
						if (!string.IsNullOrEmpty(list))
						{
							list += ";";
						}
						list += _id + "-" + _level;
					}
					vars.Set("DualSkillList", list);
					giveSkill(player, trainer, skill, false);
				}
				break;
			}
			case AcquireSkillType.COLLECT:
			{
				if (checkPlayerSkill(player, trainer, s, _level))
				{
					giveSkill(player, trainer, skill);
				}
				break;
			}
			case AcquireSkillType.ALCHEMY:
			{
				if (player.getRace() != Race.ERTHEIA)
					return ValueTask.CompletedTask;

				if (checkPlayerSkill(player, trainer, s, _level))
				{
					giveSkill(player, trainer, skill);
					player.sendPacket(new AcquireSkillDonePacket());
					player.sendPacket(new ExAlchemySkillListPacket(player));

					List<SkillLearn> alchemySkills = SkillTreeData.getInstance().getAvailableAlchemySkills(player);
					if (alchemySkills.Count == 0)
					{
						player.sendPacket(SystemMessageId.THERE_ARE_NO_OTHER_SKILLS_TO_LEARN);
					}
					else
					{
						player.sendPacket(new ExAcquirableSkillListByClassPacket(alchemySkills, AcquireSkillType.ALCHEMY));
					}
				}
				break;
			}
			case AcquireSkillType.REVELATION:
			{
				/*
				 * if (player.isSubClassActive()) { player.sendPacket(SystemMessageId.THIS_SKILL_CANNOT_BE_LEARNED_WHILE_IN_THE_SUBCLASS_STATE_PLEASE_TRY_AGAIN_AFTER_CHANGING_TO_THE_MAIN_CLASS); Util.handleIllegalPlayerAction(player, "Player " + player.getName() + " is requesting skill Id: " + _id +
				 * " level " + skillLevel + " while Sub-Class is active!", IllegalActionPunishmentType.NONE); return; } if ((player.getLevel() < 85) || !player.isInCategory(CategoryType.SIXTH_CLASS_GROUP)) { player.sendPacket(SystemMessageId.YOU_DO_NOT_HAVE_ENOUGH_ITEMS_TO_LEARN_THIS_SKILL);
				 * Util.handleIllegalPlayerAction(player, "Player " + player.getName() + " is requesting skill Id: " + _id + " level " + skillLevel + " while not being level 85 or awaken!", IllegalActionPunishmentType.NONE); return; } int count = 0; for (String varName : REVELATION_VAR_NAMES) { if
				 * (player.getVariables().getInt(varName, 0) > 0) { count++; } } if (count >= 2) { player.sendPacket(SystemMessageId.YOU_DO_NOT_HAVE_ENOUGH_ITEMS_TO_LEARN_THIS_SKILL); Util.handleIllegalPlayerAction(player, "Player " + player.getName() + " is requesting skill Id: " + _id + " level "
				 * + _level + " while having already learned 2 skills!", IllegalActionPunishmentType.NONE); return; } if (checkPlayerSkill(player, trainer, s)) { String varName = count == 0 ? REVELATION_VAR_NAMES[0] : REVELATION_VAR_NAMES[1]; player.getVariables().set(varName, skill.getId());
				 * giveSkill(player, trainer, skill); ThreadPool.schedule(() -> { player.getStat().recalculateStats(false); player.broadcastUserInfo(); }, 100); } List<SkillLearn> skills = SkillTreeData.getInstance().getAvailableRevelationSkills(player, SubclassType.BASECLASS); if
				 * (!skills.isEmpty()) { player.sendPacket(new ExAcquirableSkillListByClass(skills, AcquireSkillType.REVELATION)); } else { player.sendPacket(SystemMessageId.THERE_ARE_NO_OTHER_SKILLS_TO_LEARN); }
				 */
				break;
			}
			case AcquireSkillType.REVELATION_DUALCLASS:
			{
				/*
				 * if (player.isSubClassActive() && !player.isDualClassActive()) { player.sendPacket(SystemMessageId.THIS_SKILL_CANNOT_BE_LEARNED_WHILE_IN_THE_SUBCLASS_STATE_PLEASE_TRY_AGAIN_AFTER_CHANGING_TO_THE_MAIN_CLASS); Util.handleIllegalPlayerAction(player, "Player " + player.getName() +
				 * " is requesting skill Id: " + _id + " level " + skillLevel + " while Sub-Class is active!", IllegalActionPunishmentType.NONE); return; } if ((player.getLevel() < 85) || !player.isInCategory(CategoryType.SIXTH_CLASS_GROUP)) {
				 * player.sendPacket(SystemMessageId.YOU_DO_NOT_HAVE_ENOUGH_ITEMS_TO_LEARN_THIS_SKILL); Util.handleIllegalPlayerAction(player, "Player " + player.getName() + " is requesting skill Id: " + _id + " level " + skillLevel + " while not being level 85 or awaken!",
				 * IllegalActionPunishmentType.NONE); return; } int count = 0; for (String varName : DUALCLASS_REVELATION_VAR_NAMES) { if (player.getVariables().getInt(varName, 0) > 0) { count++; } } if (count >= 2) {
				 * player.sendPacket(SystemMessageId.YOU_DO_NOT_HAVE_ENOUGH_ITEMS_TO_LEARN_THIS_SKILL); Util.handleIllegalPlayerAction(player, "Player " + player.getName() + " is requesting skill Id: " + _id + " level " + skillLevel + " while having already learned 2 skills!",
				 * IllegalActionPunishmentType.NONE); return; } if (checkPlayerSkill(player, trainer, s)) { String varName = count == 0 ? DUALCLASS_REVELATION_VAR_NAMES[0] : DUALCLASS_REVELATION_VAR_NAMES[1]; player.getVariables().set(varName, skill.getId()); giveSkill(player, trainer, skill);
				 * ThreadPool.schedule(() -> { player.getStat().recalculateStats(false); player.broadcastUserInfo(); }, 100); } List<SkillLearn> skills = SkillTreeData.getInstance().getAvailableRevelationSkills(player, SubclassType.DUALCLASS); if (!skills.isEmpty()) { player.sendPacket(new
				 * ExAcquirableSkillListByClass(skills, AcquireSkillType.REVELATION_DUALCLASS)); } else { player.sendPacket(SystemMessageId.THERE_ARE_NO_OTHER_SKILLS_TO_LEARN); }
				 */
				break;
			}
			default:
			{
				PacketLogger.Instance.Warn("Recived Wrong Packet Data in Aquired Skill, unknown skill type:" + _skillType);
				break;
			}
		}

		return ValueTask.CompletedTask;
	}

	public static void showSubUnitSkillList(Player player)
    {
        Clan? clan = player.getClan();
        if (clan is null)
        {
            player.sendPacket(SystemMessageId.THERE_ARE_NO_OTHER_SKILLS_TO_LEARN);
            return;
        }

		List<SkillLearn> skills = SkillTreeData.getInstance().getAvailableSubPledgeSkills(clan);
		if (skills.Count == 0)
		{
			player.sendPacket(SystemMessageId.THERE_ARE_NO_OTHER_SKILLS_TO_LEARN);
		}
		else
		{
			player.sendPacket(new ExAcquirableSkillListByClassPacket(skills, AcquireSkillType.SUBPLEDGE));
		}
	}

	public static void showSubSkillList(Player player)
	{
		List<SkillLearn> skills = SkillTreeData.getInstance().getAvailableSubClassSkills(player);
		if (skills.Count != 0)
		{
			player.sendPacket(new ExAcquirableSkillListByClassPacket(skills, AcquireSkillType.SUBCLASS));
		}
		else
		{
			player.sendPacket(SystemMessageId.THERE_ARE_NO_OTHER_SKILLS_TO_LEARN);
		}
	}

	public static void showDualSkillList(Player player)
	{
		List<SkillLearn> skills = SkillTreeData.getInstance().getAvailableDualClassSkills(player);
		if (skills.Count != 0)
		{
			player.sendPacket(new ExAcquirableSkillListByClassPacket(skills, AcquireSkillType.DUALCLASS));
		}
		else
		{
			player.sendPacket(SystemMessageId.THERE_ARE_NO_OTHER_SKILLS_TO_LEARN);
		}
	}

	/**
	 * Perform a simple check for current player and skill.<br>
	 * Takes the needed SP if the skill require it and all requirements are meet.<br>
	 * Consume required items if the skill require it and all requirements are meet.
	 * @param player the skill learning player.
	 * @param trainer the skills teaching Npc.
	 * @param skillLearn the skill to be learn.
	 * @return {@code true} if all requirements are meet, {@code false} otherwise.
	 */
	private bool checkPlayerSkill(Player player, Npc? trainer, SkillLearn skillLearn, int skillLevel)
	{
		if (skillLearn != null && skillLearn.getSkillLevel() == skillLevel)
		{
			// Hack check.
			if (skillLearn.getGetLevel() > player.getLevel())
			{
				player.sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_SKILL_LEVEL_REQUIREMENTS);
				Util.handleIllegalPlayerAction(player, player + ", level " + player.getLevel() + " is requesting skill Id: " + _id + " level " + skillLevel + " without having minimum required level, " + skillLearn.getGetLevel() + "!", IllegalActionPunishmentType.NONE);
				return false;
			}

			if (skillLearn.getDualClassLevel() > 0)
			{
				SubClassHolder? playerDualClass = player.getDualClass();
				if (playerDualClass == null || playerDualClass.getLevel() < skillLearn.getDualClassLevel())
				{
					return false;
				}
			}

			// First it checks that the skill require SP and the player has enough SP to learn it.
			long levelUpSp = skillLearn.getLevelUpSp();
			if (levelUpSp > 0 && levelUpSp > player.getSp())
			{
				player.sendPacket(new ExAcquireSkillResultPacket(skillLearn.getSkillId(), skillLearn.getSkillLevel(), false, SystemMessageId.YOU_DO_NOT_HAVE_ENOUGH_SP_TO_LEARN_THIS_SKILL));
				showSkillList(trainer, player);
				return false;
			}

			if (!Config.Character.DIVINE_SP_BOOK_NEEDED && _id == (int)CommonSkill.DIVINE_INSPIRATION)
			{
				return true;
			}

			// Check for required skills.
			if (!skillLearn.getPreReqSkills().isEmpty())
			{
				foreach (SkillHolder skill in skillLearn.getPreReqSkills())
				{
					if (player.getSkillLevel(skill.getSkillId()) < skill.getSkillLevel())
					{
						if (skill.getSkillId() == (int)CommonSkill.ONYX_BEAST_TRANSFORMATION)
						{
							player.sendPacket(new ExAcquireSkillResultPacket(skillLearn.getSkillId(),
								skillLearn.getSkillLevel(), false,
								SystemMessageId
									.YOU_MUST_LEARN_THE_ONYX_BEAST_SKILL_BEFORE_YOU_CAN_LEARN_FURTHER_SKILLS));
						}
						else
						{
							player.sendPacket(new ExAcquireSkillResultPacket(skillLearn.getSkillId(),
								skillLearn.getSkillLevel(), false,
								SystemMessageId.NOT_ENOUGH_ITEMS_TO_LEARN_THE_SKILL));
						}
						return false;
					}
				}
			}

			// Check for required items.
			if (skillLearn.getRequiredItems().Count != 0)
			{
				// Then checks that the player has all the items
				int count = 0;
				long playerItemCount = 0;
				foreach (List<ItemHolder> items in skillLearn.getRequiredItems())
				{
					count = 0;
					foreach (ItemHolder item in items)
					{
						count++;
						playerItemCount = player.getInventory().getInventoryItemCount(item.getId(), -1);
						if (playerItemCount >= item.getCount())
						{
							break;
						}

						if (count == items.Count)
						{
							player.sendPacket(new ExAcquireSkillResultPacket(skillLearn.getSkillId(),
								skillLearn.getSkillLevel(), false,
								SystemMessageId.NOT_ENOUGH_ITEMS_TO_LEARN_THE_SKILL));

							showSkillList(trainer, player);
							return false;
						}
					}
				}

				// If the player has all required items, they are consumed.
				foreach (List<ItemHolder> items in skillLearn.getRequiredItems())
				{
					count = 0;
					foreach (ItemHolder item in items)
					{
						count++;
						playerItemCount = player.getInventory().getInventoryItemCount(item.getId(), -1);
						if (playerItemCount >= item.getCount() && player.destroyItemByItemId("SkillLearn", item.getId(), item.getCount(), trainer, true))
						{
							break;
						}

						if (count == items.Count)
						{
							Util.handleIllegalPlayerAction(player, "Somehow " + player + ", level " + player.getLevel() + " lose required item Id: " + item.getId() + " to learn skill while learning skill Id: " + _id + " level " + skillLevel + "!", IllegalActionPunishmentType.NONE);
						}
					}
				}
			}

			if (!skillLearn.getRemoveSkills().isEmpty())
			{
				skillLearn.getRemoveSkills().ForEach(skillId =>
				{
					Skill? skillToRemove = player.getKnownSkill(skillId);
					if (skillToRemove != null)
					{
						player.removeSkill(skillToRemove, true);
					}
				});
			}

			// If the player has SP and all required items then consume SP.
			if (levelUpSp > 0)
			{
				player.setSp(player.getSp() - levelUpSp);

                if (!player.isSubclassLocked())
                {
                    UserInfoPacket ui = new UserInfoPacket(player);
                    ui.AddComponentType(UserInfoType.CURRENT_HPMPCP_EXP_SP);
                    player.sendPacket(ui);
                }
            }
			return true;
		}
		return false;
	}

	/**
	 * Add the skill to the player and makes proper updates.
	 * @param player the player acquiring a skill.
	 * @param trainer the Npc teaching a skill.
	 * @param skill the skill to be learn.
	 */
	private void giveSkill(Player player, Npc? trainer, Skill skill)
	{
		giveSkill(player, trainer, skill, true);
	}

	/**
	 * Add the skill to the player and makes proper updates.
	 * @param player the player acquiring a skill.
	 * @param trainer the Npc teaching a skill.
	 * @param skill the skill to be learn.
	 * @param store
	 */
	private void giveSkill(Player player, Npc? trainer, Skill skill, bool store)
	{
		player.addSkill(skill, store);
		player.sendItemList();
		player.updateShortCuts(_id, skill.getLevel(), skill.getSubLevel());
		player.sendPacket(new ShortCutInitPacket(player));
		player.sendPacket(ExBasicActionListPacket.STATIC_PACKET);
		player.sendPacket(new ExAcquireSkillResultPacket(skill.getId(), skill.getLevel(), true, SystemMessageId.YOU_HAVE_LEARNED_THE_SKILL_S1_2));
		player.sendSkillList(skill.getId());

		// If skill is expand type then sends packet:
		if (_id >= 1368 && _id <= 1372)
		{
			player.sendStorageMaxCount();
		}

		// Notify scripts of the skill learn.
		if (player.Events.HasSubscribers<OnPlayerSkillLearn>())
		{
			player.Events.NotifyAsync(new OnPlayerSkillLearn(trainer, player, skill, _skillType));
		}

		player.restoreAutoShortcutVisual();
	}

	/**
	 * Wrapper for returning the skill list to the player after it's done with current skill.
	 * @param trainer the Npc which the {@code player} is interacting
	 * @param player the active character
	 */
	private void showSkillList(Npc? trainer, Player player)
	{
		if (_skillType == AcquireSkillType.SUBCLASS)
		{
			showSubSkillList(player);
		}
		else if (_skillType == AcquireSkillType.DUALCLASS)
		{
			showDualSkillList(player);
		}
		else if (trainer is Fisherman)
		{
			Fisherman.showFishSkillList(player);
		}
	}

	/**
	 * Verify if the player can transform.
	 * @param player the player to verify
	 * @return {@code true} if the player meets the required conditions to learn a transformation, {@code false} otherwise
	 */
	public static bool canTransform(Player player)
	{
		if (Config.Character.ALLOW_TRANSFORM_WITHOUT_QUEST)
		{
			return true;
		}
		QuestState? qs = player.getQuestState("Q00136_MoreThanMeetsTheEye");
		return qs != null && qs.isCompleted();
	}
}
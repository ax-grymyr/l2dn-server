using System.Collections.Frozen;
using System.Text;
using L2Dn.Extensions;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Quests;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class VillageMaster: Folk
{
	private new static readonly Logger LOGGER = LogManager.GetLogger(nameof(VillageMaster));

	private static readonly Set<CharacterClass> neverSubclassed;
	private static readonly Set<CharacterClass> subclasseSet1;
	private static readonly Set<CharacterClass> subclasseSet2;
	private static readonly Set<CharacterClass> subclasseSet3;
	private static readonly Set<CharacterClass> subclasseSet4;
	private static readonly Set<CharacterClass> subclasseSet5;
	private static readonly Set<CharacterClass> mainSubclassSet;
	private static readonly Map<CharacterClass, Set<CharacterClass>> subclassSetMap = new();

	static VillageMaster()
	{
		neverSubclassed = [];
		neverSubclassed.addAll([CharacterClass.OVERLORD, CharacterClass.WARSMITH]);

		subclasseSet1 = [];
		subclasseSet1.addAll([CharacterClass.DARK_AVENGER, CharacterClass.PALADIN, CharacterClass.TEMPLE_KNIGHT, CharacterClass.SHILLIEN_KNIGHT]);

		subclasseSet2 = [];
		subclasseSet2.addAll([CharacterClass.TREASURE_HUNTER, CharacterClass.ABYSS_WALKER, CharacterClass.PLAINS_WALKER]);

		subclasseSet3 = [];
		subclasseSet3.addAll([CharacterClass.HAWKEYE, CharacterClass.SILVER_RANGER, CharacterClass.PHANTOM_RANGER]);

		subclasseSet4 = [];
		subclasseSet4.addAll([CharacterClass.WARLOCK, CharacterClass.ELEMENTAL_SUMMONER, CharacterClass.PHANTOM_SUMMONER]);

		subclasseSet5 = [];
		subclasseSet5.addAll([CharacterClass.SORCERER, CharacterClass.SPELLSINGER, CharacterClass.SPELLHOWLER]);

		Set<CharacterClass> subclasses = [];
        FrozenSet<int>? thirdClassGroup = CategoryData.getInstance().getCategoryByType(CategoryType.THIRD_CLASS_GROUP);
        if (thirdClassGroup != null)
		    subclasses.addAll(thirdClassGroup.Select(x => (CharacterClass)x));

		subclasses.removeAll(neverSubclassed);
		mainSubclassSet = subclasses;

		subclassSetMap.put(CharacterClass.DARK_AVENGER, subclasseSet1);
		subclassSetMap.put(CharacterClass.PALADIN, subclasseSet1);
		subclassSetMap.put(CharacterClass.TEMPLE_KNIGHT, subclasseSet1);
		subclassSetMap.put(CharacterClass.SHILLIEN_KNIGHT, subclasseSet1);
		subclassSetMap.put(CharacterClass.TREASURE_HUNTER, subclasseSet2);
		subclassSetMap.put(CharacterClass.ABYSS_WALKER, subclasseSet2);
		subclassSetMap.put(CharacterClass.PLAINS_WALKER, subclasseSet2);
		subclassSetMap.put(CharacterClass.HAWKEYE, subclasseSet3);
		subclassSetMap.put(CharacterClass.SILVER_RANGER, subclasseSet3);
		subclassSetMap.put(CharacterClass.PHANTOM_RANGER, subclasseSet3);
		subclassSetMap.put(CharacterClass.WARLOCK, subclasseSet4);
		subclassSetMap.put(CharacterClass.ELEMENTAL_SUMMONER, subclasseSet4);
		subclassSetMap.put(CharacterClass.PHANTOM_SUMMONER, subclasseSet4);
		subclassSetMap.put(CharacterClass.SORCERER, subclasseSet5);
		subclassSetMap.put(CharacterClass.SPELLSINGER, subclasseSet5);
		subclassSetMap.put(CharacterClass.SPELLHOWLER, subclasseSet5);
	}

	/**
	 * Creates a village master.
	 * @param template the village master NPC template
	 */
	public VillageMaster(NpcTemplate template): base(template)
	{
		InstanceType = InstanceType.VillageMaster;
	}

	public override bool isAutoAttackable(Creature attacker)
	{
		if (attacker.isMonster())
		{
			return true;
		}
		return base.isAutoAttackable(attacker);
	}

	public override string getHtmlPath(int npcId, int value, Player? player)
	{
		string pom;
		if (value == 0)
		{
			pom = npcId.ToString();
		}
		else
		{
			pom = npcId + "-" + value;
		}
		return "html/villagemaster/" + pom + ".htm";
	}

	public override void onBypassFeedback(Player player, string command)
	{
		string[] commandStr = command.Split(" ");
		string actualCommand = commandStr[0]; // Get actual command
		string cmdParams = "";
		string cmdParams2 = "";
		if (commandStr.Length >= 2)
		{
			cmdParams = commandStr[1];
		}
		if (commandStr.Length >= 3)
		{
			cmdParams2 = commandStr[2];
		}

		if (actualCommand.equalsIgnoreCase("create_clan"))
		{
			if (string.IsNullOrEmpty(cmdParams))
			{
				return;
			}

			if (!string.IsNullOrEmpty(cmdParams2) || !isValidName(cmdParams))
			{
				player.sendPacket(SystemMessageId.CLAN_NAME_IS_INVALID);
				return;
			}

			ClanTable.getInstance().createClan(player, cmdParams);
		}
		else if (actualCommand.equalsIgnoreCase("create_academy"))
		{
			if (string.IsNullOrEmpty(cmdParams))
			{
				return;
			}

			createSubPledge(player, cmdParams, string.Empty, Clan.SUBUNIT_ACADEMY, 5);
		}
		else if (actualCommand.equalsIgnoreCase("rename_pledge"))
		{
			if (string.IsNullOrEmpty(cmdParams) || string.IsNullOrEmpty(cmdParams2))
			{
				return;
			}

			renameSubPledge(player, int.Parse(cmdParams), cmdParams2);
		}
		else if (actualCommand.equalsIgnoreCase("create_royal"))
		{
			if (string.IsNullOrEmpty(cmdParams))
			{
				return;
			}

			createSubPledge(player, cmdParams, cmdParams2, Clan.SUBUNIT_ROYAL1, 6);
		}
		else if (actualCommand.equalsIgnoreCase("create_knight"))
		{
			if (string.IsNullOrEmpty(cmdParams))
			{
				return;
			}

			createSubPledge(player, cmdParams, cmdParams2, Clan.SUBUNIT_KNIGHT1, 7);
		}
		else if (actualCommand.equalsIgnoreCase("assign_subpl_leader"))
		{
			if (string.IsNullOrEmpty(cmdParams))
			{
				return;
			}

			assignSubPledgeLeader(player, cmdParams, cmdParams2);
		}
		else if (actualCommand.equalsIgnoreCase("create_ally"))
		{
			if (string.IsNullOrEmpty(cmdParams))
			{
				return;
			}

            Clan? clan = player.getClan();
			if (clan == null)
			{
				player.sendPacket(SystemMessageId.ONLY_CLAN_LEADERS_MAY_CREATE_ALLIANCES);
			}
			else
			{
                clan.createAlly(player, cmdParams);
			}
		}
		else if (actualCommand.equalsIgnoreCase("dissolve_ally"))
		{
			player.getClan()?.dissolveAlly(player);
		}
		else if (actualCommand.equalsIgnoreCase("dissolve_clan"))
		{
			dissolveClan(player, player.getClanId());
		}
		else if (actualCommand.equalsIgnoreCase("change_clan_leader"))
		{
			if (string.IsNullOrEmpty(cmdParams))
			{
				return;
			}

            Clan? clan = player.getClan();
			if (!player.isClanLeader() || clan == null)
			{
				player.sendPacket(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
				return;
			}

			if (player.getName().equalsIgnoreCase(cmdParams))
			{
				return;
			}

			ClanMember? member = clan.getClanMember(cmdParams);
			if (member == null)
			{
				SystemMessagePacket sm = new(SystemMessageId.S1_DOES_NOT_EXIST);
				sm.Params.addString(cmdParams);
				player.sendPacket(sm);
				return;
			}

            Player? memberPlayer = member.getPlayer();
			if (!member.isOnline() || memberPlayer == null)
			{
				player.sendPacket(SystemMessageId.THAT_PLAYER_IS_NOT_CURRENTLY_ONLINE);
				return;
			}

			// To avoid clans with null clan leader, academy members shouldn't be eligible for clan leader.
			if (memberPlayer.isAcademyMember())
			{
				player.sendPacket(SystemMessageId.THAT_PRIVILEGE_CANNOT_BE_GRANTED_TO_A_CLAN_ACADEMY_MEMBER);
				return;
			}

			if (Config.ALT_CLAN_LEADER_INSTANT_ACTIVATION)
			{
				clan.setNewLeader(member);
			}
			else
			{
				string filePath;
				if (clan.getNewLeaderId() == 0)
				{
					clan.setNewLeaderId(member.getObjectId(), true);
					filePath = "scripts/village_master/ClanMaster/9000-07-success.htm";
				}
				else
				{
					filePath = "scripts/village_master/ClanMaster/9000-07-in-progress.htm";
				}

				HtmlContent htmlContent = HtmlContent.LoadFromFile(filePath, player);
				NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(ObjectId, 0, htmlContent);
				player.sendPacket(msg);
			}
		}
		else if (actualCommand.equalsIgnoreCase("cancel_clan_leader_change"))
		{
            Clan? clan = player.getClan();
			if (!player.isClanLeader() || clan == null)
			{
				player.sendPacket(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
				return;
			}

			HtmlContent htmlContent;
			if (clan.getNewLeaderId() != 0)
			{
				clan.setNewLeaderId(0, true);
				htmlContent = HtmlContent.LoadFromFile("scripts/village_master/ClanMaster/9000-07-canceled.htm", player);
			}
			else
			{
				htmlContent = HtmlContent.LoadFromText(
					"<html><body>You don't have clan leader delegation applications submitted yet!</body></html>",
					player);
			}

			NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(ObjectId, 0, htmlContent);
			player.sendPacket(msg);
		}
		else if (actualCommand.equalsIgnoreCase("recover_clan"))
		{
			recoverClan(player, player.getClanId());
		}
		else if (actualCommand.equalsIgnoreCase("increase_clan_level"))
        {
            Clan? clan = player.getClan();
			if (clan != null && clan.levelUpClan(player))
			{
				player.broadcastPacket(new MagicSkillUsePacket(player, 5103, 1, TimeSpan.Zero, TimeSpan.Zero));
				player.broadcastPacket(new MagicSkillLaunchedPacket(player, 5103, 1));
			}
		}
		else if (actualCommand.equalsIgnoreCase("learn_clan_skills"))
		{
			showPledgeSkillList(player);
		}
		else if (command.startsWith("Subclass"))
		{
			// Subclasses may not be changed while a skill is in use.
			if (player.isCastingNow() || player.isAllSkillsDisabled())
			{
				player.sendPacket(SystemMessageId.SUBCLASSES_MAY_NOT_BE_CREATED_OR_CHANGED_WHILE_A_SKILL_IS_IN_USE);
				return;
			}

			// Subclasses may not be changed while a transformated state.
			if (player.isTransformed())
			{
				HtmlContent htmlText1 = HtmlContent.LoadFromFile("html/villagemaster/SubClass_NoTransformed.htm", player);
				NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(ObjectId, 0, htmlText1);
				player.sendPacket(msg);
				return;
			}
			// Subclasses may not be changed while a summon is active.
			if (player.hasSummon())
			{
				HtmlContent htmlText1 = HtmlContent.LoadFromFile("html/villagemaster/SubClass_NoSummon.htm", player);
				NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(ObjectId, 0, htmlText1);
				player.sendPacket(msg);
				return;
			}
			// Subclasses may not be changed while you have exceeded your inventory limit.
			if (!player.isInventoryUnder90(true))
			{
				player.sendPacket(SystemMessageId.YOU_CANNOT_CREATE_OR_CHANGE_A_SUBCLASS_WHILE_YOU_HAVE_NO_FREE_SPACE_IN_YOUR_INVENTORY);
				return;
			}
			// Subclasses may not be changed while a you are over your weight limit.
			if (player.getWeightPenalty() >= 2)
			{
				player.sendPacket(SystemMessageId.YOU_CANNOT_CREATE_OR_CHANGE_A_DUAL_CLASS_WHILE_YOU_HAVE_OVERWEIGHT);
				return;
			}

			int cmdChoice = 0;
			int paramOne = 0;
			int paramTwo = 0;
			try
			{
				cmdChoice = int.Parse(command.Substring(9, 10).Trim());
				int endIndex = command.IndexOf(' ', 11);
				if (endIndex == -1)
				{
					endIndex = command.Length;
				}

				if (command.Length > 11)
				{
					paramOne = int.Parse(command.Substring(11, endIndex).Trim());
					if (command.Length > endIndex)
					{
						paramTwo = int.Parse(command.Substring(endIndex).Trim());
					}
				}
			}
			catch (Exception nfe)
			{
				LOGGER.Warn(nameof(VillageMaster) + ": Wrong numeric values for command " + command + ": " + nfe);
			}

			Set<CharacterClass>? subsAvailable;
			HtmlContent? htmlText = null;
			NpcHtmlMessagePacket html;
			switch (cmdChoice)
			{
				case 0: // Subclass change menu
					htmlText = HtmlContent.LoadFromFile(getSubClassMenu(player.getRace()), player);
					break;
				case 1: // Add Subclass - Initial
					// Avoid giving player an option to add a new sub class, if they have max sub-classes already.
					if (player.getTotalSubClasses() >= Config.MAX_SUBCLASS)
					{
						htmlText = HtmlContent.LoadFromFile(getSubClassFail(), player);
						break;
					}

					subsAvailable = getAvailableSubClasses(player);
					if (subsAvailable != null && !subsAvailable.isEmpty())
					{
						htmlText = HtmlContent.LoadFromFile("html/villagemaster/SubClass_Add.htm", player);
						StringBuilder content1 = new StringBuilder(200);
						foreach (CharacterClass subClass in subsAvailable)
						{
							content1.Append("<a action=\"bypass npc_%objectId%_Subclass 4 " + (int)subClass + "\" msg=\"1268;" + ClassListData.getInstance().getClass(subClass).getClassName() + "\">" + ClassListData.getInstance().getClass(subClass).getClientCode() + "</a><br>");
						}

						htmlText.Replace("%list%", content1.ToString());
					}
					else
					{
						if (player.getRace() == Race.ELF || player.getRace() == Race.DARK_ELF)
						{
							HtmlContent htmlText1 = HtmlContent.LoadFromFile("html/villagemaster/SubClass_Fail_Elves.htm", player);
							html = new NpcHtmlMessagePacket(ObjectId, 0, htmlText1);
							player.sendPacket(html);
						}
						else if (player.getRace() == Race.KAMAEL)
						{
							HtmlContent htmlText1 = HtmlContent.LoadFromFile("html/villagemaster/SubClass_Fail_Kamael.htm", player);
							html = new NpcHtmlMessagePacket(ObjectId, 0, htmlText1);
							player.sendPacket(html);
						}
						else
						{
							// TODO: Retail message
							player.sendMessage("There are no sub classes available at this time.");
						}
						return;
					}
					break;
				case 2: // Change Class - Initial
					if (player.getSubClasses().Count == 0)
					{
						htmlText = HtmlContent.LoadFromFile("html/villagemaster/SubClass_ChangeNo.htm", player);
					}
					else
					{
						StringBuilder content2 = new StringBuilder(200);
						if (checkVillageMaster(player.getBaseClass()))
						{
							content2.Append("<a action=\"bypass -h npc_%objectId%_Subclass 5 0\">" + ClassListData.getInstance().getClass(player.getBaseClass()).getClientCode() + "</a><br>");
						}

						foreach (SubClassHolder holder in player.getSubClasses().Values)
						{
							if (checkVillageMaster(holder.getClassDefinition()))
							{
								content2.Append("<a action=\"bypass -h npc_%objectId%_Subclass 5 " +
								                holder.getClassIndex() + "\">" +
								                ClassListData.getInstance().getClass(holder.getClassDefinition())
									                .getClientCode() + "</a><br>");
							}
						}

						if (content2.Length > 0)
						{
							htmlText = HtmlContent.LoadFromFile("html/villagemaster/SubClass_Change.htm", player);
							htmlText.Replace("%list%", content2.ToString());
						}
						else
						{
							htmlText = HtmlContent.LoadFromFile("html/villagemaster/SubClass_ChangeNotFound.htm", player);
						}
					}
					break;
				case 3: // Change/Cancel Subclass - Initial
					if (player.getSubClasses() == null || player.getSubClasses().Count == 0)
					{
						htmlText = HtmlContent.LoadFromFile("html/villagemaster/SubClass_ModifyEmpty.htm", player);
						break;
					}

					// custom value
					if (player.getTotalSubClasses() > 3)
					{
						htmlText = HtmlContent.LoadFromFile("html/villagemaster/SubClass_ModifyCustom.htm", player);
						StringBuilder content3 = new StringBuilder(200);
						int classIndex = 1;
						foreach (SubClassHolder holder in player.getSubClasses().Values)
						{
							content3.Append("Sub-class " + classIndex++ +
							                "<br><a action=\"bypass -h npc_%objectId%_Subclass 6 " +
							                holder.getClassIndex() + "\">" +
							                ClassListData.getInstance().getClass(holder.getClassDefinition())
								                .getClientCode() + "</a><br>");
						}
						htmlText.Replace("%list%", content3.ToString());
					}
					else
					{
						// retail html contain only 3 subclasses
						htmlText = HtmlContent.LoadFromFile("html/villagemaster/SubClass_Modify.htm", player);
						if (player.getSubClasses().ContainsKey(1))
						{
							htmlText.Replace("%sub1%", ClassListData.getInstance().getClass(player.getSubClasses().get(1)!.getClassDefinition()).getClientCode());
						}
						else
						{
							htmlText.Replace("<Button ALIGN=LEFT ICON=\"NORMAL\" action=\"bypass npc_%objectId%_Subclass 6 1\">%sub1%</Button>", "");
						}

						if (player.getSubClasses().ContainsKey(2))
						{
							htmlText.Replace("%sub2%", ClassListData.getInstance().getClass(player.getSubClasses().get(2)!.getClassDefinition()).getClientCode());
						}
						else
						{
							htmlText.Replace("<Button ALIGN=LEFT ICON=\"NORMAL\" action=\"bypass npc_%objectId%_Subclass 6 2\">%sub2%</Button>", "");
						}

						if (player.getSubClasses().ContainsKey(3))
						{
							htmlText.Replace("%sub3%", ClassListData.getInstance().getClass(player.getSubClasses().get(3)!.getClassDefinition()).getClientCode());
						}
						else
						{
							htmlText.Replace("<Button ALIGN=LEFT ICON=\"NORMAL\" action=\"bypass npc_%objectId%_Subclass 6 3\">%sub3%</Button>", "");
						}
					}
					break;
				case 4: // Add Subclass - Action (Subclass 4 x[x])
					/**
					 * If the character is less than level 75 on any of their previously chosen classes then disallow them to change to their most recently added sub-class choice.
					 */
					// TODO: flood protectors
					// if (!player.getClient().getFloodProtectors().canChangeSubclass())
					// {
					// 	LOGGER.Warn(nameof(VillageMaster) + ": " + player + " has performed a subclass change too fast");
					// 	return;
					// }

					bool allowAddition = true;
					if (player.getTotalSubClasses() >= Config.MAX_SUBCLASS)
					{
						allowAddition = false;
					}

					if (player.getLevel() < 75)
					{
						allowAddition = false;
					}

					if (allowAddition && player.getSubClasses().Count != 0)
					{
						foreach (SubClassHolder holder in player.getSubClasses().Values)
						{
							if (holder.getLevel() < 75)
							{
								allowAddition = false;
								break;
							}
						}
					}

					/**
					 * If quest checking is enabled, verify if the character has completed the Mimir's Elixir (Path to Subclass) and Fate's Whisper (A Grade Weapon) quests by checking for instances of their unique reward items. If they both exist, remove both unique items and continue with adding
					 * the sub-class.
					 */
					if (allowAddition && !Config.ALT_GAME_SUBCLASS_WITHOUT_QUESTS)
					{
						allowAddition = checkQuests(player);
					}

					if (allowAddition && isValidNewSubClass(player, (CharacterClass)paramOne))
					{
						if (!player.addSubClass((CharacterClass)paramOne, player.getTotalSubClasses() + 1, false))
						{
							return;
						}

						player.setActiveClass(player.getTotalSubClasses());

						htmlText = HtmlContent.LoadFromFile("html/villagemaster/SubClass_AddOk.htm", player);

						SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.YOU_HAVE_ACHIEVED_THE_SECOND_CLASS_S1_CONGRATS);
						msg.Params.addClassId(player.getClassId());
						player.sendPacket(msg);
					}
					else
					{
						htmlText = HtmlContent.LoadFromFile(getSubClassFail(), player);
					}
					break;
				case 5: // Change Class - Action
					/**
					 * If the character is less than level 75 on any of their previously chosen classes then disallow them to change to their most recently added sub-class choice. Note: paramOne = classIndex
					 */
					// TODO flood protectors
					// if (!player.getClient().getFloodProtectors().canChangeSubclass())
					// {
					// 	LOGGER.Warn(nameof(VillageMaster) + ": " + player + " has performed a subclass change too fast");
					// 	return;
					// }

					if (player.getClassIndex() == paramOne)
					{
						htmlText = HtmlContent.LoadFromFile("html/villagemaster/SubClass_Current.htm", player);
						break;
					}

					if (paramOne == 0)
					{
						if (!checkVillageMaster(player.getBaseClass()))
						{
							return;
						}
					}
					else
                    {
                        SubClassHolder? subClass = player.getSubClasses().get(paramOne);
                        if (subClass == null || !checkVillageMaster(subClass.getClassDefinition()))
                            return;
                    }

					player.setActiveClass(paramOne);
					// TODO: Retail message YOU_HAVE_SUCCESSFULLY_SWITCHED_S1_TO_S2
					player.sendMessage("You have successfully switched to your subclass.");
					return;
				case 6: // Change/Cancel Subclass - Choice
					// validity check
					if (paramOne < 1 || paramOne > Config.MAX_SUBCLASS)
					{
						return;
					}

					subsAvailable = getAvailableSubClasses(player);
					// another validity check
					if (subsAvailable == null || subsAvailable.isEmpty())
					{
						// TODO: Retail message
						player.sendMessage("There are no sub classes available at this time.");
						return;
					}

					StringBuilder content6 = new StringBuilder(200);
					foreach (CharacterClass subClass in subsAvailable)
					{
						content6.Append("<a action=\"bypass npc_%objectId%_Subclass 7 " + paramOne + " " +
						                (int)subClass + "\" msg=\"1445;\">" +
						                ClassListData.getInstance().getClass(subClass).getClientCode() + "</a><br>");
					}

					switch (paramOne)
					{
						case 1:
							htmlText = HtmlContent.LoadFromFile("html/villagemaster/SubClass_ModifyChoice1.htm", player);
							break;
						case 2:
							htmlText = HtmlContent.LoadFromFile("html/villagemaster/SubClass_ModifyChoice2.htm", player);
							break;
						case 3:
							htmlText = HtmlContent.LoadFromFile("html/villagemaster/SubClass_ModifyChoice3.htm", player);
							break;
						default:
							htmlText = HtmlContent.LoadFromFile("html/villagemaster/SubClass_ModifyChoice.htm", player);
							break;
					}

					htmlText.Replace("%list%", content6.ToString());
					break;
				case 7: // Change Subclass - Action
					/**
					 * Warning: the information about this subclass will be removed from the subclass list even if false!
					 */
					// TODO: flood protectors
					// if (!player.getClient().getFloodProtectors().canChangeSubclass())
					// {
					// 	LOGGER.Warn(nameof(VillageMaster) + ": " + player + " has performed a subclass change too fast");
					// 	return;
					// }

					if (!isValidNewSubClass(player, (CharacterClass)paramTwo))
					{
						return;
					}

					if (player.modifySubClass(paramOne, (CharacterClass)paramTwo, false))
					{
						player.abortCast();
						player.stopAllEffectsExceptThoseThatLastThroughDeath(); // all effects from old subclass stopped!
						player.stopAllEffects();
						player.stopCubics();
						player.setActiveClass(paramOne);

						htmlText = HtmlContent.LoadFromFile("html/villagemaster/SubClass_ModifyOk.htm", player);
						htmlText.Replace("%name%", ClassListData.getInstance().getClass((CharacterClass)paramTwo).getClientCode());

						SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.YOU_HAVE_ACHIEVED_THE_SECOND_CLASS_S1_CONGRATS);
						msg.Params.addClassId(player.getClassId());
						player.sendPacket(msg);
					}
					else
					{
						/**
						 * This isn't good! modifySubClass() removed subclass from memory we must update _classIndex! Else IndexOutOfBoundsException can turn up some place down the line along with other seemingly unrelated problems.
						 */
						player.setActiveClass(0); // Also updates _classIndex plus switching _classid to baseclass.
						player.sendMessage("The sub class could not be added, you have been reverted to your base class.");
						return;
					}
					break;
			}

			if (htmlText is not null)
			{
				htmlText.Replace("%objectId%", ObjectId.ToString());
				html = new NpcHtmlMessagePacket(ObjectId, 0, htmlText);
				player.sendPacket(html);
			}
		}
		else
		{
			base.onBypassFeedback(player, command);
		}
	}

	protected string getSubClassMenu(Race race)
	{
		if (Config.ALT_GAME_SUBCLASS_EVERYWHERE || race != Race.KAMAEL)
		{
			return "html/villagemaster/SubClass.htm";
		}

		return "html/villagemaster/SubClass_NoOther.htm";
	}

	protected string getSubClassFail()
	{
		return "html/villagemaster/SubClass_Fail.htm";
	}

	protected bool checkQuests(Player player)
	{
		// Noble players can add Sub-Classes without quests
		if (player.isNoble())
		{
			return true;
		}

		QuestState? qs = player.getQuestState("Q00234_FatesWhisper"); // TODO: Not added with Saviors.
		if (qs == null || !qs.isCompleted())
		{
			return false;
		}

		qs = player.getQuestState("Q00235_MimirsElixir"); // TODO: Not added with Saviors.
		if (qs == null || !qs.isCompleted())
		{
			return false;
		}

		return true;
	}

	/**
	 * Returns list of available subclasses Base class and already used subclasses removed
	 * @param player
	 * @return
	 */
	private Set<CharacterClass>? getAvailableSubClasses(Player player)
	{
		// get player base class
		CharacterClass currentBaseId = player.getBaseClass();
		CharacterClass baseCid = currentBaseId;

		// we need 2nd occupation ID
		CharacterClass baseClassId;
        CharacterClass? parentClass = baseCid.GetParentClass();
		if (baseCid.GetLevel() > 2 && parentClass != null)
		{
			baseClassId = parentClass.Value;
		}
		else
		{
			baseClassId = currentBaseId;
		}

		// If the race of your main class is Elf or Dark Elf, you may not select each class as a subclass
		// to the other class. If the race of your main class is Kamael, you may not subclass any other race.
		// If the race of your main class is NOT Kamael, you may not subclass any Kamael class. You may not
		// select Overlord and Warsmith class as a subclass. You may not select a similar class as the subclass.
		// The occupations classified as similar classes are as follows: Treasure Hunter, Plainswalker and
		// Abyss Walker Hawkeye, Silver Ranger and Phantom Ranger Paladin, Dark Avenger, Temple Knight
		// and Shillien Knight Warlocks, Elemental Summoner and Phantom Summoner Elder and Shillien Elder
		// Swordsinger and Bladedancer Sorcerer, Spellsinger and Spellhowler.
		// Also, Kamael have a special, hidden 4 subclass, the inspector, which can only be taken if you have
		// already completed the other two Kamael subclasses.
		Set<CharacterClass>? availSubs = getSubclasses(player, baseClassId);
		if (availSubs != null && !availSubs.isEmpty())
		{
			List<CharacterClass> copy = availSubs.ToList();
			foreach (CharacterClass classId in copy)
			{
				// check for the village master
				if (!checkVillageMaster(classId))
				{
					availSubs.remove(classId);
					continue;
				}

				// scan for already used subclasses
				foreach (SubClassHolder subList in player.getSubClasses().Values)
				{
					CharacterClass subClassId = subList.getClassDefinition();
					if (subClassId.EqualsOrChildOf(classId))
					{
						availSubs.remove(classId);
						break;
					}
				}
			}
		}

		return availSubs;
	}

	public Set<CharacterClass>? getSubclasses(Player player, CharacterClass classId)
	{
		Set<CharacterClass>? subclasses = null;
		CharacterClass pClass = classId;
		if (CategoryData.getInstance().isInCategory(CategoryType.THIRD_CLASS_GROUP, classId) || CategoryData.getInstance().isInCategory(CategoryType.FOURTH_CLASS_GROUP, classId))
		{
			subclasses = new();
			subclasses.addAll(mainSubclassSet);
			subclasses.remove(pClass);

			if (player.getRace() == Race.KAMAEL)
			{
				foreach (CharacterClass cid in EnumUtil.GetValues<CharacterClass>())
				{
					if (cid.GetRace() != Race.KAMAEL)
					{
						subclasses.remove(cid);
					}
				}

				// if (player.getAppearance().isFemale())
				// {
				// subclasses.remove(CharacterClass.MALE_SOULBREAKER);
				// }
				// else
				// {
				// subclasses.remove(CharacterClass.FEMALE_SOULBREAKER);
				// }
				//
				// if (!player.getSubClasses().containsKey(2) || (player.getSubClasses().get(2).getLevel() < 75))
				// {
				// subclasses.remove(CharacterClass.INSPECTOR);
				// }
			}
			else
			{
				if (player.getRace() == Race.ELF)
				{
					foreach (CharacterClass cid in EnumUtil.GetValues<CharacterClass>())
					{
						if (cid.GetRace() == Race.DARK_ELF)
						{
							subclasses.remove(cid);
						}
					}
				}
				else if (player.getRace() == Race.DARK_ELF)
				{
					foreach (CharacterClass cid in EnumUtil.GetValues<CharacterClass>())
					{
						if (cid.GetRace() == Race.ELF)
						{
							subclasses.remove(cid);
						}
					}
				}

				foreach (CharacterClass cid in EnumUtil.GetValues<CharacterClass>())
				{
					if (cid.GetRace() == Race.KAMAEL)
					{
						subclasses.remove(cid);
					}
				}
			}

			Set<CharacterClass>? unavailableClasses = subclassSetMap.get(pClass);
			if (unavailableClasses != null)
			{
				subclasses.removeAll(unavailableClasses);
			}
		}

		if (subclasses != null)
		{
			CharacterClass currClassId = player.getClassId();
			foreach (CharacterClass tempClass in subclasses)
			{
				if (currClassId.EqualsOrChildOf(tempClass))
				{
					subclasses.remove(tempClass);
				}
			}
		}

        return subclasses;
	}

	/**
	 * Check new subclass classId for validity (villagemaster race/type, is not contains in previous subclasses,
	 * is contains in allowed subclasses) Base class not added into allowed subclasses.
	 * @param player
	 * @param classId
	 * @return
	 */
	private bool isValidNewSubClass(Player player, CharacterClass classId)
	{
		if (!checkVillageMaster(classId))
		{
			return false;
		}

		foreach (SubClassHolder sub in player.getSubClasses().Values)
		{
			CharacterClass subClassId = sub.getClassDefinition();
			if (subClassId.EqualsOrChildOf(classId))
			{
				return false;
			}
		}

		// get player base class
		CharacterClass currentBaseId = player.getBaseClass();
		CharacterClass baseCID = currentBaseId;

		// we need 2nd occupation ID
		CharacterClass baseClassId;
        CharacterClass? parentClass = baseCID.GetParentClass();
		if (baseCID.GetLevel() > 2 && parentClass != null)
		{
			baseClassId = parentClass.Value;
		}
		else
		{
			baseClassId = currentBaseId;
		}

		Set<CharacterClass>? availSubs = getSubclasses(player, baseClassId);
		if (availSubs == null || availSubs.isEmpty())
		{
			return false;
		}

		bool found = availSubs.Contains(classId);
		return found;
	}

	protected virtual bool checkVillageMasterRace(CharacterClass pClass)
	{
		return true;
	}

	protected virtual bool checkVillageMasterTeachType(CharacterClass pClass)
	{
		return true;
	}

	/**
	 * Returns true if this PlayerClass is allowed for master
	 * @param pclass
	 * @return
	 */
	public bool checkVillageMaster(CharacterClass pclass)
	{
		if (Config.ALT_GAME_SUBCLASS_EVERYWHERE)
		{
			return true;
		}
		return checkVillageMasterRace(pclass) && checkVillageMasterTeachType(pclass);
	}

	private void dissolveClan(Player player, int? clanId)
	{
        Clan? clan = player.getClan();
		if (!player.isClanLeader() || clan == null)
		{
			player.sendPacket(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
			return;
		}

		if (clan.getAllyId() != 0)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_DISPERSE_THE_CLANS_IN_YOUR_ALLIANCE);
			return;
		}
		if (clan.isAtWar())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_DISSOLVE_A_CLAN_WHILE_ENGAGED_IN_A_WAR);
			return;
		}
		if (clan.getCastleId() != 0 || clan.getHideoutId() != 0 || clan.getFortId() != 0)
		{
			player.sendPacket(SystemMessageId.YOU_CAN_T_DISBAND_THE_CLAN_THAT_HAS_A_CLAN_HALL_OR_CASTLE);
			return;
		}

		foreach (Castle castle in CastleManager.getInstance().getCastles())
		{
			if (SiegeManager.getInstance().checkIsRegistered(clan, castle.getResidenceId()))
			{
				player.sendPacket(SystemMessageId.YOU_CANNOT_DISSOLVE_A_CLAN_DURING_A_SIEGE_OR_WHILE_PROTECTING_A_CASTLE);
				return;
			}
		}
		foreach (Fort fort in InstanceManagers.FortManager.getInstance().getForts())
		{
			if (FortSiegeManager.getInstance().checkIsRegistered(clan, fort.getResidenceId()))
			{
				player.sendPacket(SystemMessageId.YOU_CANNOT_DISSOLVE_A_CLAN_DURING_A_SIEGE_OR_WHILE_PROTECTING_A_CASTLE);
				return;
			}
		}

		if (player.isInsideZone(ZoneId.SIEGE))
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_DISSOLVE_A_CLAN_DURING_A_SIEGE_OR_WHILE_PROTECTING_A_CASTLE);
			return;
		}
		if (clan.getDissolvingExpiryTime() > DateTime.UtcNow)
		{
			player.sendPacket(SystemMessageId.YOU_HAVE_ALREADY_REQUESTED_THE_DISSOLUTION_OF_S1_CLAN);
			return;
		}

		clan.setDissolvingExpiryTime(DateTime.UtcNow + TimeSpan.FromDays(Config.ALT_CLAN_DISSOLVE_DAYS)); // 24*60*60*1000 = 86400000
		clan.updateClanInDB();

		// The clan leader should take the XP penalty of a full death.
		player.calculateDeathExpPenalty(null);
		ClanTable.getInstance().scheduleRemoveClan(clan.getId());
	}

	private void recoverClan(Player player, int? clanId)
	{
        Clan? clan = player.getClan();
		if (!player.isClanLeader() || clan == null)
		{
			player.sendPacket(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
			return;
		}

		clan.setDissolvingExpiryTime(null);
		clan.updateClanInDB();
	}

	private void createSubPledge(Player player, string clanName, string leaderName, int pledgeType, int minClanLvl)
	{
        Clan? clan = player.getClan();
		if (!player.isClanLeader() || clan == null)
		{
			player.sendPacket(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
			return;
		}

		if (clan.getLevel() < minClanLvl)
		{
			if (pledgeType == Clan.SUBUNIT_ACADEMY)
			{
				player.sendPacket(SystemMessageId.TO_ESTABLISH_A_CLAN_ACADEMY_YOUR_CLAN_MUST_BE_LEVEL_5_OR_HIGHER);
			}
			else
			{
				player.sendPacket(SystemMessageId.THE_CONDITIONS_NECESSARY_TO_CREATE_A_MILITARY_UNIT_HAVE_NOT_BEEN_MET);
			}

			return;
		}

		if (string.IsNullOrEmpty(clanName) || !clanName.ContainsAlphaNumericOnly() || !isValidName(clanName) ||
		    2 > clanName.Length)
		{
			player.sendPacket(SystemMessageId.CLAN_NAME_IS_INVALID);
			return;
		}

		if (clanName.Length > 16)
		{
			player.sendPacket(SystemMessageId.CLAN_NAME_S_LENGTH_IS_INCORRECT);
			return;
		}

		SystemMessagePacket sm;
		foreach (Clan tempClan in ClanTable.getInstance().getClans())
		{
			if (tempClan.getSubPledge(clanName) != null)
			{
				if (pledgeType == Clan.SUBUNIT_ACADEMY)
				{
					sm = new SystemMessagePacket(SystemMessageId.S1_ALREADY_EXISTS);
					sm.Params.addString(clanName);
					player.sendPacket(sm);
				}
				else
				{
					player.sendPacket(SystemMessageId.ANOTHER_MILITARY_UNIT_IS_ALREADY_USING_THAT_NAME_PLEASE_ENTER_A_DIFFERENT_NAME);
				}

				return;
			}
		}

		if (pledgeType != Clan.SUBUNIT_ACADEMY)
		{
			ClanMember? member = clan.getClanMember(leaderName);
			if (member == null || member.getPledgeType() != 0 || clan.getLeaderSubPledge(member.getObjectId()) > 0)
			{
				if (pledgeType >= Clan.SUBUNIT_KNIGHT1)
				{
					player.sendPacket(SystemMessageId.THE_CAPTAIN_OF_THE_ORDER_OF_KNIGHTS_CANNOT_BE_APPOINTED);
				}
				else if (pledgeType >= Clan.SUBUNIT_ROYAL1)
				{
					player.sendPacket(SystemMessageId.THE_GUARD_CAPTAIN_CANNOT_BE_APPOINTED);
				}

				return;
			}
		}

		int leaderId = pledgeType != Clan.SUBUNIT_ACADEMY ? clan.getClanMember(leaderName)?.getObjectId() ?? 0 : 0;
		if (clan.createSubPledge(player, pledgeType, leaderId, clanName) == null)
		{
			return;
		}

		if (pledgeType == Clan.SUBUNIT_ACADEMY)
		{
			sm = new SystemMessagePacket(SystemMessageId.CONGRATULATIONS_THE_S1_S_CLAN_ACADEMY_HAS_BEEN_CREATED);
			sm.Params.addString(clan.getName());
		}
		else if (pledgeType >= Clan.SUBUNIT_KNIGHT1)
		{
			sm = new SystemMessagePacket(SystemMessageId.THE_KNIGHTS_OF_S1_HAVE_BEEN_CREATED);
			sm.Params.addString(clan.getName());
		}
		else if (pledgeType >= Clan.SUBUNIT_ROYAL1)
		{
			sm = new SystemMessagePacket(SystemMessageId.THE_ROYAL_GUARD_OF_S1_HAVE_BEEN_CREATED);
			sm.Params.addString(clan.getName());
		}
		else
		{
			sm = new SystemMessagePacket(SystemMessageId.YOUR_CLAN_HAS_BEEN_CREATED);
		}
		player.sendPacket(sm);

		if (pledgeType != Clan.SUBUNIT_ACADEMY)
		{
			ClanMember? leaderSubPledge = clan.getClanMember(leaderName);
			Player? leaderPlayer = leaderSubPledge?.getPlayer();
			if (leaderPlayer != null)
			{
				leaderPlayer.setPledgeClass(ClanMember.calculatePledgeClass(leaderPlayer));
				leaderPlayer.updateUserInfo();
			}
		}
	}

	private void renameSubPledge(Player player, int pledgeType, string pledgeName)
	{
        Clan? clan = player.getClan();
		if (!player.isClanLeader() || clan == null)
		{
			player.sendPacket(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
			return;
		}

		Clan.SubPledge? subPledge = clan.getSubPledge(pledgeType);
		if (subPledge == null)
		{
			player.sendMessage("Pledge don't exists.");
			return;
		}

		if (string.IsNullOrEmpty(pledgeName) || !pledgeName.ContainsAlphaNumericOnly() || !isValidName(pledgeName) ||
		    2 > pledgeName.Length)
		{
			player.sendPacket(SystemMessageId.CLAN_NAME_IS_INVALID);
			return;
		}

		if (pledgeName.Length > 16)
		{
			player.sendPacket(SystemMessageId.CLAN_NAME_S_LENGTH_IS_INCORRECT);
			return;
		}

		subPledge.setName(pledgeName);
		clan.updateSubPledgeInDB(subPledge.getId());
		clan.broadcastClanStatus();
		player.sendMessage("Pledge name changed.");
	}

	private void assignSubPledgeLeader(Player player, string clanName, string leaderName)
	{
        Clan? clan = player.getClan();
		if (!player.isClanLeader() || clan == null)
		{
			player.sendPacket(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
			return;
		}
		if (leaderName.Length > 16)
		{
			player.sendPacket(SystemMessageId.ENTER_THE_CHARACTER_S_NAME_UP_TO_16_CHARACTERS);
			return;
		}
		if (player.getName().equals(leaderName))
		{
			player.sendPacket(SystemMessageId.THE_GUARD_CAPTAIN_CANNOT_BE_APPOINTED);
			return;
		}

		Clan.SubPledge? subPledge = clan.getSubPledge(clanName);
		if (null == subPledge || subPledge.getId() == Clan.SUBUNIT_ACADEMY)
		{
			player.sendPacket(SystemMessageId.CLAN_NAME_IS_INVALID);
			return;
		}

		ClanMember? member = clan.getClanMember(leaderName);
		if (member == null || member.getPledgeType() != 0 || clan.getLeaderSubPledge(member.getObjectId()) > 0)
		{
			if (subPledge.getId() >= Clan.SUBUNIT_KNIGHT1)
			{
				player.sendPacket(SystemMessageId.THE_CAPTAIN_OF_THE_ORDER_OF_KNIGHTS_CANNOT_BE_APPOINTED);
			}
			else if (subPledge.getId() >= Clan.SUBUNIT_ROYAL1)
			{
				player.sendPacket(SystemMessageId.THE_GUARD_CAPTAIN_CANNOT_BE_APPOINTED);
			}

			return;
		}

		subPledge.setLeaderId(member.getObjectId());
		clan.updateSubPledgeInDB(subPledge.getId());

		Player? leaderPlayer = member.getPlayer();
		if (leaderPlayer != null)
		{
			leaderPlayer.setPledgeClass(ClanMember.calculatePledgeClass(leaderPlayer));
			leaderPlayer.updateUserInfo();
		}

		clan.broadcastClanStatus();
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_HAS_BEEN_SELECTED_AS_THE_CAPTAIN_OF_S2);
		sm.Params.addString(leaderName);
		sm.Params.addString(clanName);
		clan.broadcastToOnlineMembers(sm);
	}

	/**
	 * this displays PledgeSkillList to the player.
	 * @param player
	 */
	public static void showPledgeSkillList(Player player)
	{
        Clan? clan = player.getClan();
		if (!player.isClanLeader() || clan == null)
		{
			HtmlContent htmlContent = HtmlContent.LoadFromFile("html/villagemaster/NotClanLeader.htm", player);
			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 0, htmlContent);

			player.sendPacket(html);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return;
		}

		List<SkillLearn> skills = SkillTreeData.getInstance().getAvailablePledgeSkills(clan);
		if (skills.Count == 0)
		{
			if (clan.getLevel() <= 1)
			{
				SystemMessagePacket sm = new(SystemMessageId.YOU_DO_NOT_HAVE_ANY_FURTHER_SKILLS_TO_LEARN_COME_BACK_WHEN_YOU_HAVE_REACHED_LEVEL_S1);
				if (clan.getLevel() <= 1)
				{
					sm.Params.addInt(1);
				}
				else
				{
					sm.Params.addInt(clan.getLevel() + 1);
				}
				player.sendPacket(sm);
			}
			else
			{
				HtmlContent htmlContent = HtmlContent.LoadFromFile("html/villagemaster/NoMoreSkills.htm", player);
				NpcHtmlMessagePacket html = new(null, 0, htmlContent);
				player.sendPacket(html);
			}
		}
		else
		{
			player.sendPacket(new ExAcquirableSkillListByClassPacket(skills, AcquireSkillType.PLEDGE));
		}

		player.sendPacket(ActionFailedPacket.STATIC_PACKET);
	}

	private static bool isValidName(string name)
	{
		return Config.CLAN_NAME_TEMPLATE.IsMatch(name);
	}
}
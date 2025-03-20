using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;
using Config = L2Dn.GameServer.Configuration.Config;
using Pet = L2Dn.GameServer.Model.Actor.Instances.Pet;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * EditChar admin command implementation.
 */
public class AdminEditChar: IAdminCommandHandler
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AdminEditChar));

	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_edit_character",
		"admin_current_player",
		"admin_setreputation", // sets reputation of target char to any amount. //setreputation <amout>
		"admin_nokarma", // sets reputation to 0 if its negative.
		"admin_setfame", // sets fame of target char to any amount. //setfame <fame>
		"admin_character_list", // same as character_info, kept for compatibility purposes
		"admin_character_info", // given a player name, displays an information window
		"admin_show_characters", // list of characters
		"admin_find_character", // find a player by his name or a part of it (case-insensitive)
		"admin_find_ip", // find all the player connections from a given IPv4 number
		"admin_find_account", // list all the characters from an account (useful for GMs w/o DB access)
		"admin_find_dualbox", // list all the IPs with more than 1 char logged in (dualbox)
		"admin_strict_find_dualbox",
		"admin_tracert",
		"admin_rec", // gives recommendation points
		"admin_settitle", // changes char title
		"admin_changename", // changes char name
		"admin_setsex", // changes characters' sex
		"admin_setcolor", // change charnames' color display
		"admin_settcolor", // change char title color
		"admin_setclass", // changes chars' classId
		"admin_setpk", // changes PK count
		"admin_setpvp", // changes PVP count
		"admin_set_pvp_flag",
		"admin_fullfood", // fulfills a pet's food bar
		"admin_remove_clan_penalty", // removes clan penalties
		"admin_summon_info", // displays an information window about target summon
		"admin_unsummon",
		"admin_summon_setlvl",
		"admin_show_pet_inv",
		"admin_partyinfo",
		"admin_setnoble",
		"admin_set_hp",
		"admin_set_mp",
		"admin_set_cp",
		"admin_setparam",
		"admin_unsetparam",
    ];

	public bool useAdminCommand(string command, Player activeChar)
	{
		if (command.equals("admin_current_player"))
		{
			ShowCharacterInfo(activeChar, activeChar);
		}
		else if (command.startsWith("admin_character_info"))
        {
            WorldObject? activeCharTarget = activeChar.getTarget();
			string[] data = command.Split(" ");
			if (data.Length > 1)
			{
				ShowCharacterInfo(activeChar, World.getInstance().getPlayer(data[1]));
			}
			else if (activeCharTarget != null && activeCharTarget.isPlayer())
			{
				ShowCharacterInfo(activeChar, activeCharTarget.getActingPlayer());
			}
			else
			{
				activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			}
		}
		else if (command.startsWith("admin_character_list"))
		{
			listCharacters(activeChar, 0);
		}
		else if (command.startsWith("admin_show_characters"))
		{
			try
			{
				string val = command.Substring(22);
				int page = int.Parse(val);
				listCharacters(activeChar, page);
			}
			catch (IndexOutOfRangeException e)
			{
                LOGGER.Error(e);
				// Case of empty page number
				BuilderUtil.sendSysMessage(activeChar, "Usage: //show_characters <page_number>");
			}
		}
		else if (command.startsWith("admin_find_character"))
		{
			try
			{
				string val = command.Substring(21);
				findCharacter(activeChar, val);
			}
			catch (IndexOutOfRangeException e)
			{
                LOGGER.Error(e);
                // Case of empty character name
				BuilderUtil.sendSysMessage(activeChar, "Usage: //find_character <character_name>");
				listCharacters(activeChar, 0);
			}
		}
		else if (command.startsWith("admin_find_ip"))
		{
			try
			{
				string val = command.Substring(14);
				findCharactersPerIp(activeChar, val);
			}
			catch (Exception e)
			{
                LOGGER.Error(e);
                // Case of empty or malformed IP number
				BuilderUtil.sendSysMessage(activeChar, "Usage: //find_ip <www.xxx.yyy.zzz>");
				listCharacters(activeChar, 0);
			}
		}
		else if (command.startsWith("admin_find_account"))
		{
			try
			{
				string val = command.Substring(19);
				FindCharactersPerAccount(activeChar, val);
			}
			catch (Exception e)
			{
                LOGGER.Error(e);
                // Case of empty or malformed player name
				BuilderUtil.sendSysMessage(activeChar, "Usage: //find_account <player_name>");
				listCharacters(activeChar, 0);
			}
		}
		else if (command.startsWith("admin_edit_character"))
		{
            WorldObject? activeCharTarget = activeChar.getTarget();
			string[] data = command.Split(" ");
			if (data.Length > 1)
			{
				EditCharacter(activeChar, data[1]);
			}
			else if (activeCharTarget != null && activeCharTarget.isPlayer())
			{
				EditCharacter(activeChar, null);
			}
			else
			{
				activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			}
		}
		else if (command.startsWith("admin_setreputation"))
		{
			try
			{
				string val = command.Substring(20);
				int reputation = int.Parse(val);
				SetTargetReputation(activeChar, reputation);
			}
			catch (Exception e)
			{
				if (Config.General.DEVELOPER)
				{
					LOGGER.Warn("Set reputation error: " + e);
				}
				BuilderUtil.sendSysMessage(activeChar, "Usage: //setreputation <new_reputation_value>");
			}
		}
		else if (command.startsWith("admin_nokarma"))
		{
            WorldObject? activeCharTarget = activeChar.getTarget();
			if (activeCharTarget == null || !activeCharTarget.isPlayer())
			{
				BuilderUtil.sendSysMessage(activeChar, "You must target a player.");
				return false;
			}

			if (activeCharTarget.getActingPlayer()?.getReputation() < 0)
			{
				SetTargetReputation(activeChar, 0);
			}
		}
		else if (command.startsWith("admin_setpk"))
		{
			try
			{
				string val = command.Substring(12);
				int pk = int.Parse(val);
				WorldObject? target = activeChar.getTarget();
                Player? player = target?.getActingPlayer();
				if (target != null && target.isPlayer() && player != null)
				{
					player.setPkKills(pk);
					player.setTotalKills(pk);
					player.broadcastUserInfo();
					player.sendMessage("A GM changed your PK count to " + pk);
					activeChar.sendMessage(player.getName() + "'s PK count changed to " + pk);
				}
				else
				{
					activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				}
			}
			catch (Exception e)
			{
				if (Config.General.DEVELOPER)
				{
					LOGGER.Warn("Set pk error: " + e);
				}
				BuilderUtil.sendSysMessage(activeChar, "Usage: //setpk <pk_count>");
			}
		}
		else if (command.startsWith("admin_setpvp"))
		{
			try
			{
				string val = command.Substring(13);
				int pvp = int.Parse(val);
				WorldObject? target = activeChar.getTarget();
				if (target != null && target.isPlayer())
				{
					Player player = (Player) target;
					player.setPvpKills(pvp);
					player.setTotalKills(pvp);
					player.updatePvpTitleAndColor(false);
					player.broadcastUserInfo();
					player.sendMessage("A GM changed your PVP count to " + pvp);
					activeChar.sendMessage(player.getName() + "'s PVP count changed to " + pvp);
				}
				else
				{
					activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				}
			}
			catch (Exception e)
			{
				if (Config.General.DEVELOPER)
				{
					LOGGER.Warn("Set pvp error: " + e);
				}
				BuilderUtil.sendSysMessage(activeChar, "Usage: //setpvp <pvp_count>");
			}
		}
		else if (command.startsWith("admin_setfame"))
		{
			try
			{
				string val = command.Substring(14);
				int fame = int.Parse(val);
				WorldObject? target = activeChar.getTarget();
				if (target != null && target.isPlayer())
				{
					Player player = (Player) target;
					player.setFame(fame);
					player.broadcastUserInfo();
					player.sendMessage("A GM changed your Reputation points to " + fame);
					activeChar.sendMessage(player.getName() + "'s Fame changed to " + fame);
				}
				else
				{
					activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				}
			}
			catch (Exception e)
			{
				if (Config.General.DEVELOPER)
				{
					LOGGER.Warn("Set Fame error: " + e);
				}
				BuilderUtil.sendSysMessage(activeChar, "Usage: //setfame <new_fame_value>");
			}
		}
		else if (command.startsWith("admin_rec"))
		{
			try
			{
				string val = command.Substring(10);
				int recVal = int.Parse(val);
				WorldObject? target = activeChar.getTarget();
				if (target != null && target.isPlayer())
				{
					Player player = (Player) target;
					player.setRecomHave(recVal);
					player.broadcastUserInfo();
					player.sendMessage("A GM changed your Recommend points to " + recVal);
					activeChar.sendMessage(player.getName() + "'s Recommend changed to " + recVal);
				}
				else
				{
					activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				}
			}
			catch (Exception e)
			{
                LOGGER.Error(e);
				BuilderUtil.sendSysMessage(activeChar, "Usage: //rec number");
			}
		}
		else if (command.startsWith("admin_setclass"))
		{
			try
			{
				string val = command.Substring(15).Trim();
				CharacterClass classidval = (CharacterClass)int.Parse(val);
				WorldObject? target = activeChar.getTarget();
				if (target == null || !target.isPlayer())
				{
					return false;
				}

				Player? player = target.getActingPlayer();
				if (Enum.IsDefined(classidval) && player != null && player.getClassId() != classidval)
				{
					player.setClassId(classidval);
					if (player.isSubClassActive())
					{
                        // TODO: refactor later
						player.getSubClasses().get(player.getClassIndex())!.setClassId(player.getActiveClass());
					}
					else
					{
						player.setBaseClass(player.getActiveClass());
					}

					// Sex checks.
					if (player.getRace() == Race.KAMAEL)
					{
						switch (classidval)
						{
							case (CharacterClass)123: // Soldier (Male) // TODO: update enumeration
							case CharacterClass.TROOPER: // Trooper
							case CharacterClass.BERSERKER: // Berserker
							case (CharacterClass)128: // Soul Breaker (Male)
							case CharacterClass.DOOMBRINGER: // Doombringer
							case (CharacterClass)132: // Soul Hound (Male)
							case (CharacterClass)157: // Tyrr Doombringer
							{
								if (player.getAppearance().getSex() == Sex.Female)
								{
									player.getAppearance().setSex(Sex.Male);
								}
								break;
							}
							case (CharacterClass)124: // Soldier (Female)
							case CharacterClass.WARDER: // Warder
							case (CharacterClass)129: // Soul Breaker (Female)
							case CharacterClass.SOUL_RANGER: // Arbalester
							case (CharacterClass)133: // Soul Hound (Female)
							case CharacterClass.TRICKSTER: // Trickster
							case (CharacterClass)165: // Yul Trickster
							{
								if (player.getAppearance().getSex() != Sex.Female)
								{
									player.getAppearance().setSex(Sex.Female);
								}
								break;
							}
						}
					}

					// Sylph checks
					if (!CategoryData.getInstance().isInCategory(CategoryType.SYLPH_ALL_CLASS, classidval) && player.getActiveWeaponItem() != null && player.getActiveWeaponItem().getItemType() == WeaponType.PISTOLS)
					{
						Item? itemToRemove = player.getInventory().getPaperdollItem(Inventory.PAPERDOLL_RHAND);
						if (itemToRemove != null)
						{
							long slot = player.getInventory().getSlotFromItem(itemToRemove);
							player.getInventory().unEquipItemInBodySlot(slot);

							InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(itemToRemove, ItemChangeType.MODIFIED));
							player.sendInventoryUpdate(iu);
							player.broadcastUserInfo();
						}
					}

					// Death Knight checks.
					if (CategoryData.getInstance().isInCategory(CategoryType.DEATH_KNIGHT_ALL_CLASS, classidval))
					{
						player.getAppearance().setSex(Sex.Male);
						if (!player.isDeathKnight())
						{
							player.setDeathKnight(true);
						}
					}
					else
					{
						if (player.isDeathKnight())
						{
							player.setDeathKnight(false);
						}
					}

					// Vanguard checks.
					if (CategoryData.getInstance().isInCategory(CategoryType.VANGUARD_ALL_CLASS, classidval))
					{
						player.getAppearance().setSex(Sex.Male);
						player.disarmShield();
						if (!player.isVanguard())
						{
							player.setVanguard(true);
						}
					}
					else
					{
						if (player.isVanguard())
						{
							player.setVanguard(false);
						}
					}

					// Assassin checks.
					if (CategoryData.getInstance().isInCategory(CategoryType.ASSASSIN_ALL_CLASS, classidval))
					{
						if (player.getRace() == Race.HUMAN)
						{
							player.getAppearance().setSex(Sex.Male);
						}
						else
						{
							player.getAppearance().setSex(Sex.Female);
						}

						if (CategoryData.getInstance().isInCategory(CategoryType.FOURTH_CLASS_GROUP, classidval))
						{
							player.setAssassinationPoints(0);
						}
					}

					string newclass = ClassListData.getInstance().getClass(player.getClassId()).getClassName();
					player.store(false);
					foreach (Skill oldSkill in player.getAllSkills())
					{
						if (oldSkill.IsBad)
						{
							AutoUseTaskManager.getInstance().removeAutoSkill(player, oldSkill.Id);
						}
						else
						{
							AutoUseTaskManager.getInstance().removeAutoBuff(player, oldSkill.Id);
						}
						player.removeSkill(oldSkill, true, true);
					}

					player.broadcastUserInfo();
					player.sendSkillList();
					player.rewardSkills();
					player.sendPacket(new ExSubJobInfoPacket(player, SubclassInfoType.CLASS_CHANGED));
					player.sendPacket(new ExUserInfoInventoryWeightPacket(player));
					player.sendMessage("A GM changed your class to " + newclass + ".");
					activeChar.sendMessage(player.getName() + " is a " + newclass + ".");
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: //setclass <valid_new_classid>");
				}
			}
			catch (IndexOutOfRangeException e)
			{
                LOGGER.Error(e);
				AdminHtml.showAdminHtml(activeChar, "setclass/human_fighter.htm");
			}
			catch (FormatException e)
			{
                LOGGER.Error(e);
				BuilderUtil.sendSysMessage(activeChar, "Usage: //setclass <valid_new_classid>");
			}
		}
		else if (command.startsWith("admin_settitle"))
		{
			try
			{
				string val = command.Substring(15);
				WorldObject? target = activeChar.getTarget();
				Player? player = null;
				if (target != null && target.isPlayer())
				{
					player = (Player) target;
				}
				else
				{
					return false;
				}
				player.setTitle(val);
				player.sendMessage("Your title has been changed by a GM");
				player.broadcastTitleInfo();
			}
			catch (IndexOutOfRangeException e)
			{
                LOGGER.Error(e);
                // Case of empty character title
				BuilderUtil.sendSysMessage(activeChar, "You need to specify the new title.");
			}
		}
		else if (command.startsWith("admin_changename"))
		{
			try
			{
				string val = command.Substring(17);
				WorldObject? target = activeChar.getTarget();
				Player? player = null;
				if (target != null && target.isPlayer())
				{
					player = (Player) target;
				}
				else
				{
					return false;
				}
				if (CharInfoTable.getInstance().doesCharNameExist(val))
				{
					BuilderUtil.sendSysMessage(activeChar, "Warning, player " + val + " already exists");
					return false;
				}
				player.setName(val);
				CharInfoTable.getInstance().addName(player);
				player.storeMe();

				BuilderUtil.sendSysMessage(activeChar, "Changed name to " + val);
				player.sendMessage("Your name has been changed by a GM.");
				player.broadcastUserInfo();

                Party? party = player.getParty();
				if (player.isInParty() && party != null)
				{
					// Delete party window for other party members
					party.broadcastToPartyMembers(player, PartySmallWindowDeleteAllPacket.STATIC_PACKET);
					foreach (Player member in party.getMembers())
					{
						// And re-add
						if (member != player)
						{
							member.sendPacket(new PartySmallWindowAllPacket(member, party));
						}
					}
				}

                Clan? clan = player.getClan();
				if (clan != null)
				{
                    clan.broadcastClanStatus();
				}
			}
			catch (IndexOutOfRangeException e)
			{
                LOGGER.Error(e);
                // Case of empty character name
				BuilderUtil.sendSysMessage(activeChar, "Usage: //setname new_name_for_target");
			}
		}
		else if (command.startsWith("admin_setsex"))
		{
			WorldObject? target = activeChar.getTarget();
			Player? player = null;
			if (target != null && target.isPlayer())
			{
				player = (Player) target;
			}
			else
			{
				return false;
			}
			if (player.getAppearance().getSex() == Sex.Female)
			{
				player.getAppearance().setSex(Sex.Male);
			}
			else
			{
				player.getAppearance().setSex(Sex.Female);
			}
			player.sendMessage("Your gender has been changed by a GM");
			player.broadcastUserInfo();
		}
		else if (command.startsWith("admin_setcolor"))
		{
			try
			{
				string val = command.Substring(15);
				WorldObject? target = activeChar.getTarget();
				Player? player = null;
				if (target != null && target.isPlayer())
				{
					player = (Player) target;
				}
				else
				{
					return false;
				}
				player.getAppearance().setNameColor(new Color(int.Parse("0x" + val, NumberStyles.HexNumber)));
				player.sendMessage("Your name color has been changed by a GM");
				player.broadcastUserInfo();
			}
			catch (Exception e)
			{
                LOGGER.Error(e);
                // Case of empty color or invalid hex string
				BuilderUtil.sendSysMessage(activeChar, "You need to specify a valid new color.");
			}
		}
		else if (command.startsWith("admin_settcolor"))
		{
			try
			{
				string val = command.Substring(16);
				WorldObject? target = activeChar.getTarget();
				Player? player = null;
				if (target != null && target.isPlayer())
				{
					player = (Player) target;
				}
				else
				{
					return false;
				}
				player.getAppearance().setTitleColor(new Color(int.Parse("0x" + val, NumberStyles.HexNumber)));
				player.sendMessage("Your title color has been changed by a GM");
				player.broadcastUserInfo();
			}
			catch (Exception e)
			{
                LOGGER.Error(e);
                // Case of empty color or invalid hex string
				BuilderUtil.sendSysMessage(activeChar, "You need to specify a valid new color.");
			}
		}
		else if (command.startsWith("admin_fullfood"))
		{
			WorldObject? target = activeChar.getTarget();
			if (target != null && target.isPet())
			{
				Pet targetPet = (Pet)target;
				targetPet.setCurrentFed(targetPet.getMaxFed());
				targetPet.broadcastStatusUpdate();
			}
			else
			{
				activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			}
		}
		else if (command.startsWith("admin_remove_clan_penalty"))
		{
			try
			{
				StringTokenizer st = new StringTokenizer(command, " ");
				if (st.countTokens() != 3)
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: //remove_clan_penalty join|create charname");
					return false;
				}

				st.nextToken();

				bool changeCreateExpiryTime = st.nextToken().equalsIgnoreCase("create");
				string playerName = st.nextToken();
				Player? player = World.getInstance().getPlayer(playerName);
				if (player == null)
				{
					using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
					IQueryable<DbCharacter> query = ctx.Characters.Where(r => r.Name == playerName);
					if (changeCreateExpiryTime)
						query.ExecuteUpdate(s => s.SetProperty(r => r.ClanCreateExpiryTime, (DateTime?)null));
					else
						query.ExecuteUpdate(s => s.SetProperty(r => r.ClanJoinExpiryTime, (DateTime?)null));
				}
				else if (changeCreateExpiryTime) // removing penalty
				{
					player.setClanCreateExpiryTime(null);
				}
				else
				{
					player.setClanJoinExpiryTime(null);
				}

				BuilderUtil.sendSysMessage(activeChar, "Clan penalty successfully removed to character: " + playerName);
			}
			catch (Exception e)
			{
				LOGGER.Warn(e);
			}
		}
		else if (command.startsWith("admin_find_dualbox"))
		{
			int multibox = 2;
			try
			{
				string val = command.Substring(19);
				multibox = int.Parse(val);
				if (multibox < 1)
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: //find_dualbox [number > 0]");
					return false;
				}
			}
			catch (Exception e)
			{
                LOGGER.Error(e);
			}
			findDualbox(activeChar, multibox);
		}
		else if (command.startsWith("admin_strict_find_dualbox"))
		{
			int multibox = 2;
			try
			{
				string val = command.Substring(26);
				multibox = int.Parse(val);
				if (multibox < 1)
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: //strict_find_dualbox [number > 0]");
					return false;
				}
			}
			catch (Exception e)
			{
                LOGGER.Error(e);
			}

			findDualboxStrict(activeChar, multibox);
		}
		else if (command.startsWith("admin_tracert"))
		{
			string[] data = command.Split(" ");
			Player? pl = null;
			if (data.Length > 1)
			{
				pl = World.getInstance().getPlayer(data[1]);
			}
			else
			{
				WorldObject? target = activeChar.getTarget();
				if (target != null && target.isPlayer())
				{
					pl = (Player) target;
				}
			}

			if (pl == null)
			{
				activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				return false;
			}

			GameSession? client = pl.getClient();
			if (client == null)
			{
				BuilderUtil.sendSysMessage(activeChar, "Client is null.");
				return false;
			}

			if (client.IsDetached)
			{
				BuilderUtil.sendSysMessage(activeChar, "Client is detached.");
				return false;
			}

			// TODO: trace
			// String ip;
			// int[][] trace = client.getTrace();
			// for (int i = 0; i < trace.Length; i++)
			// {
			// 	ip = "";
			// 	for (int o = 0; o < trace[0].Length; o++)
			// 	{
			// 		ip = ip + trace[i][o];
			// 		if (o != (trace[0].Length - 1))
			// 		{
			// 			ip = ip + ".";
			// 		}
			// 	}
			//
			// 	BuilderUtil.sendSysMessage(activeChar, "Hop" + i + ": " + ip);
			// }
		}
		else if (command.startsWith("admin_summon_info"))
		{
			WorldObject? target = activeChar.getTarget();
			if (target != null && target.isSummon())
			{
				gatherSummonInfo((Summon) target, activeChar);
			}
			else
			{
				BuilderUtil.sendSysMessage(activeChar, "Invalid target.");
			}
		}
		else if (command.startsWith("admin_unsummon"))
		{
			WorldObject? target = activeChar.getTarget();
			if (target != null && target.isSummon())
			{
				((Summon) target).unSummon(((Summon) target).getOwner());
			}
			else
			{
				BuilderUtil.sendSysMessage(activeChar, "Usable only with Pets/Summons");
			}
		}
		else if (command.startsWith("admin_summon_setlvl"))
		{
			WorldObject? target = activeChar.getTarget();
			if (target != null && target.isPet())
			{
				Pet pet = (Pet) target;
				try
				{
					string val = command.Substring(20);
					int level = int.Parse(val);
					long oldexp = pet.getStat().getExp();
					long newexp = pet.getStat().getExpForLevel(level);
					if (oldexp > newexp)
					{
						pet.getStat().removeExp(oldexp - newexp);
					}
					else if (oldexp < newexp)
					{
						pet.getStat().addExp(newexp - oldexp);
					}
				}
				catch (Exception e)
				{
                    LOGGER.Error(e);
				}
			}
			else
			{
				BuilderUtil.sendSysMessage(activeChar, "Usable only with Pets");
			}
		}
		else if (command.startsWith("admin_show_pet_inv"))
		{
			WorldObject? target;
			try
			{
				string val = command.Substring(19);
				int objId = int.Parse(val);
				target = World.getInstance().getPet(objId);
			}
			catch (Exception e)
			{
                LOGGER.Error(e);
				target = activeChar.getTarget();
			}

			if (target != null && target.isPet())
			{
				activeChar.sendPacket(new GMViewItemListPacket(1, (Pet) target));
			}
			else
			{
				BuilderUtil.sendSysMessage(activeChar, "Usable only with Pets");
			}
		}
		else if (command.startsWith("admin_partyinfo"))
		{
			WorldObject? target;
			try
			{
				string val = command.Substring(16);
				target = World.getInstance().getPlayer(val);
				if (target == null)
				{
					target = activeChar.getTarget();
				}
			}
			catch (Exception e)
			{
                LOGGER.Error(e);
				target = activeChar.getTarget();
			}

			if (target != null && target.isPlayer())
			{
				if (((Player) target).isInParty())
				{
					gatherPartyInfo((Player) target, activeChar);
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "Not in party.");
				}
			}
			else
			{
				activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			}
		}
		else if (command.equals("admin_setnoble"))
        {
            WorldObject? activeCharTarget = activeChar.getTarget();
			Player? player;
			if (activeCharTarget != null && activeCharTarget.isPlayer())
			{
				player = (Player?)activeChar.getTarget();
			}
			else
			{
				player = activeChar;
			}

			if (player != null)
			{
				player.setNoble(!player.isNoble());
				if (player.ObjectId != activeChar.ObjectId)
				{
					BuilderUtil.sendSysMessage(activeChar, "You've changed nobless status of: " + player.getName());
				}
				player.broadcastUserInfo();
				player.sendMessage("GM changed your nobless status!");
			}
		}
		else if (command.startsWith("admin_set_hp"))
		{
			string[] data = command.Split(" ");
			try
			{
				WorldObject? target = activeChar.getTarget();
				if (target == null || !target.isCreature())
				{
					activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
					return false;
				}
				((Creature) target).setCurrentHp(double.Parse(data[1], CultureInfo.InvariantCulture));
			}
			catch (Exception e)
			{
                LOGGER.Error(e);
				BuilderUtil.sendSysMessage(activeChar, "Usage: //set_hp 1000");
			}
		}
		else if (command.startsWith("admin_set_mp"))
		{
			string[] data = command.Split(" ");
			try
			{
				WorldObject? target = activeChar.getTarget();
				if (target == null || !target.isCreature())
				{
					activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
					return false;
				}
				((Creature) target).setCurrentMp(double.Parse(data[1], CultureInfo.InvariantCulture));
			}
			catch (Exception e)
			{
                LOGGER.Error(e);
				BuilderUtil.sendSysMessage(activeChar, "Usage: //set_mp 1000");
			}
		}
		else if (command.startsWith("admin_set_cp"))
		{
			string[] data = command.Split(" ");
			try
			{
				WorldObject? target = activeChar.getTarget();
				if (target == null || !target.isCreature())
				{
					activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
					return false;
				}
				((Creature) target).setCurrentCp(double.Parse(data[1], CultureInfo.InvariantCulture));
			}
			catch (Exception e)
			{
                LOGGER.Error(e);
				BuilderUtil.sendSysMessage(activeChar, "Usage: //set_cp 1000");
			}
		}
		else if (command.startsWith("admin_set_pvp_flag"))
		{
			try
			{
				WorldObject? target = activeChar.getTarget();
				if (target == null || !target.isPlayable())
				{
					activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
					return false;
				}

				Playable playable = (Playable) target;
				playable.updatePvPFlag(PvpFlagStatus.Enabled);
			}
			catch (Exception e)
			{
                LOGGER.Error(e);
				BuilderUtil.sendSysMessage(activeChar, "Usage: //set_pvp_flag");
			}
		}
		else if (command.startsWith("admin_setparam"))
		{
			WorldObject? target = activeChar.getTarget();
			if (target == null || !target.isCreature())
			{
				activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				return false;
			}
			StringTokenizer st = new StringTokenizer(command, " ");
			st.nextToken(); // admin_setparam
			if (!st.hasMoreTokens())
			{
				BuilderUtil.sendSysMessage(activeChar, "Syntax: //setparam <stat> <value>");
				return false;
			}
			string statName = st.nextToken();
			if (!st.hasMoreTokens())
			{
				BuilderUtil.sendSysMessage(activeChar, "Syntax: //setparam <stat> <value>");
				return false;
			}

			try
			{
				Stat? stat = null;
				foreach (Stat stats in EnumUtil.GetValues<Stat>())
				{
					if (statName.equalsIgnoreCase(stats.ToString()) || statName.equalsIgnoreCase(((int)stats).ToString()))
					{
						stat = stats;
						break;
					}
				}
				if (stat == null)
				{
					BuilderUtil.sendSysMessage(activeChar, "Couldn't find such stat!");
					return false;
				}

				double value = double.Parse(st.nextToken(), CultureInfo.InvariantCulture);
				Creature targetCreature = (Creature) target;
				if (value >= 0)
				{
					targetCreature.getStat().addFixedValue(stat.Value, value);
					targetCreature.getStat().recalculateStats(true);
					BuilderUtil.sendSysMessage(activeChar, "Fixed stat: " + stat + " has been set to " + value);
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "Non negative values are only allowed!");
				}
			}
			catch (Exception e)
			{
                LOGGER.Error(e);
				BuilderUtil.sendSysMessage(activeChar, "Syntax: //setparam <stat> <value>");
				return false;
			}
		}
		else if (command.startsWith("admin_unsetparam"))
		{
			WorldObject? target = activeChar.getTarget();
			if (target == null || !target.isCreature())
			{
				activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				return false;
			}
			StringTokenizer st = new StringTokenizer(command, " ");
			st.nextToken(); // admin_setparam
			if (!st.hasMoreTokens())
			{
				BuilderUtil.sendSysMessage(activeChar, "Syntax: //unsetparam <stat>");
				return false;
			}
			string statName = st.nextToken();
			Stat? stat = null;
			foreach (Stat stats in EnumUtil.GetValues<Stat>())
			{
				if (statName.equalsIgnoreCase(stats.ToString()) || statName.equalsIgnoreCase(((int)stats).ToString()))
				{
					stat = stats;
					break;
				}
			}
			if (stat == null)
			{
				BuilderUtil.sendSysMessage(activeChar, "Couldn't find such stat!");
				return false;
			}

			Creature targetCreature = (Creature) target;
			targetCreature.getStat().removeFixedValue(stat.Value);
			targetCreature.getStat().recalculateStats(true);
			BuilderUtil.sendSysMessage(activeChar, "Fixed stat: " + stat + " has been removed.");
		}
		return true;
	}

	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}

	private void listCharacters(Player activeChar, int page)
	{
		List<Player> players = new(World.getInstance().getPlayers());
		players.Sort((x, y) => -x.getUptime().CompareTo(y.getUptime()));

		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/charlist.htm", activeChar);
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);

		PageResult result = PageBuilder.newBuilder(players, 20, "bypass -h admin_show_characters").currentPage(page).bodyHandler((pages, player, sb) =>
		{
			sb.Append("<tr>");
			sb.Append("<td width=80><a action=\"bypass -h admin_character_info " + player.getName() + "\">" + (player.isInOfflineMode() ? "<font color=\"808080\">" + player.getName() + "</font>" : player.getName()) + "</a></td>");
			sb.Append("<td width=110>" + ClassListData.getInstance().getClass(player.getClassId()).getClientCode() + "</td><td width=40>" + player.getLevel() + "</td>");
			sb.Append("</tr>");
		}).build();

		if (result.getPages() > 0)
		{
			htmlContent.Replace("%pages%", "<table width=280 cellspacing=0><tr>" + result.getPagerTemplate() + "</tr></table>");
		}
		else
		{
			htmlContent.Replace("%pages%", "");
		}

		htmlContent.Replace("%players%", result.getBodyTemplate().ToString());
		activeChar.sendPacket(html);
	}

    private void ShowCharacterInfo(Player activeChar, Player? targetPlayer)
    {
        Player? player = targetPlayer;
        if (player == null)
        {
            WorldObject? target = activeChar.getTarget();
            if (target != null && target.isPlayer())
            {
                player = (Player)target;
            }
            else
            {
                return;
            }
        }
        else
        {
            activeChar.setTarget(player);
        }

        GatherCharacterInfo(activeChar, player, "charinfo.htm");
    }

    /**
     * Retrieve and replace player's info in filename htm file, sends it to activeChar as NpcHtmlMessagePacket.
     * @param activeChar
     * @param player
     * @param filename
     */
	private static void GatherCharacterInfo(Player activeChar, Player? player, string filename)
	{
		string ip = "N/A";
		if (player == null)
		{
			BuilderUtil.sendSysMessage(activeChar, "Player is null.");
			return;
		}

		GameSession? client = player.getClient();
		if (client == null)
		{
			BuilderUtil.sendSysMessage(activeChar, "Client is null.");
		}
		else if (client.IsDetached)
		{
			BuilderUtil.sendSysMessage(activeChar, "Client is detached.");
		}
		else
		{
			ip = client.IpAddress.ToString();
		}

		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/" + filename, activeChar);
		NpcHtmlMessagePacket adminReply = new NpcHtmlMessagePacket(null, 1, htmlContent);
		htmlContent.Replace("%name%", player.getName());
		htmlContent.Replace("%level%", player.getLevel().ToString());
		htmlContent.Replace("%clan%", player.getClan() != null ? "<a action=\"bypass -h admin_clan_info " + player.ObjectId + "\">" + player.getClan()?.getName() + "</a>" : null);
		htmlContent.Replace("%xp%", player.getExp().ToString());
		htmlContent.Replace("%sp%", player.getSp().ToString());
		htmlContent.Replace("%class%", ClassListData.getInstance().getClass(player.getClassId()).getClientCode());
		htmlContent.Replace("%ordinal%", player.getClassId().ToString());
		htmlContent.Replace("%classid%", player.getClassId().ToString());
		htmlContent.Replace("%baseclass%", ClassListData.getInstance().getClass(player.getBaseClass()).getClientCode());
		htmlContent.Replace("%x%", player.getX().ToString());
		htmlContent.Replace("%y%", player.getY().ToString());
		htmlContent.Replace("%z%", player.getZ().ToString());
		htmlContent.Replace("%heading%", player.getHeading().ToString());
		htmlContent.Replace("%currenthp%", ((int) player.getCurrentHp()).ToString());
		htmlContent.Replace("%maxhp%", player.getMaxHp().ToString());
		htmlContent.Replace("%reputation%", player.getReputation().ToString());
		htmlContent.Replace("%currentmp%", ((int) player.getCurrentMp()).ToString());
		htmlContent.Replace("%maxmp%", player.getMaxMp().ToString());
		htmlContent.Replace("%pvpflag%", player.getPvpFlag().ToString());
		htmlContent.Replace("%currentcp%", ((int) player.getCurrentCp()).ToString());
		htmlContent.Replace("%maxcp%", player.getMaxCp().ToString());
		htmlContent.Replace("%pvpkills%", player.getPvpKills().ToString());
		htmlContent.Replace("%pkkills%", player.getPkKills().ToString());
		htmlContent.Replace("%currentload%", player.getCurrentLoad().ToString());
		htmlContent.Replace("%maxload%", player.getMaxLoad().ToString());
		htmlContent.Replace("%percent%", (100.0 * player.getCurrentLoad() / player.getMaxLoad()).ToString());
		htmlContent.Replace("%patk%", player.getPAtk().ToString());
		htmlContent.Replace("%matk%", player.getMAtk().ToString());
		htmlContent.Replace("%pdef%", player.getPDef().ToString());
		htmlContent.Replace("%mdef%", player.getMDef().ToString());
		htmlContent.Replace("%accuracy%", player.getAccuracy().ToString());
		htmlContent.Replace("%evasion%", player.getEvasionRate().ToString());
		htmlContent.Replace("%critical%", player.getCriticalHit().ToString());
		htmlContent.Replace("%runspeed%", player.getRunSpeed().ToString());
		htmlContent.Replace("%patkspd%", player.getPAtkSpd().ToString());
		htmlContent.Replace("%matkspd%", player.getMAtkSpd().ToString());
		htmlContent.Replace("%hpregen%", player.getStat().getHpRegen().ToString());
		htmlContent.Replace("%mpregen%", player.getStat().getMpRegen().ToString());
		htmlContent.Replace("%cpregen%", player.getStat().getCpRegen().ToString());
		htmlContent.Replace("%access%", player.getAccessLevel().Level + " (" + player.getAccessLevel().Name + ")");
		htmlContent.Replace("%account%", player.getAccountName());
		htmlContent.Replace("%ip%", ip);
		htmlContent.Replace("%protocol%", client != null ? client.ProtocolVersion.ToString() : "NULL");
		htmlContent.Replace("%hwid%", client != null && client.HardwareInfo != null ? client.HardwareInfo.getMacAddress() : "Unknown");
		htmlContent.Replace("%ai%", player.getAI().getIntention().ToString());
		htmlContent.Replace("%inst%", player.isInInstance() ? " " + player.getInstanceWorld()?.getName() + "</td><td><button value=\"Go\" action=\"bypass -h admin_instanceteleport " + player.getInstanceId() + "\"width=60 height=20 back=\"L2UI_CT1.Button_DF_Down\" fore=\"L2UI_CT1.Button_DF\">" : "NONE");
		htmlContent.Replace("%noblesse%", player.isNoble() ? "Yes" : "No");
		activeChar.sendPacket(adminReply);
	}

	private static void SetTargetReputation(Player activeChar, int value)
	{
		WorldObject? target = activeChar.getTarget();
		Player? player;
		if (target != null && target.isPlayer())
		{
			player = (Player) target;
		}
		else
		{
			return;
		}

		int newReputation = value;
		if (newReputation > Config.Pvp.MAX_REPUTATION)
		{
			newReputation = Config.Pvp.MAX_REPUTATION;
		}

		int oldReputation = player.getReputation();
		player.setReputation(newReputation);
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOUR_REPUTATION_HAS_BEEN_CHANGED_TO_S1);
		sm.Params.addInt(newReputation);
		player.sendPacket(sm);

        BuilderUtil.sendSysMessage(activeChar,
            "Successfully Changed karma for " + player.getName() + " from (" + oldReputation + ") to (" +
            newReputation + ").");
    }

	private static void EditCharacter(Player activeChar, string? targetName)
	{
		WorldObject? target;
		if (targetName != null)
		{
			target = World.getInstance().getPlayer(targetName);
		}
		else
		{
			target = activeChar.getTarget();
		}

		if (target != null && target.isPlayer())
		{
			Player player = (Player)target;
			GatherCharacterInfo(activeChar, player, "charedit.htm");
		}
	}

	/**
	 * @param activeChar
	 * @param characterToFind
	 */
	private void findCharacter(Player activeChar, string characterToFind)
	{
		int charactersFound = 0;
		string name;

		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/charfind.htm", activeChar);
		NpcHtmlMessagePacket adminReply = new NpcHtmlMessagePacket(null, 1, htmlContent);

		StringBuilder replyMSG = new StringBuilder(1000);
		List<Player> players = new(World.getInstance().getPlayers().OrderByDescending(p=>p.getUptime()));
		foreach (Player player in players)
		{ // Add player info into new Table row
			name = player.getName();
			if (name.toLowerCase().contains(characterToFind.toLowerCase()))
			{
				charactersFound += 1;
				replyMSG.Append("<tr><td width=80><a action=\"bypass -h admin_character_info ");
				replyMSG.Append(name);
				replyMSG.Append("\">");
				replyMSG.Append(name);
				replyMSG.Append("</a></td><td width=110>");
				replyMSG.Append(ClassListData.getInstance().getClass(player.getClassId()).getClientCode());
				replyMSG.Append("</td><td width=40>");
				replyMSG.Append(player.getLevel());
				replyMSG.Append("</td></tr>");
			}
			if (charactersFound > 20)
			{
				break;
			}
		}
		htmlContent.Replace("%results%", replyMSG.ToString());

		string replyMSG2;
		if (charactersFound == 0)
		{
			replyMSG2 = "s. Please try again.";
		}
		else if (charactersFound > 20)
		{
			htmlContent.Replace("%number%", " more than 20");
			replyMSG2 = "s.<br>Please refine your search to see all of the results.";
		}
		else if (charactersFound == 1)
		{
			replyMSG2 = ".";
		}
		else
		{
			replyMSG2 = "s.";
		}

		htmlContent.Replace("%number%", charactersFound.ToString());
		htmlContent.Replace("%end%", replyMSG2);
		activeChar.sendPacket(adminReply);
	}

	/**
	 * @param activeChar
	 * @param ipAdress
	 */
	private void findCharactersPerIp(Player activeChar, string ipAdress)
	{
		bool findDisconnected = false;
		if (ipAdress.equals("disconnected"))
		{
			findDisconnected = true;
		}
		else if (!Regex.IsMatch(ipAdress, "^(?:(?:[0-9]|[1-9][0-9]|1[0-9][0-9]|2(?:[0-4][0-9]|5[0-5]))\\.){3}(?:[0-9]|[1-9][0-9]|1[0-9][0-9]|2(?:[0-4][0-9]|5[0-5]))$"))
		{
			throw new ArgumentException("Malformed IPv4 number");
		}

		int charactersFound = 0;
		GameSession? client;
		string ip = "0.0.0.0";
		StringBuilder replyMSG = new StringBuilder(1000);

		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/ipfind.htm", activeChar);
		NpcHtmlMessagePacket adminReply = new NpcHtmlMessagePacket(null, 1, htmlContent);

		List<Player> players = new(World.getInstance().getPlayers().OrderByDescending(p => p.getUptime()));
		foreach (Player player in players)
		{
			client = player.getClient();
			if (client == null)
			{
				continue;
			}

			if (client.IsDetached)
			{
				if (!findDisconnected)
				{
					continue;
				}
			}
			else
			{
				if (findDisconnected)
				{
					continue;
				}

				ip = client.IpAddress.ToString();
				if (!ip.equals(ipAdress))
				{
					continue;
				}
			}

			string name = player.getName();
			charactersFound += 1;
			replyMSG.Append("<tr><td width=80><a action=\"bypass -h admin_character_info ");
			replyMSG.Append(name);
			replyMSG.Append("\">");
			replyMSG.Append(name);
			replyMSG.Append("</a></td><td width=110>");
			replyMSG.Append(ClassListData.getInstance().getClass(player.getClassId()).getClientCode());
			replyMSG.Append("</td><td width=40>");
			replyMSG.Append(player.getLevel());
			replyMSG.Append("</td></tr>");

			if (charactersFound > 20)
			{
				break;
			}
		}
		htmlContent.Replace("%results%", replyMSG.ToString());

		string replyMSG2;
		if (charactersFound == 0)
		{
			replyMSG2 = "s. Maybe they got d/c? :)";
		}
		else if (charactersFound > 20)
		{
			htmlContent.Replace("%number%", " more than " + charactersFound);
			replyMSG2 = "s.<br>In order to avoid you a client crash I won't <br1>display results beyond the 20th character.";
		}
		else if (charactersFound == 1)
		{
			replyMSG2 = ".";
		}
		else
		{
			replyMSG2 = "s.";
		}
		htmlContent.Replace("%ip%", ipAdress);
		htmlContent.Replace("%number%", charactersFound.ToString());
		htmlContent.Replace("%end%", replyMSG2);
		activeChar.sendPacket(adminReply);
	}

	/**
	 * @param activeChar
	 * @param characterName
	 */
	private static void FindCharactersPerAccount(Player activeChar, string characterName)
	{
		Player? player = World.getInstance().getPlayer(characterName);
		if (player == null)
		{
			throw new ArgumentException("Player doesn't exist");
		}

		Map<int, string> chars = player.getAccountChars();
		string replyMSG = string.Join("<br1>", chars.Values);

		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/accountinfo.htm", activeChar);
		NpcHtmlMessagePacket adminReply = new NpcHtmlMessagePacket(null, 1, htmlContent);
		htmlContent.Replace("%account%", player.getAccountName());
		htmlContent.Replace("%player%", characterName);
		htmlContent.Replace("%characters%", replyMSG);
		activeChar.sendPacket(adminReply);
	}

	/**
	 * @param activeChar
	 * @param multibox
	 */
	private void findDualbox(Player activeChar, int multibox)
	{
		Map<string, List<Player>> ipMap = new();
        Map<string, int> dualboxIPs = new();
		List<Player> players = new(World.getInstance().getPlayers().OrderByDescending(p=>p.getUptime()));
		foreach (Player player in players)
		{
			GameSession? client = player.getClient();
			if (client == null || client.IsDetached)
			{
				continue;
			}

			string ip = client.IpAddress.ToString();
            List<Player> list = ipMap.GetOrAdd(ip, _ => []);
			list.Add(player);

			if (list.Count >= multibox)
            {
                dualboxIPs.AddOrUpdate(ip, _ => multibox, (_, oldValue) => oldValue + 1);
			}
		}

		List<string> keys = new(dualboxIPs.Keys.OrderByDescending(r => dualboxIPs[r]));

		StringBuilder results = new StringBuilder();
		foreach (string dualboxIP in keys)
		{
			results.Append("<a action=\"bypass -h admin_find_ip " + dualboxIP + "\">" + dualboxIP + " (" + dualboxIPs.get(dualboxIP) + ")</a><br1>");
		}

		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/dualbox.htm", activeChar);
		NpcHtmlMessagePacket adminReply = new NpcHtmlMessagePacket(null, 1, htmlContent);
		htmlContent.Replace("%multibox%", multibox.ToString());
		htmlContent.Replace("%results%", results.ToString());
		htmlContent.Replace("%strict%", "");
		activeChar.sendPacket(adminReply);
	}

	private void findDualboxStrict(Player activeChar, int multibox)
	{
		Map<IpPack, List<Player>> ipMap = new();
		GameSession? client;
		Map<IpPack, int> dualboxIPs = new();
		List<Player> players = new(World.getInstance().getPlayers().OrderByDescending(p=>p.getUptime()));
		foreach (Player player in players)
		{
			client = player.getClient();
			if (client == null || client.IsDetached)
			{
				continue;
			}

			IpPack pack = new IpPack(client.IpAddress.ToString(), null /*client.getTrace()*/);

            List<Player> list = ipMap.GetOrAdd(pack, _ => []);
			list.Add(player);

			if (list.Count >= multibox)
			{
                dualboxIPs.AddOrUpdate(pack, _ => multibox, (_, oldValue) => oldValue + 1);
			}
		}

		List<IpPack> keys = new(dualboxIPs.Keys.OrderByDescending(x => dualboxIPs[x]));

		StringBuilder results = new StringBuilder();
		foreach (IpPack dualboxIP in keys)
		{
			results.Append("<a action=\"bypass -h admin_find_ip " + dualboxIP.ip + "\">" + dualboxIP.ip + " (" + dualboxIPs.get(dualboxIP) + ")</a><br1>");
		}

		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/dualbox.htm", activeChar);
		NpcHtmlMessagePacket adminReply = new NpcHtmlMessagePacket(null, 1, htmlContent);
		htmlContent.Replace("%multibox%", multibox.ToString());
		htmlContent.Replace("%results%", results.ToString());
		htmlContent.Replace("%strict%", "strict_");
		activeChar.sendPacket(adminReply);
	}

	private class IpPack
	{
		public string ip;
		//public int[][] tracert;

		public IpPack(string ip, int[][]? tracert)
		{
			this.ip = ip;
			//this.tracert = tracert;
		}

		public override int GetHashCode()
		{
			int prime = 31;
			int result = 1;
			result = prime * result + (ip == null ? 0 : ip.GetHashCode());
			// foreach (int[] array in tracert)
			// {
			// 	result = (prime * result) + Arrays.hashCode(array);
			// }

			return result;
		}

		public override bool Equals(object? obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (GetType() != obj.GetType())
			{
				return false;
			}
			IpPack other = (IpPack) obj;
			if (ip == null)
			{
				if (other.ip != null)
				{
					return false;
				}
			}
			else if (!ip.equals(other.ip))
			{
				return false;
			}
			// for (int i = 0; i < tracert.Length; i++)
			// {
			// 	for (int o = 0; o < tracert[0].Length; o++)
			// 	{
			// 		if (tracert[i][o] != other.tracert[i][o])
			// 		{
			// 			return false;
			// 		}
			// 	}
			// }
			return true;
		}
	}

	private void gatherSummonInfo(Summon target, Player activeChar)
	{
		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/petinfo.htm", activeChar);
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);
		string name = target.getName();
		htmlContent.Replace("%name%", name == null ? "N/A" : name);
		htmlContent.Replace("%level%", target.getLevel().ToString());
		htmlContent.Replace("%exp%", target.getStat().getExp().ToString());
		string owner = target.getActingPlayer().getName();
		htmlContent.Replace("%owner%", " <a action=\"bypass -h admin_character_info " + owner + "\">" + owner + "</a>");
		htmlContent.Replace("%class%", target.GetType().Name);
		htmlContent.Replace("%ai%", target.hasAI() ? target.getAI().getIntention().ToString() : "NULL");
		htmlContent.Replace("%hp%", (int) target.getStatus().getCurrentHp() + "/" + target.getStat().getMaxHp());
		htmlContent.Replace("%mp%", (int) target.getStatus().getCurrentMp() + "/" + target.getStat().getMaxMp());
		htmlContent.Replace("%karma%", target.getReputation().ToString());
		htmlContent.Replace("%race%", target.getTemplate().getRace().ToString());
		if (target.isPet())
		{
			int objId = target.getActingPlayer().ObjectId;
			htmlContent.Replace("%inv%", " <a action=\"bypass admin_show_pet_inv " + objId + "\">view</a>");
		}
		else
		{
			htmlContent.Replace("%inv%", "none");
		}
		if (target.isPet())
		{
			htmlContent.Replace("%food%", ((Pet) target).getCurrentFed() + "/" + ((Pet)target).getPetLevelData().getPetMaxFeed());
			htmlContent.Replace("%load%", target.getInventory()?.getTotalWeight() + "/" + target.getMaxLoad());
		}
		else
		{
			htmlContent.Replace("%food%", "N/A");
			htmlContent.Replace("%load%", "N/A");
		}
		activeChar.sendPacket(html);
	}

	private void gatherPartyInfo(Player target, Player activeChar)
	{
        Party? party = target.getParty();
		bool color = true;
		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/partyinfo.htm", activeChar);
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);
		StringBuilder text = new StringBuilder(400);
        if (party != null)
        {
            foreach (Player member in party.getMembers())
            {
                if (color)
                {
                    text.Append(
                        "<tr><td><table width=270 border=0 bgcolor=131210 cellpadding=2><tr><td width=30 align=right>");
                }
                else
                {
                    text.Append("<tr><td><table width=270 border=0 cellpadding=2><tr><td width=30 align=right>");
                }

                text.Append(member.getLevel() + "</td><td width=130><a action=\"bypass -h admin_character_info " +
                    member.getName() + "\">" + member.getName() + "</a>");

                text.Append("</td><td width=110 align=right>" + member.getClassId() + "</td></tr></table></td></tr>");
                color = !color;
            }
        }

        htmlContent.Replace("%player%", target.getName());
		htmlContent.Replace("%party%", text.ToString());
		activeChar.sendPacket(html);
	}
}
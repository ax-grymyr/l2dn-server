using System.Globalization;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author NosBit
 */
public class AdminReload: IAdminCommandHandler
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AdminReload));
	
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_reload",
    ];
	
	private const string RELOAD_USAGE = "Usage: //reload <config|access|npc|quest [quest_id|quest_name]|walker|htm[l] [file|directory]|multisell|buylist|teleport|skill|item|door|effect|handler|enchant|options|fishing>";
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command, " ");
		string actualCommand = st.nextToken();
		if (actualCommand.equalsIgnoreCase("admin_reload"))
		{
			if (!st.hasMoreTokens())
			{
				AdminHtml.showAdminHtml(activeChar, "reload.htm");
				activeChar.sendMessage(RELOAD_USAGE);
				return true;
			}
			
			string type = st.nextToken();
			switch (type.toLowerCase())
			{
				case "config":
				{
					Config.Load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Configs.");
					break;
				}
				case "access":
				{
					AdminData.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Access.");
					break;
				}
				case "npc":
				{
					NpcData.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Npcs.");
					break;
				}
				case "quest":
				{
					if (st.hasMoreElements())
					{
						string value = st.nextToken();
						if (!int.TryParse(value, CultureInfo.InvariantCulture, out int questId))
						{
							QuestManager.getInstance().reload(value);
							AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Quest Name:" + value + ".");
						}
						else
						{
							QuestManager.getInstance().reload(questId);
							AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Quest ID:" + questId + ".");
						}
					}
					else
					{
						QuestManager.getInstance().reloadAllScripts();
						BuilderUtil.sendSysMessage(activeChar, "All scripts have been reloaded.");
						AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Quests.");
					}
					break;
				}
				case "walker":
				{
					WalkingManager.getInstance().load();
					BuilderUtil.sendSysMessage(activeChar, "All walkers have been reloaded");
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Walkers.");
					break;
				}
				case "htm":
				case "html":
				{
					if (st.hasMoreElements())
					{
						string path = st.nextToken();
						string filePath = Path.Combine(Config.DATAPACK_ROOT_PATH, "html/" + path);
						if (File.Exists(filePath))
						{
							HtmCache.getInstance().reload(filePath);
							AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Htm File:" + filePath + ".");
						}
						else
						{
							BuilderUtil.sendSysMessage(activeChar, "File or Directory does not exist.");
						}
					}
					else
					{
						HtmCache.getInstance().reload();

						double memoryUsage = HtmCache.getInstance().getMemoryUsage() / 1048576.0;
						BuilderUtil.sendSysMessage(activeChar,
							"Cache[HTML]: " + memoryUsage + " megabytes on " +
							HtmCache.getInstance().getLoadedFiles() + " files loaded");
						
						AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Htms.");
					}
					break;
				}
				case "multisell":
				{
					MultisellData.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Multisells.");
					break;
				}
				case "buylist":
				{
					BuyListData.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Buylists.");
					break;
				}
				case "teleport":
				{
					TeleporterData.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Teleports.");
					break;
				}
				case "skill":
				{
					SkillData.getInstance().Reload();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Skills.");
					break;
				}
				case "item":
				{
					ItemData.getInstance().reload();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Items.");
					break;
				}
				case "door":
				{
					DoorData.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Doors.");
					break;
				}
				case "zone":
				{
					ZoneManager.getInstance().reload();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Zones.");
					break;
				}
				case "cw":
				{
					CursedWeaponsManager.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Cursed Weapons.");
					break;
				}
				case "crest":
				{
					CrestTable.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Crests.");
					break;
				}
				case "effect":
				{
					try
					{
						// TODO ScriptEngineManager.getInstance().executeScript(ScriptEngineManager.EFFECT_MASTER_HANDLER_FILE);
						AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded effect master handler.");
					}
					catch (Exception e)
					{
						LOGGER.Warn("Failed executing effect master handler! " + e);
						BuilderUtil.sendSysMessage(activeChar, "Error reloading effect master handler!");
					}
					break;
				}
				case "handler":
				{
					try
					{
						// TODO ScriptEngineManager.getInstance().executeScript(ScriptEngineManager.MASTER_HANDLER_FILE);
						AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded master handler.");
					}
					catch (Exception e)
					{
						LOGGER.Warn("Failed executing master handler! " + e);
						BuilderUtil.sendSysMessage(activeChar, "Error reloading master handler!");
					}
					break;
				}
				case "enchant":
				{
					EnchantItemOptionsData.getInstance().load();
					EnchantItemGroupsData.getInstance().load();
					EnchantItemData.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded item enchanting data.");
					break;
				}
				case "transform":
				{
					TransformData.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded transform data.");
					break;
				}
				case "crystalizable":
				{
					ItemCrystallizationData.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded item crystalization data.");
					break;
				}
				case "primeshop":
				{
					PrimeShopData.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Prime Shop data.");
					break;
				}
				case "limitshop":
				{
					LimitShopData.getInstance().load();
					LimitShopCraftData.getInstance().load();
					LimitShopClanData.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Limit Shop data.");
					break;
				}
				case "appearance":
				{
					AppearanceItemData.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded appearance item data.");
					break;
				}
				case "sayune":
				{
					SayuneData.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Sayune data.");
					break;
				}
				case "sets":
				{
					ArmorSetData.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Armor sets data.");
					break;
				}
				case "options":
				{
					OptionData.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Options data.");
					break;
				}
				case "fishing":
				{
					FishingData.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Fishing data.");
					break;
				}
				case "attendance":
				{
					AttendanceRewardData.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Attendance Reward data.");
					break;
				}
				case "fakeplayers":
				{
					FakePlayerData.getInstance().load();
					foreach (WorldObject obj in World.getInstance().getVisibleObjects())
					{
						if (obj.isFakePlayer())
						{
							obj.broadcastInfo();
						}
					}
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Fake Player data.");
					break;
				}
				case "fakeplayerchat":
				{
					FakePlayerChatManager.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Fake Player Chat data.");
					break;
				}
				case "localisations":
				{
					//SystemMessageId.loadLocalisations();
					//NpcStringId.loadLocalisations();
					SendMessageLocalisationData.getInstance().load();
					NpcNameLocalisationData.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Localisation data.");
					break;
				}
				case "instance":
				{
					InstanceManager.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Instances data.");
					break;
				}
				case "combination":
				{
					CombinationItemsData.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Combination data.");
					break;
				}
				case "equipmentupgrade":
				{
					EquipmentUpgradeData.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Equipment Upgrade data.");
					break;
				}
				case "randomcraft":
				{
					RandomCraftData.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Random Craft data.");
					break;
				}
				case "variation":
				{
					VariationData.getInstance().load();
					AdminData.getInstance().broadcastMessageToGMs(activeChar.getName() + ": Reloaded Variation data.");
					break;
				}
				default:
				{
					activeChar.sendMessage(RELOAD_USAGE);
					return true;
				}
			}
			BuilderUtil.sendSysMessage(activeChar, "WARNING: There are several known issues regarding this feature. Reloading server data during runtime is STRONGLY NOT RECOMMENDED for live servers, just for developing environments.");
		}
		return true;
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}

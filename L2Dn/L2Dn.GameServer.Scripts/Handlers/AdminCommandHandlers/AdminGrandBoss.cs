using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Quests;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author St3eT
 */
public class AdminGrandBoss: IAdminCommandHandler
{
	private const int ANTHARAS = 29068; // Antharas
	private const int ANTHARAS_ZONE = 70050; // Antharas Nest
	private const int VALAKAS = 29028; // Valakas
	private const int BAIUM = 29020; // Baium
	private const int BAIUM_ZONE = 70051; // Baium Nest
	private const int QUEENANT = 29001; // Queen Ant
	private const int ORFEN = 29014; // Orfen
	private const int CORE = 29006; // Core
	
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_grandboss",
		"admin_grandboss_skip",
		"admin_grandboss_respawn",
		"admin_grandboss_minions",
		"admin_grandboss_abort",
	};
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command, " ");
		string actualCommand = st.nextToken();
		switch (actualCommand.toLowerCase())
		{
			case "admin_grandboss":
			{
				if (st.hasMoreTokens())
				{
					int grandBossId = int.Parse(st.nextToken());
					manageHtml(activeChar, grandBossId);
				}
				else
				{
					HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/grandboss/grandboss.htm", activeChar);
					NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);
					activeChar.sendPacket(html);
				}
				break;
			}
			
			case "admin_grandboss_skip":
			{
				if (st.hasMoreTokens())
				{
					int grandBossId = int.Parse(st.nextToken());
					if (grandBossId == ANTHARAS)
					{
						antharasAi().notifyEvent("SKIP_WAITING", null, activeChar);
						manageHtml(activeChar, grandBossId);
					}
					else
					{
						BuilderUtil.sendSysMessage(activeChar, "Wrong ID!");
					}
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: //grandboss_skip Id");
				}
				break;
			}
			case "admin_grandboss_respawn":
			{
				if (st.hasMoreTokens())
				{
					int grandBossId = int.Parse(st.nextToken());
					
					switch (grandBossId)
					{
						case ANTHARAS:
						{
							antharasAi().notifyEvent("RESPAWN_ANTHARAS", null, activeChar);
							manageHtml(activeChar, grandBossId);
							break;
						}
						case BAIUM:
						{
							baiumAi().notifyEvent("RESPAWN_BAIUM", null, activeChar);
							manageHtml(activeChar, grandBossId);
							break;
						}
						default:
						{
							BuilderUtil.sendSysMessage(activeChar, "Wrong ID!");
							break;
						}
					}
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: //grandboss_respawn Id");
				}
				break;
			}
			case "admin_grandboss_minions":
			{
				if (st.hasMoreTokens())
				{
					int grandBossId = int.Parse(st.nextToken());
					
					switch (grandBossId)
					{
						case ANTHARAS:
						{
							antharasAi().notifyEvent("DESPAWN_MINIONS", null, activeChar);
							break;
						}
						case BAIUM:
						{
							baiumAi().notifyEvent("DESPAWN_MINIONS", null, activeChar);
							break;
						}
						default:
						{
							BuilderUtil.sendSysMessage(activeChar, "Wrong ID!");
							break;
						}
					}
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: //grandboss_minions Id");
				}
				break;
			}
			case "admin_grandboss_abort":
			{
				if (st.hasMoreTokens())
				{
					int grandBossId = int.Parse(st.nextToken());
					
					switch (grandBossId)
					{
						case ANTHARAS:
						{
							antharasAi().notifyEvent("ABORT_FIGHT", null, activeChar);
							manageHtml(activeChar, grandBossId);
							break;
						}
						case BAIUM:
						{
							baiumAi().notifyEvent("ABORT_FIGHT", null, activeChar);
							manageHtml(activeChar, grandBossId);
							break;
						}
						default:
						{
							BuilderUtil.sendSysMessage(activeChar, "Wrong ID!");
							break;
						}
					}
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: //grandboss_abort Id");
				}
			}
				break;
		}
		return true;
	}
	
	private void manageHtml(Player activeChar, int grandBossId)
	{
		List<int> list = [ANTHARAS, VALAKAS, BAIUM, QUEENANT, ORFEN, CORE]; 
		if (list.Contains(grandBossId))
		{
			int bossStatus = GrandBossManager.getInstance().getStatus(grandBossId);
			NoRestartZone bossZone = null;
			string textColor = null;
			string text = null;
			string htmlPatch = null;
			int deadStatus = 0;
			
			switch (grandBossId)
			{
				case ANTHARAS:
				{
					bossZone = ZoneManager.getInstance().getZoneById<NoRestartZone>(ANTHARAS_ZONE);
					htmlPatch = "html/admin/grandboss/grandboss_antharas.htm";
					break;
				}
				case VALAKAS:
				{
					htmlPatch = "html/admin/grandboss/grandboss_valakas.htm";
					break;
				}
				case BAIUM:
				{
					bossZone = ZoneManager.getInstance().getZoneById<NoRestartZone>(BAIUM_ZONE);
					htmlPatch = "html/admin/grandboss/grandboss_baium.htm";
					break;
				}
				case QUEENANT:
				{
					htmlPatch = "html/admin/grandboss/grandboss_queenant.htm";
					break;
				}
				case ORFEN:
				{
					htmlPatch = "html/admin/grandboss/grandboss_orfen.htm";
					break;
				}
				case CORE:
				{
					htmlPatch = "html/admin/grandboss/grandboss_core.htm";
					break;
				}
			}
			
			list = [ANTHARAS, VALAKAS, BAIUM];
			if (list.Contains(grandBossId))
			{
				deadStatus = 3;
				switch (bossStatus)
				{
					case 0:
					{
						textColor = "00FF00"; // Green
						text = "Alive";
						break;
					}
					case 1:
					{
						textColor = "FFFF00"; // Yellow
						text = "Waiting";
						break;
					}
					case 2:
					{
						textColor = "FF9900"; // Orange
						text = "In Fight";
						break;
					}
					case 3:
					{
						textColor = "FF0000"; // Red
						text = "Dead";
						break;
					}
					default:
					{
						textColor = "FFFFFF"; // White
						text = "Unk " + bossStatus;
						break;
					}
				}
			}
			else
			{
				deadStatus = 1;
				switch (bossStatus)
				{
					case 0:
					{
						textColor = "00FF00"; // Green
						text = "Alive";
						break;
					}
					case 1:
					{
						textColor = "FF0000"; // Red
						text = "Dead";
						break;
					}
					default:
					{
						textColor = "FFFFFF"; // White
						text = "Unk " + bossStatus;
						break;
					}
				}
			}
			
			StatSet info = GrandBossManager.getInstance().getStatSet(grandBossId);
			string bossRespawn = info.getDateTime("respawn_time").ToString("yyyy-MM-dd HH:mm:ss");

			HtmlContent htmlContent = HtmlContent.LoadFromFile(htmlPatch, activeChar);
			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(0, 1, htmlContent);
			htmlContent.Replace("%bossStatus%", text);
			htmlContent.Replace("%bossColor%", textColor);
			htmlContent.Replace("%respawnTime%", bossStatus == deadStatus ? bossRespawn : "Already respawned!");
			htmlContent.Replace("%playersInside%", bossZone != null ? bossZone.getPlayersInside().Count.ToString() : "Zone not found!");
			activeChar.sendPacket(html);
		}
		else
		{
			BuilderUtil.sendSysMessage(activeChar, "Wrong ID!");
		}
	}
	
	private Quest antharasAi()
	{
		return null;
		// return QuestManager.getInstance().getQuest(Antharas.class.getSimpleName());
	}
	
	private Quest baiumAi()
	{
		return null;
		// return QuestManager.getInstance().getQuest(Baium.class.getSimpleName());
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}

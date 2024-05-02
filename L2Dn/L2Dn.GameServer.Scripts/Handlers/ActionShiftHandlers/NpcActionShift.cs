using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Quests;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Model.Spawns;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Scripts.Handlers.BypassHandlers;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.ActionShiftHandlers;

public class NpcActionShift: IActionShiftHandler
{
	public bool action(Player player, WorldObject target, bool interact)
	{
		// Check if the Player is a GM
		if (player.isGM())
		{
			// Set the target of the Player player
			player.setTarget(target);
			
			Npc npc = (Npc) target;
			ClanHall clanHall = ClanHallData.getInstance().getClanHallByNpcId(npc.getId());

			HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/npcinfo.htm", player);
			htmlContent.Replace("%objid%", target.getObjectId().ToString());
			htmlContent.Replace("%class%", npc.GetType().Name);
			htmlContent.Replace("%race%", npc.getTemplate().getRace().ToString());
			htmlContent.Replace("%id%", npc.getTemplate().getId().ToString());
			htmlContent.Replace("%lvl%", npc.getTemplate().getLevel().ToString());
			htmlContent.Replace("%name%", npc.getTemplate().getName());
			htmlContent.Replace("%tmplid%", npc.getTemplate().getId().ToString());
			htmlContent.Replace("%aggro%", (target.isAttackable() ? ((Attackable) target).getAggroRange() : 0).ToString());
			htmlContent.Replace("%hp%", ((int) npc.getCurrentHp()).ToString());
			htmlContent.Replace("%hpmax%", npc.getMaxHp().ToString());
			htmlContent.Replace("%mp%", ((int) npc.getCurrentMp()).ToString());
			htmlContent.Replace("%mpmax%", npc.getMaxMp().ToString());
			htmlContent.Replace("%exp%", npc.getTemplate().getExp().ToString());
			htmlContent.Replace("%sp%", npc.getTemplate().getSP().ToString());
			
			htmlContent.Replace("%patk%", npc.getPAtk().ToString());
			htmlContent.Replace("%matk%", npc.getMAtk().ToString());
			htmlContent.Replace("%pdef%", npc.getPDef().ToString());
			htmlContent.Replace("%mdef%", npc.getMDef().ToString());
			htmlContent.Replace("%accu%", npc.getAccuracy().ToString());
			htmlContent.Replace("%evas%", npc.getEvasionRate().ToString());
			htmlContent.Replace("%crit%", npc.getCriticalHit().ToString());
			htmlContent.Replace("%rspd%", npc.getRunSpeed().ToString());
			htmlContent.Replace("%aspd%", npc.getPAtkSpd().ToString());
			htmlContent.Replace("%cspd%", npc.getMAtkSpd().ToString());
			htmlContent.Replace("%atkType%", npc.getTemplate().getBaseAttackType().ToString());
			htmlContent.Replace("%atkRng%", npc.getTemplate().getBaseAttackRange().ToString());
			htmlContent.Replace("%str%", npc.getSTR().ToString());
			htmlContent.Replace("%dex%", npc.getDEX().ToString());
			htmlContent.Replace("%con%", npc.getCON().ToString());
			htmlContent.Replace("%int%", npc.getINT().ToString());
			htmlContent.Replace("%wit%", npc.getWIT().ToString());
			htmlContent.Replace("%men%", npc.getMEN().ToString());
			htmlContent.Replace("%loc%", target.getX() + " " + target.getY() + " " + target.getZ());
			htmlContent.Replace("%heading%", npc.getHeading().ToString());
			htmlContent.Replace("%collision_radius%", npc.getTemplate().getFCollisionRadius().ToString());
			htmlContent.Replace("%collision_height%", npc.getTemplate().getFCollisionHeight().ToString());
			htmlContent.Replace("%clanHall%", clanHall != null ? clanHall.getName() : "none");
			htmlContent.Replace("%mpRewardValue%", npc.getTemplate().getMpRewardValue().ToString());
			htmlContent.Replace("%mpRewardTicks%", npc.getTemplate().getMpRewardTicks().ToString());
			htmlContent.Replace("%mpRewardType%", npc.getTemplate().getMpRewardType().ToString());
			htmlContent.Replace("%mpRewardAffectType%", npc.getTemplate().getMpRewardAffectType().ToString());
			htmlContent.Replace("%loc2d%", ((int) player.calculateDistance2D(npc.getLocation().ToLocation2D())).ToString());
			htmlContent.Replace("%loc3d%", ((int) player.calculateDistance3D(npc.getLocation().ToLocation3D())).ToString());
			
			AttributeType attackAttribute = npc.getAttackElement();
			htmlContent.Replace("%ele_atk%", attackAttribute.ToString());
			htmlContent.Replace("%ele_atk_value%", npc.getAttackElementValue(attackAttribute).ToString());
			htmlContent.Replace("%ele_dfire%", npc.getDefenseElementValue(AttributeType.FIRE).ToString());
			htmlContent.Replace("%ele_dwater%", npc.getDefenseElementValue(AttributeType.WATER).ToString());
			htmlContent.Replace("%ele_dwind%", npc.getDefenseElementValue(AttributeType.WIND).ToString());
			htmlContent.Replace("%ele_dearth%", npc.getDefenseElementValue(AttributeType.EARTH).ToString());
			htmlContent.Replace("%ele_dholy%", npc.getDefenseElementValue(AttributeType.HOLY).ToString());
			htmlContent.Replace("%ele_ddark%", npc.getDefenseElementValue(AttributeType.DARK).ToString());
			
			Spawn spawn = npc.getSpawn();
			if (spawn != null)
			{
				NpcSpawnTemplate template = spawn.getNpcSpawnTemplate();
				if (template != null)
				{
					String fileName = template.getSpawnTemplate().getFile().Replace('\\', '/');
				
					htmlContent.Replace("%spawnfile%", fileName.Replace("spawns/", ""));
					htmlContent.Replace("%spawnname%", template.getSpawnTemplate().getName() ?? string.Empty);
					htmlContent.Replace("%spawngroup%", template.getGroup()?.getName() ?? string.Empty);
					if (template.getSpawnTemplate().getAI() != null)
					{
						Quest script = QuestManager.getInstance().getQuest(template.getSpawnTemplate().getAI());
						if (script != null)
						{
							htmlContent.Replace("%spawnai%", "<a action=\"bypass -h admin_quest_info " + script.Name + "\"><font color=\"LEVEL\">" + script.Name + "</font></a>");
						}
					}
					htmlContent.Replace("%spawnai%", "<font color=FF0000>" + template.getSpawnTemplate().getAI() + "</font>");
				}

				htmlContent.Replace("%spawn%",
					(template != null ? template.getSpawnLocation().getX() : npc.getSpawn().Location.getX()) + " " +
					(template != null ? template.getSpawnLocation().getY() : npc.getSpawn().Location.getY()) + " " +
					(template != null ? template.getSpawnLocation().getZ() : npc.getSpawn().Location.getZ()));

				if (npc.getSpawn().getRespawnMinDelay() == TimeSpan.Zero)
				{
					htmlContent.Replace("%resp%", "None");
				}
				else if (npc.getSpawn().hasRespawnRandom())
				{
					htmlContent.Replace("%resp%", (npc.getSpawn().getRespawnMinDelay() / 1000) + "-" + (npc.getSpawn().getRespawnMaxDelay() / 1000) + " sec");
				}
				else
				{
					htmlContent.Replace("%resp%", (npc.getSpawn().getRespawnMinDelay() / 1000) + " sec");
				}
				htmlContent.Replace("%chaseRange%", npc.getSpawn().getChaseRange().ToString());
			}
			else
			{
				htmlContent.Replace("%spawn%", "<font color=FF0000>null</font>");
				htmlContent.Replace("%resp%", "<font color=FF0000>--</font>");
				htmlContent.Replace("%chaseRange%", "<font color=FF0000>--</font>");
			}
			
			htmlContent.Replace("%spawnfile%", "<font color=FF0000>--</font>");
			htmlContent.Replace("%spawnname%", "<font color=FF0000>--</font>");
			htmlContent.Replace("%spawngroup%", "<font color=FF0000>--</font>");
			htmlContent.Replace("%spawnai%", "<font color=FF0000>--</font>");
			
			if (npc.hasAI())
			{
				Set<String> clans = NpcData.getInstance().getClansByIds(npc.getTemplate().getClans());
				Set<int> ignoreClanNpcIds = npc.getTemplate().getIgnoreClanNpcIds();
				String clansString = !clans.isEmpty() ? string.Join(", ", clans) : "";
				String ignoreClanNpcIdsString = ignoreClanNpcIds != null ? string.Join(", ", ignoreClanNpcIds) : "";
				
				htmlContent.Replace("%ai_intention%", "<tr><td><table width=270 border=0 bgcolor=131210><tr><td width=100><font color=FFAA00>Intention:</font></td><td align=right width=170>" + npc.getAI().getIntention() + "</td></tr></table></td></tr>");
				htmlContent.Replace("%ai%", "<tr><td><table width=270 border=0><tr><td width=100><font color=FFAA00>AI</font></td><td align=right width=170>" + npc.getAI().GetType().Name + "</td></tr></table></td></tr>");
				htmlContent.Replace("%ai_type%", "<tr><td><table width=270 border=0 bgcolor=131210><tr><td width=100><font color=FFAA00>AIType</font></td><td align=right width=170>" + npc.getAiType() + "</td></tr></table></td></tr>");
				htmlContent.Replace("%ai_clan%", "<tr><td><table width=270 border=0><tr><td width=100><font color=FFAA00>Clan & Range:</font></td><td align=right width=170>" + clansString + " " + npc.getTemplate().getClanHelpRange() + "</td></tr></table></td></tr>");
				htmlContent.Replace("%ai_enemy_clan%", "<tr><td><table width=270 border=0 bgcolor=131210><tr><td width=100><font color=FFAA00>Ignore & Range:</font></td><td align=right width=170>" + ignoreClanNpcIdsString + " " + npc.getTemplate().getAggroRange() + "</td></tr></table></td></tr>");
			}
			else
			{
				htmlContent.Replace("%ai_intention%", "");
				htmlContent.Replace("%ai%", "");
				htmlContent.Replace("%ai_type%", "");
				htmlContent.Replace("%ai_clan%", "");
				htmlContent.Replace("%ai_enemy_clan%", "");
			}
			
			String routeName = WalkingManager.getInstance().getRouteName(npc);
			if (!routeName.isEmpty())
			{
				htmlContent.Replace("%route%", "<tr><td><table width=270 border=0><tr><td width=100><font color=LEVEL>Route:</font></td><td align=right width=170>" + routeName + "</td></tr></table></td></tr>");
			}
			else
			{
				htmlContent.Replace("%route%", "");
			}

			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);
			player.sendPacket(html);
		}
		else if (Config.ALT_GAME_VIEWNPC)
		{
			if (!target.isNpc() || target.isFakePlayer())
			{
				return false;
			}

			player.setTarget(target);
			NpcViewMod.sendNpcView(player, (Npc) target);
		}
		
		return true;
	}
	
	public InstanceType getInstanceType()
	{
		return InstanceType.Npc;
	}
}
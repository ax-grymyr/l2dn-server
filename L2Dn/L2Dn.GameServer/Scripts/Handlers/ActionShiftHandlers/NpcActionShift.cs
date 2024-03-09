using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers.BypassHandlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Quests;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Model.Spawns;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers.ActionShiftHandlers;

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

			HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, "html/admin/npcinfo.htm");
			
			helper.Replace("%objid%", target.getObjectId().ToString());
			helper.Replace("%class%", npc.GetType().Name);
			helper.Replace("%race%", npc.getTemplate().getRace().ToString());
			helper.Replace("%id%", npc.getTemplate().getId().ToString());
			helper.Replace("%lvl%", npc.getTemplate().getLevel().ToString());
			helper.Replace("%name%", npc.getTemplate().getName());
			helper.Replace("%tmplid%", npc.getTemplate().getId().ToString());
			helper.Replace("%aggro%", (target.isAttackable() ? ((Attackable) target).getAggroRange() : 0).ToString());
			helper.Replace("%hp%", ((int) npc.getCurrentHp()).ToString());
			helper.Replace("%hpmax%", npc.getMaxHp().ToString());
			helper.Replace("%mp%", ((int) npc.getCurrentMp()).ToString());
			helper.Replace("%mpmax%", npc.getMaxMp().ToString());
			helper.Replace("%exp%", npc.getTemplate().getExp().ToString());
			helper.Replace("%sp%", npc.getTemplate().getSP().ToString());
			
			helper.Replace("%patk%", npc.getPAtk().ToString());
			helper.Replace("%matk%", npc.getMAtk().ToString());
			helper.Replace("%pdef%", npc.getPDef().ToString());
			helper.Replace("%mdef%", npc.getMDef().ToString());
			helper.Replace("%accu%", npc.getAccuracy().ToString());
			helper.Replace("%evas%", npc.getEvasionRate().ToString());
			helper.Replace("%crit%", npc.getCriticalHit().ToString());
			helper.Replace("%rspd%", npc.getRunSpeed().ToString());
			helper.Replace("%aspd%", npc.getPAtkSpd().ToString());
			helper.Replace("%cspd%", npc.getMAtkSpd().ToString());
			helper.Replace("%atkType%", npc.getTemplate().getBaseAttackType().ToString());
			helper.Replace("%atkRng%", npc.getTemplate().getBaseAttackRange().ToString());
			helper.Replace("%str%", npc.getSTR().ToString());
			helper.Replace("%dex%", npc.getDEX().ToString());
			helper.Replace("%con%", npc.getCON().ToString());
			helper.Replace("%int%", npc.getINT().ToString());
			helper.Replace("%wit%", npc.getWIT().ToString());
			helper.Replace("%men%", npc.getMEN().ToString());
			helper.Replace("%loc%", target.getX() + " " + target.getY() + " " + target.getZ());
			helper.Replace("%heading%", npc.getHeading().ToString());
			helper.Replace("%collision_radius%", npc.getTemplate().getFCollisionRadius().ToString());
			helper.Replace("%collision_height%", npc.getTemplate().getFCollisionHeight().ToString());
			helper.Replace("%clanHall%", clanHall != null ? clanHall.getName() : "none");
			helper.Replace("%mpRewardValue%", npc.getTemplate().getMpRewardValue().ToString());
			helper.Replace("%mpRewardTicks%", npc.getTemplate().getMpRewardTicks().ToString());
			helper.Replace("%mpRewardType%", npc.getTemplate().getMpRewardType().ToString());
			helper.Replace("%mpRewardAffectType%", npc.getTemplate().getMpRewardAffectType().ToString());
			helper.Replace("%loc2d%", ((int) player.calculateDistance2D(npc)).ToString());
			helper.Replace("%loc3d%", ((int) player.calculateDistance3D(npc)).ToString());
			
			AttributeType attackAttribute = npc.getAttackElement();
			helper.Replace("%ele_atk%", attackAttribute.ToString());
			helper.Replace("%ele_atk_value%", npc.getAttackElementValue(attackAttribute).ToString());
			helper.Replace("%ele_dfire%", npc.getDefenseElementValue(AttributeType.FIRE).ToString());
			helper.Replace("%ele_dwater%", npc.getDefenseElementValue(AttributeType.WATER).ToString());
			helper.Replace("%ele_dwind%", npc.getDefenseElementValue(AttributeType.WIND).ToString());
			helper.Replace("%ele_dearth%", npc.getDefenseElementValue(AttributeType.EARTH).ToString());
			helper.Replace("%ele_dholy%", npc.getDefenseElementValue(AttributeType.HOLY).ToString());
			helper.Replace("%ele_ddark%", npc.getDefenseElementValue(AttributeType.DARK).ToString());
			
			Spawn spawn = npc.getSpawn();
			if (spawn != null)
			{
				NpcSpawnTemplate template = spawn.getNpcSpawnTemplate();
				if (template != null)
				{
					String fileName = template.getSpawnTemplate().getFile().Replace('\\', '/');
				
					helper.Replace("%spawnfile%", fileName.Replace("data/spawns/", ""));
					helper.Replace("%spawnname%", template.getSpawnTemplate().getName() ?? string.Empty);
					helper.Replace("%spawngroup%", template.getGroup()?.getName() ?? string.Empty);
					if (template.getSpawnTemplate().getAI() != null)
					{
						Quest script = QuestManager.getInstance().getQuest(template.getSpawnTemplate().getAI());
						if (script != null)
						{
							helper.Replace("%spawnai%", "<a action=\"bypass -h admin_quest_info " + script.getName() + "\"><font color=\"LEVEL\">" + script.getName() + "</font></a>");
						}
					}
					helper.Replace("%spawnai%", "<font color=FF0000>" + template.getSpawnTemplate().getAI() + "</font>");
				}
				helper.Replace("%spawn%", (template != null ? template.getSpawnLocation().getX() : npc.getSpawn().getX()) + " " + (template != null ? template.getSpawnLocation().getY() : npc.getSpawn().getY()) + " " + (template != null ? template.getSpawnLocation().getZ() : npc.getSpawn().getZ()));
				if (npc.getSpawn().getRespawnMinDelay() == TimeSpan.Zero)
				{
					helper.Replace("%resp%", "None");
				}
				else if (npc.getSpawn().hasRespawnRandom())
				{
					helper.Replace("%resp%", (npc.getSpawn().getRespawnMinDelay() / 1000) + "-" + (npc.getSpawn().getRespawnMaxDelay() / 1000) + " sec");
				}
				else
				{
					helper.Replace("%resp%", (npc.getSpawn().getRespawnMinDelay() / 1000) + " sec");
				}
				helper.Replace("%chaseRange%", npc.getSpawn().getChaseRange().ToString());
			}
			else
			{
				helper.Replace("%spawn%", "<font color=FF0000>null</font>");
				helper.Replace("%resp%", "<font color=FF0000>--</font>");
				helper.Replace("%chaseRange%", "<font color=FF0000>--</font>");
			}
			
			helper.Replace("%spawnfile%", "<font color=FF0000>--</font>");
			helper.Replace("%spawnname%", "<font color=FF0000>--</font>");
			helper.Replace("%spawngroup%", "<font color=FF0000>--</font>");
			helper.Replace("%spawnai%", "<font color=FF0000>--</font>");
			
			if (npc.hasAI())
			{
				Set<String> clans = NpcData.getInstance().getClansByIds(npc.getTemplate().getClans());
				Set<int> ignoreClanNpcIds = npc.getTemplate().getIgnoreClanNpcIds();
				String clansString = !clans.isEmpty() ? string.Join(", ", clans) : "";
				String ignoreClanNpcIdsString = ignoreClanNpcIds != null ? string.Join(", ", ignoreClanNpcIds) : "";
				
				helper.Replace("%ai_intention%", "<tr><td><table width=270 border=0 bgcolor=131210><tr><td width=100><font color=FFAA00>Intention:</font></td><td align=right width=170>" + npc.getAI().getIntention() + "</td></tr></table></td></tr>");
				helper.Replace("%ai%", "<tr><td><table width=270 border=0><tr><td width=100><font color=FFAA00>AI</font></td><td align=right width=170>" + npc.getAI().GetType().Name + "</td></tr></table></td></tr>");
				helper.Replace("%ai_type%", "<tr><td><table width=270 border=0 bgcolor=131210><tr><td width=100><font color=FFAA00>AIType</font></td><td align=right width=170>" + npc.getAiType() + "</td></tr></table></td></tr>");
				helper.Replace("%ai_clan%", "<tr><td><table width=270 border=0><tr><td width=100><font color=FFAA00>Clan & Range:</font></td><td align=right width=170>" + clansString + " " + npc.getTemplate().getClanHelpRange() + "</td></tr></table></td></tr>");
				helper.Replace("%ai_enemy_clan%", "<tr><td><table width=270 border=0 bgcolor=131210><tr><td width=100><font color=FFAA00>Ignore & Range:</font></td><td align=right width=170>" + ignoreClanNpcIdsString + " " + npc.getTemplate().getAggroRange() + "</td></tr></table></td></tr>");
			}
			else
			{
				helper.Replace("%ai_intention%", "");
				helper.Replace("%ai%", "");
				helper.Replace("%ai_type%", "");
				helper.Replace("%ai_clan%", "");
				helper.Replace("%ai_enemy_clan%", "");
			}
			
			String routeName = WalkingManager.getInstance().getRouteName(npc);
			if (!routeName.isEmpty())
			{
				helper.Replace("%route%", "<tr><td><table width=270 border=0><tr><td width=100><font color=LEVEL>Route:</font></td><td align=right width=170>" + routeName + "</td></tr></table></td></tr>");
			}
			else
			{
				helper.Replace("%route%", "");
			}

			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(0, 1, helper);
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
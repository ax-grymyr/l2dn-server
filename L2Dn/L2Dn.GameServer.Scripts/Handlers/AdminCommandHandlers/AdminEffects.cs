using System.Globalization;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Html.Styles;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Scripts.Handlers.PlayerActions;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands:
 * <li>invis/invisible/vis/visible = makes yourself invisible or visible
 * <li>earthquake = causes an earthquake of a given intensity and duration around you
 * <li>bighead/shrinkhead = changes head size
 * <li>gmspeed = temporary Super Haste effect.
 * <li>para/unpara = paralyze/remove paralysis from target
 * <li>para_all/unpara_all = same as para/unpara, affects the whole world.
 * <li>changename = temporary change name
 * <li>clearteams/setteam_close/setteam = team related commands
 * <li>social = forces an Creature instance to broadcast social action packets.
 * <li>effect = forces an Creature instance to broadcast MSU packets.
 * <li>abnormal = force changes over an Creature instance's abnormal state.
 * <li>play_sound/play_sounds = Music broadcasting related commands
 * <li>atmosphere = sky change related commands.
 */
public class AdminEffects: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_invis",
		"admin_invisible",
		"admin_setinvis",
		"admin_vis",
		"admin_visible",
		"admin_invis_menu",
		"admin_earthquake",
		"admin_earthquake_menu",
		"admin_bighead",
		"admin_shrinkhead",
		"admin_unpara_all",
		"admin_para_all",
		"admin_unpara",
		"admin_para",
		"admin_unpara_all_menu",
		"admin_para_all_menu",
		"admin_unpara_menu",
		"admin_para_menu",
		"admin_clearteams",
		"admin_setteam_close",
		"admin_setteam",
		"admin_social",
		"admin_effect",
		"admin_npc_use_skill",
		"admin_effect_menu",
		"admin_ave_abnormal",
		"admin_social_menu",
		"admin_play_sounds",
		"admin_play_sound",
		"admin_atmosphere",
		"admin_atmosphere_menu",
		"admin_set_displayeffect",
		"admin_set_displayeffect_menu",
		"admin_event_trigger",
		"admin_settargetable",
		"admin_playmovie",
    ];
	
	public bool useAdminCommand(string commandValue, Player activeChar)
	{
		string command = commandValue;
		StringTokenizer st = new StringTokenizer(command);
		st.nextToken();
		
		if (command.equals("admin_invis_menu"))
		{
			if (!activeChar.isInvisible())
			{
				activeChar.setInvisible(true);
				activeChar.broadcastUserInfo();
				activeChar.sendPacket(new ExUserInfoAbnormalVisualEffectPacket(activeChar));
				World.getInstance().forEachVisibleObject<Creature>(activeChar, target =>
				{
					if ((target != null) && (target.getTarget() == activeChar))
					{
						target.setTarget(null);
						target.abortAttack();
						target.abortCast();
						target.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
					}
				});
				
				BuilderUtil.sendSysMessage(activeChar, "Now, you cannot be seen.");
			}
			else
			{
				activeChar.setInvisible(false);
				activeChar.getEffectList().stopAbnormalVisualEffect(AbnormalVisualEffect.STEALTH);
				activeChar.broadcastUserInfo();
				activeChar.sendPacket(new ExUserInfoAbnormalVisualEffectPacket(activeChar));
				BuilderUtil.sendSysMessage(activeChar, "Now, you can be seen.");
			}
			
			command = "";
			AdminHtml.showAdminHtml(activeChar, "gm_menu.htm");
		}
		else if (command.startsWith("admin_invis"))
		{
			activeChar.setInvisible(true);
			activeChar.broadcastUserInfo();
			activeChar.sendPacket(new ExUserInfoAbnormalVisualEffectPacket(activeChar));
			World.getInstance().forEachVisibleObject<Creature>(activeChar, target =>
			{
				if ((target != null) && (target.getTarget() == activeChar))
				{
					target.setTarget(null);
					target.abortAttack();
					target.abortCast();
					target.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
				}
			});
			BuilderUtil.sendSysMessage(activeChar, "Now, you cannot be seen.");
		}
		else if (command.startsWith("admin_vis"))
		{
			activeChar.setInvisible(false);
			activeChar.getEffectList().stopAbnormalVisualEffect(AbnormalVisualEffect.STEALTH);
			activeChar.broadcastUserInfo();
			activeChar.sendPacket(new ExUserInfoAbnormalVisualEffectPacket(activeChar));
			BuilderUtil.sendSysMessage(activeChar, "Now, you can be seen.");
		}
		else if (command.startsWith("admin_setinvis"))
		{
			if ((activeChar.getTarget() == null) || !activeChar.getTarget().isCreature())
			{
				activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				return false;
			}
			Creature target = (Creature) activeChar.getTarget();
			target.setInvisible(!target.isInvisible());
			BuilderUtil.sendSysMessage(activeChar, "You've made " + target.getName() + " " + (target.isInvisible() ? "invisible" : "visible") + ".");
			if (target.isPlayer())
			{
				((Player) target).broadcastUserInfo();
			}
		}
		else if (command.startsWith("admin_earthquake"))
		{
			try
			{
				string val1 = st.nextToken();
				int intensity = int.Parse(val1);
				string val2 = st.nextToken();
				int duration = int.Parse(val2);
				activeChar.broadcastPacket(new EarthquakePacket(new Location3D(activeChar.getX(), activeChar.getY(), activeChar.getZ()), intensity, duration));
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Usage: //earthquake <intensity> <duration>");
			}
		}
		else if (command.startsWith("admin_atmosphere"))
		{
			try
			{
				string type = st.nextToken();
				string state = st.nextToken();
				int duration = int.Parse(st.nextToken());
				adminAtmosphere(type, state, duration, activeChar);
			}
			catch (Exception ex)
			{
				BuilderUtil.sendSysMessage(activeChar, "Usage: //atmosphere <signsky dawn|dusk>|<sky day|night|red> <duration>");
			}
		}
		else if (command.equals("admin_play_sounds"))
		{
			AdminHtml.showAdminHtml(activeChar, "songs/songs.htm");
		}
		else if (command.startsWith("admin_play_sounds"))
		{
			try
			{
				AdminHtml.showAdminHtml(activeChar, "songs/songs" + command.Substring(18) + ".htm");
			}
			catch (IndexOutOfRangeException e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Usage: //play_sounds <pagenumber>");
			}
		}
		else if (command.startsWith("admin_play_sound"))
		{
			try
			{
				playAdminSound(activeChar, command.Substring(17));
			}
			catch (IndexOutOfRangeException e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Usage: //play_sound <soundname>");
			}
		}
		else if (command.equals("admin_para_all"))
		{
			World.getInstance().forEachVisibleObject<Player>(activeChar, player =>
			{
				if (!player.isGM())
				{
					player.getEffectList().startAbnormalVisualEffect(AbnormalVisualEffect.PARALYZE);
					player.setBlockActions(true);
					player.startParalyze();
					player.broadcastInfo();
				}
			});
		}
		else if (command.equals("admin_unpara_all"))
		{
			World.getInstance().forEachVisibleObject<Player>(activeChar, player =>
			{
				player.getEffectList().stopAbnormalVisualEffect(AbnormalVisualEffect.PARALYZE);
				player.setBlockActions(false);
				player.broadcastInfo();
				
			});
		}
		else if (command.startsWith("admin_para")) // || command.startsWith("admin_para_menu"))
		{
			string type = "1";
			try
			{
				type = st.nextToken();
			}
			catch (Exception e)
			{
			}
			try
			{
				WorldObject target = activeChar.getTarget();
				Creature creature = null;
				if (target.isCreature())
				{
					creature = (Creature) target;
					if (type.equals("1"))
					{
						creature.getEffectList().startAbnormalVisualEffect(AbnormalVisualEffect.PARALYZE);
					}
					else
					{
						creature.getEffectList().startAbnormalVisualEffect(AbnormalVisualEffect.FLESH_STONE);
					}
					creature.setBlockActions(true);
					creature.startParalyze();
					creature.broadcastInfo();
				}
			}
			catch (Exception e)
			{
			}
		}
		else if (command.startsWith("admin_unpara")) // || command.startsWith("admin_unpara_menu"))
		{
			string type = "1";
			try
			{
				type = st.nextToken();
			}
			catch (Exception e)
			{
			}
			try
			{
				WorldObject target = activeChar.getTarget();
				Creature creature = null;
				if (target.isCreature())
				{
					creature = (Creature) target;
					if (type.equals("1"))
					{
						creature.getEffectList().stopAbnormalVisualEffect(AbnormalVisualEffect.PARALYZE);
					}
					else
					{
						creature.getEffectList().stopAbnormalVisualEffect(AbnormalVisualEffect.FLESH_STONE);
					}
					creature.setBlockActions(false);
					creature.broadcastInfo();
				}
			}
			catch (Exception e)
			{
			}
		}
		else if (command.startsWith("admin_bighead"))
		{
			try
			{
				WorldObject target = activeChar.getTarget();
				Creature creature = null;
				if (target.isCreature())
				{
					creature = (Creature) target;
					creature.getEffectList().startAbnormalVisualEffect(AbnormalVisualEffect.BIG_HEAD);
				}
			}
			catch (Exception e)
			{
			}
		}
		else if (command.startsWith("admin_shrinkhead"))
		{
			try
			{
				WorldObject target = activeChar.getTarget();
				Creature creature = null;
				if (target.isCreature())
				{
					creature = (Creature) target;
					creature.getEffectList().stopAbnormalVisualEffect(AbnormalVisualEffect.BIG_HEAD);
				}
			}
			catch (Exception e)
			{
			}
		}
		else if (command.equals("admin_clearteams"))
		{
			World.getInstance().forEachVisibleObject<Player>(activeChar, player =>
			{
				player.setTeam(Team.NONE);
				player.broadcastUserInfo();
			});
		}
		else if (command.startsWith("admin_setteam_close"))
		{
			try
			{
				string val = st.nextToken();
				int radius = 400;
				if (st.hasMoreTokens())
				{
					radius = int.Parse(st.nextToken());
				}
				Team team = Enum.Parse<Team>(val.toUpperCase());
				World.getInstance().forEachVisibleObjectInRange<Player>(activeChar, radius, player => player.setTeam(team));
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Usage: //setteam_close <none|blue|red> [radius]");
			}
		}
		else if (command.startsWith("admin_setteam"))
		{
			try
			{
				Team team = Enum.Parse<Team>(st.nextToken().toUpperCase());
				Creature target = null;
				if (activeChar.getTarget().isCreature())
				{
					target = (Creature) activeChar.getTarget();
				}
				else
				{
					return false;
				}
				target.setTeam(team);
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Usage: //setteam <none|blue|red>");
			}
		}
		else if (command.startsWith("admin_social"))
		{
			try
			{
				string target = null;
				WorldObject obj = activeChar.getTarget();
				if (st.countTokens() == 2)
				{
					int social = int.Parse(st.nextToken());
					target = st.nextToken();
					if (target != null)
					{
						Player player = World.getInstance().getPlayer(target);
						if (player != null)
						{
							if (performSocial(social, player, activeChar))
							{
								activeChar.sendMessage(player.getName() + " was affected by your request.");
							}
						}
						else
						{
							try
							{
								int radius = int.Parse(target);
								World.getInstance().forEachVisibleObjectInRange<WorldObject>(activeChar, radius, obj => performSocial(social, obj, activeChar));
								activeChar.sendMessage(radius + " units radius affected by your request.");
							}
							catch (FormatException nbe)
							{
								BuilderUtil.sendSysMessage(activeChar, "Incorrect parameter");
							}
						}
					}
				}
				else if (st.countTokens() == 1)
				{
					int social = int.Parse(st.nextToken());
					if (obj == null)
					{
						obj = activeChar;
					}
					
					if (performSocial(social, obj, activeChar))
					{
						activeChar.sendMessage(obj.getName() + " was affected by your request.");
					}
					else
					{
						activeChar.sendPacket(SystemMessageId.NOTHING_HAPPENED);
					}
				}
				else if (!command.contains("menu"))
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: //social <social_id> [player_name|radius]");
				}
			}
			catch (Exception e)
			{
			}
		}
		else if (command.startsWith("admin_ave_abnormal"))
		{
			string param1 = null;
			if (st.countTokens() > 0)
			{
				param1 = st.nextToken();
			}

			int page = 0;
			if (!string.IsNullOrEmpty(param1) && !int.TryParse(param1, CultureInfo.InvariantCulture, out page))
			{
				AbnormalVisualEffect ave;
				
				try
				{
					ave = Enum.Parse<AbnormalVisualEffect>(param1);
				}
				catch (Exception e)
				{
					return false;
				}
				
				int radius = 0;
				string param2 = null;
				if (st.countTokens() == 1)
				{
					param2 = st.nextToken();
					if (int.TryParse(param2, CultureInfo.InvariantCulture, out int value))
					{
						radius = value;
					}
				}
				
				if (radius > 0)
				{
					World.getInstance().forEachVisibleObjectInRange<WorldObject>(activeChar, radius, obj => performAbnormalVisualEffect(ave, obj));
					BuilderUtil.sendSysMessage(activeChar, "Affected all characters in radius " + param2 + " by " + param1 + " abnormal visual effect.");
				}
				else
				{
					WorldObject obj = activeChar.getTarget() != null ? activeChar.getTarget() : activeChar;
					if (performAbnormalVisualEffect(ave, obj))
					{
						activeChar.sendMessage(obj.getName() + " affected by " + param1 + " abnormal visual effect.");
					}
					else
					{
						activeChar.sendPacket(SystemMessageId.NOTHING_HAPPENED);
					}
				}
			}
			else
			{
				PageResult result = PageBuilder
					.newBuilder(EnumUtil.GetValues<AbnormalVisualEffect>().ToList(), 100, "bypass -h admin_ave_abnormal")
					.currentPage(page).style(ButtonsStyle.INSTANCE).bodyHandler((pages, ave, sb) =>
					{
						sb.Append(string.Format(
							"<button action=\"bypass admin_ave_abnormal {0}\" align=left icon=teleport>{1}({2})</button>",
							ave.ToString(), ave.ToString(), (int)ave));
						
					}).build();

				HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/ave_abnormal.htm", activeChar);
				NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);
				if (result.getPages() > 0)
				{
					htmlContent.Replace("%pages%", "<table width=280 cellspacing=0><tr>" + result.getPagerTemplate() + "</tr></table>");
				}
				else
				{
					htmlContent.Replace("%pages%", "");
				}
				
				htmlContent.Replace("%abnormals%", result.getBodyTemplate().ToString());
				activeChar.sendPacket(html);
				BuilderUtil.sendSysMessage(activeChar, "Usage: //" + command.Replace("admin_", "") + " <AbnormalVisualEffect> [radius]");
				return true;
			}
		}
		else if (command.startsWith("admin_effect") || command.startsWith("admin_npc_use_skill"))
		{
			try
			{
				WorldObject obj = activeChar.getTarget();
				int level = 1;
				TimeSpan hittime = TimeSpan.Zero;
				int skill = int.Parse(st.nextToken());
				if (st.hasMoreTokens())
				{
					level = int.Parse(st.nextToken());
				}
				if (st.hasMoreTokens())
				{
					hittime = TimeSpan.FromMilliseconds(int.Parse(st.nextToken()));
				}
				if (obj == null)
				{
					obj = activeChar;
				}
				if (!obj.isCreature())
				{
					activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				}
				else
				{
					Creature target = (Creature) obj;
					target.broadcastPacket(new MagicSkillUsePacket(target, activeChar, skill, level, hittime, TimeSpan.Zero));
					activeChar.sendMessage(obj.getName() + " performs MSU " + skill + "/" + level + " by your request.");
				}
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Usage: //effect skill [level | level hittime]");
			}
		}
		else if (command.startsWith("admin_set_displayeffect"))
		{
			WorldObject target = activeChar.getTarget();
			if (!(target is Npc))
			{
				activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				return false;
			}
			Npc npc = (Npc) target;
			try
			{
				string type = st.nextToken();
				int diplayeffect = int.Parse(type);
				npc.setDisplayEffect(diplayeffect);
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Usage: //set_displayeffect <id>");
			}
		}
		else if (command.startsWith("admin_playmovie"))
		{
			try
			{
				new MovieHolder([activeChar], (Movie)(int.Parse(st.nextToken())));
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Usage: //playmovie <id>");
			}
		}
		else if (command.startsWith("admin_event_trigger"))
		{
			try
			{
				int triggerId = int.Parse(st.nextToken());
				bool enable = bool.Parse(st.nextToken());
				World.getInstance().forEachVisibleObject<Player>(activeChar, player => player.sendPacket(new OnEventTriggerPacket(triggerId, enable)));
				activeChar.sendPacket(new OnEventTriggerPacket(triggerId, enable));
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Usage: //event_trigger id [true | false]");
			}
		}
		else if (command.startsWith("admin_settargetable"))
		{
			activeChar.setTargetable(!activeChar.isTargetable());
		}
		
		if (command.contains("menu"))
		{
			showMainPage(activeChar, command);
		}
		return true;
	}
	
	/**
	 * @param ave the abnormal visual effect
	 * @param target the target
	 * @return {@code true} if target's abnormal state was affected, {@code false} otherwise.
	 */
	private bool performAbnormalVisualEffect(AbnormalVisualEffect ave, WorldObject target)
	{
		if (target.isCreature())
		{
			Creature creature = (Creature) target;
			if (!creature.getEffectList().hasAbnormalVisualEffect(ave))
			{
				creature.getEffectList().startAbnormalVisualEffect(ave);
			}
			else
			{
				creature.getEffectList().stopAbnormalVisualEffect(ave);
			}
			return true;
		}
		return false;
	}
	
	private bool performSocial(int action, WorldObject target, Player activeChar)
	{
		try
		{
			if (target.isCreature())
			{
				if (target is Chest)
				{
					activeChar.sendPacket(SystemMessageId.NOTHING_HAPPENED);
					return false;
				}
				if ((target.isNpc()) && ((action < 1) || (action > 20)))
				{
					activeChar.sendPacket(SystemMessageId.NOTHING_HAPPENED);
					return false;
				}
				if ((target.isPlayer()) && ((action < 2) || ((action > 18) && (action != SocialActionPacket.LEVEL_UP))))
				{
					activeChar.sendPacket(SystemMessageId.NOTHING_HAPPENED);
					return false;
				}
				Creature creature = (Creature) target;
				creature.broadcastPacket(new SocialActionPacket(creature.ObjectId, action));
			}
			else
			{
				return false;
			}
		}
		catch (Exception e)
		{
		}
		return true;
	}
	
	/**
	 * @param type - atmosphere type (signssky,sky)
	 * @param state - atmosphere state(night,day)
	 * @param duration
	 * @param activeChar
	 */
	private void adminAtmosphere(string type, string state, int duration, Player activeChar)
	{
		if (type.equals("sky"))
		{
			if (state.equals("night"))
			{
				Broadcast.toAllOnlinePlayers(SunSetPacket.STATIC_PACKET);
			}
			else if (state.equals("day"))
			{
				Broadcast.toAllOnlinePlayers(SunRisePacket.STATIC_PACKET);
			}
			else if (state.equals("red"))
			{
				if (duration != 0)
				{
					Broadcast.toAllOnlinePlayers(new ExRedSkyPacket(duration));
				}
				else
				{
					Broadcast.toAllOnlinePlayers(new ExRedSkyPacket(10));
				}
			}
		}
		else
		{
			BuilderUtil.sendSysMessage(activeChar, "Usage: //atmosphere <signsky dawn|dusk>|<sky day|night|red> <duration>");
		}
	}
	
	private void playAdminSound(Player activeChar, string sound)
	{
		PlaySoundPacket snd = new PlaySoundPacket(1, sound, 0, 0, 0, 0, 0);
		activeChar.sendPacket(snd);
		activeChar.broadcastPacket(snd);
		BuilderUtil.sendSysMessage(activeChar, "Playing " + sound + ".");
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
	
	private void showMainPage(Player activeChar, string command)
	{
		string filename = "effects_menu";
		if (command.contains("social"))
		{
			filename = "social";
		}
		AdminHtml.showAdminHtml(activeChar, filename + ".htm");
	}
}

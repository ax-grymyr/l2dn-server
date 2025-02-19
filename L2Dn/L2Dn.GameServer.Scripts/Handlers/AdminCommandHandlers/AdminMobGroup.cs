using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author littlecrow Admin commands handler for controllable mobs
 */
public class AdminMobGroup: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_mobmenu",
		"admin_mobgroup_list",
		"admin_mobgroup_create",
		"admin_mobgroup_remove",
		"admin_mobgroup_delete",
		"admin_mobgroup_spawn",
		"admin_mobgroup_unspawn",
		"admin_mobgroup_kill",
		"admin_mobgroup_idle",
		"admin_mobgroup_attack",
		"admin_mobgroup_rnd",
		"admin_mobgroup_return",
		"admin_mobgroup_follow",
		"admin_mobgroup_casting",
		"admin_mobgroup_nomove",
		"admin_mobgroup_attackgrp",
		"admin_mobgroup_invul",
    ];
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		if (command.equals("admin_mobmenu"))
		{
			showMainPage(activeChar);
			return true;
		}
		else if (command.equals("admin_mobgroup_list"))
		{
			showGroupList(activeChar);
		}
		else if (command.startsWith("admin_mobgroup_create"))
		{
			createGroup(command, activeChar);
		}
		else if (command.startsWith("admin_mobgroup_delete") || command.startsWith("admin_mobgroup_remove"))
		{
			removeGroup(command, activeChar);
		}
		else if (command.startsWith("admin_mobgroup_spawn"))
		{
			spawnGroup(command, activeChar);
		}
		else if (command.startsWith("admin_mobgroup_unspawn"))
		{
			unspawnGroup(command, activeChar);
		}
		else if (command.startsWith("admin_mobgroup_kill"))
		{
			killGroup(command, activeChar);
		}
		else if (command.startsWith("admin_mobgroup_attackgrp"))
		{
			attackGrp(command, activeChar);
		}
		else if (command.startsWith("admin_mobgroup_attack"))
		{
			if (activeChar.getTarget().isCreature())
			{
				Creature target = (Creature) activeChar.getTarget();
				attack(command, activeChar, target);
			}
		}
		else if (command.startsWith("admin_mobgroup_rnd"))
		{
			setNormal(command, activeChar);
		}
		else if (command.startsWith("admin_mobgroup_idle"))
		{
			idle(command, activeChar);
		}
		else if (command.startsWith("admin_mobgroup_return"))
		{
			returnToChar(command, activeChar);
		}
		else if (command.startsWith("admin_mobgroup_follow"))
		{
			follow(command, activeChar, activeChar);
		}
		else if (command.startsWith("admin_mobgroup_casting"))
		{
			setCasting(command, activeChar);
		}
		else if (command.startsWith("admin_mobgroup_nomove"))
		{
			noMove(command, activeChar);
		}
		else if (command.startsWith("admin_mobgroup_invul"))
		{
			invul(command, activeChar);
		}
		else if (command.startsWith("admin_mobgroup_teleport"))
		{
			teleportGroup(command, activeChar);
		}
		showMainPage(activeChar);
		return true;
	}
	
	private void showMainPage(Player activeChar)
	{
		string filename = "mobgroup.htm";
		AdminHtml.showAdminHtml(activeChar, filename);
	}
	
	private void returnToChar(string command, Player activeChar)
	{
		int groupId;
		try
		{
			groupId = int.Parse(command.Split(" ")[1]);
		}
		catch (Exception e)
		{
			BuilderUtil.sendSysMessage(activeChar, "Incorrect command arguments.");
			return;
		}
		
		MobGroup group = MobGroupTable.getInstance().getGroup(groupId);
		if (group == null)
		{
			BuilderUtil.sendSysMessage(activeChar, "Invalid group specified.");
			return;
		}
		group.returnGroup(activeChar);
	}
	
	private void idle(string command, Player activeChar)
	{
		int groupId;
		
		try
		{
			groupId = int.Parse(command.Split(" ")[1]);
		}
		catch (Exception e)
		{
			BuilderUtil.sendSysMessage(activeChar, "Incorrect command arguments.");
			return;
		}
		MobGroup group = MobGroupTable.getInstance().getGroup(groupId);
		if (group == null)
		{
			BuilderUtil.sendSysMessage(activeChar, "Invalid group specified.");
			return;
		}
		group.setIdleMode();
	}
	
	private void setNormal(string command, Player activeChar)
	{
		int groupId;
		try
		{
			groupId = int.Parse(command.Split(" ")[1]);
		}
		catch (Exception e)
		{
			BuilderUtil.sendSysMessage(activeChar, "Incorrect command arguments.");
			return;
		}
		MobGroup group = MobGroupTable.getInstance().getGroup(groupId);
		if (group == null)
		{
			BuilderUtil.sendSysMessage(activeChar, "Invalid group specified.");
			return;
		}
		group.setAttackRandom();
	}
	
	private void attack(string command, Player activeChar, Creature target)
	{
		int groupId;
		try
		{
			groupId = int.Parse(command.Split(" ")[1]);
		}
		catch (Exception e)
		{
			BuilderUtil.sendSysMessage(activeChar, "Incorrect command arguments.");
			return;
		}
		MobGroup group = MobGroupTable.getInstance().getGroup(groupId);
		if (group == null)
		{
			BuilderUtil.sendSysMessage(activeChar, "Invalid group specified.");
			return;
		}
		group.setAttackTarget(target);
	}
	
	private void follow(string command, Player activeChar, Creature target)
	{
		int groupId;
		try
		{
			groupId = int.Parse(command.Split(" ")[1]);
		}
		catch (Exception e)
		{
			BuilderUtil.sendSysMessage(activeChar, "Incorrect command arguments.");
			return;
		}
		MobGroup group = MobGroupTable.getInstance().getGroup(groupId);
		if (group == null)
		{
			BuilderUtil.sendSysMessage(activeChar, "Invalid group specified.");
			return;
		}
		group.setFollowMode(target);
	}
	
	private void createGroup(string command, Player activeChar)
	{
		int groupId;
		int templateId;
		int mobCount;
		
		try
		{
			string[] cmdParams = command.Split(" ");
			groupId = int.Parse(cmdParams[1]);
			templateId = int.Parse(cmdParams[2]);
			mobCount = int.Parse(cmdParams[3]);
		}
		catch (Exception e)
		{
			BuilderUtil.sendSysMessage(activeChar, "Usage: //mobgroup_create <group> <npcid> <count>");
			return;
		}
		
		if (MobGroupTable.getInstance().getGroup(groupId) != null)
		{
			BuilderUtil.sendSysMessage(activeChar, "Mob group " + groupId + " already exists.");
			return;
		}
		
		NpcTemplate template = NpcData.getInstance().getTemplate(templateId);
		if (template == null)
		{
			BuilderUtil.sendSysMessage(activeChar, "Invalid NPC ID specified.");
			return;
		}
		
		MobGroup group = new MobGroup(groupId, template, mobCount);
		MobGroupTable.getInstance().addGroup(groupId, group);
		BuilderUtil.sendSysMessage(activeChar, "Mob group " + groupId + " created.");
	}
	
	private void removeGroup(string command, Player activeChar)
	{
		int groupId;
		
		try
		{
			groupId = int.Parse(command.Split(" ")[1]);
		}
		catch (Exception e)
		{
			BuilderUtil.sendSysMessage(activeChar, "Usage: //mobgroup_remove <groupId>");
			return;
		}
		
		MobGroup group = MobGroupTable.getInstance().getGroup(groupId);
		if (group == null)
		{
			BuilderUtil.sendSysMessage(activeChar, "Invalid group specified.");
			return;
		}
		
		doAnimation(activeChar);
		group.unspawnGroup();
		
		if (MobGroupTable.getInstance().removeGroup(groupId))
		{
			BuilderUtil.sendSysMessage(activeChar, "Mob group " + groupId + " unspawned and removed.");
		}
	}
	
	private void spawnGroup(string command, Player activeChar)
	{
		int groupId;
		bool topos = false;
		int posx = 0;
		int posy = 0;
		int posz = 0;
		
		try
		{
			string[] cmdParams = command.Split(" ");
			groupId = int.Parse(cmdParams[1]);
			
			try
			{ // we try to get a position
				posx = int.Parse(cmdParams[2]);
				posy = int.Parse(cmdParams[3]);
				posz = int.Parse(cmdParams[4]);
				topos = true;
			}
			catch (Exception e)
			{
				// no position given
			}
		}
		catch (Exception e)
		{
			BuilderUtil.sendSysMessage(activeChar, "Usage: //mobgroup_spawn <group> [ x y z ]");
			return;
		}
		
		MobGroup group = MobGroupTable.getInstance().getGroup(groupId);
		if (group == null)
		{
			BuilderUtil.sendSysMessage(activeChar, "Invalid group specified.");
			return;
		}
		
		doAnimation(activeChar);
		
		if (topos)
		{
			group.spawnGroup(posx, posy, posz);
		}
		else
		{
			group.spawnGroup(activeChar);
		}
		
		BuilderUtil.sendSysMessage(activeChar, "Mob group " + groupId + " spawned.");
	}
	
	private void unspawnGroup(string command, Player activeChar)
	{
		int groupId;
		
		try
		{
			groupId = int.Parse(command.Split(" ")[1]);
		}
		catch (Exception e)
		{
			BuilderUtil.sendSysMessage(activeChar, "Usage: //mobgroup_unspawn <groupId>");
			return;
		}
		
		MobGroup group = MobGroupTable.getInstance().getGroup(groupId);
		if (group == null)
		{
			BuilderUtil.sendSysMessage(activeChar, "Invalid group specified.");
			return;
		}
		
		doAnimation(activeChar);
		group.unspawnGroup();
		
		BuilderUtil.sendSysMessage(activeChar, "Mob group " + groupId + " unspawned.");
	}
	
	private void killGroup(string command, Player activeChar)
	{
		int groupId;
		
		try
		{
			groupId = int.Parse(command.Split(" ")[1]);
		}
		catch (Exception e)
		{
			BuilderUtil.sendSysMessage(activeChar, "Usage: //mobgroup_kill <groupId>");
			return;
		}
		
		MobGroup group = MobGroupTable.getInstance().getGroup(groupId);
		if (group == null)
		{
			BuilderUtil.sendSysMessage(activeChar, "Invalid group specified.");
			return;
		}
		
		doAnimation(activeChar);
		group.killGroup(activeChar);
	}
	
	private void setCasting(string command, Player activeChar)
	{
		int groupId;
		
		try
		{
			groupId = int.Parse(command.Split(" ")[1]);
		}
		catch (Exception e)
		{
			BuilderUtil.sendSysMessage(activeChar, "Usage: //mobgroup_casting <groupId>");
			return;
		}
		
		MobGroup group = MobGroupTable.getInstance().getGroup(groupId);
		if (group == null)
		{
			BuilderUtil.sendSysMessage(activeChar, "Invalid group specified.");
			return;
		}
		
		group.setCastMode();
	}
	
	private void noMove(string command, Player activeChar)
	{
		int groupId;
		string enabled;
		
		try
		{
			groupId = int.Parse(command.Split(" ")[1]);
			enabled = command.Split(" ")[2];
		}
		catch (Exception e)
		{
			BuilderUtil.sendSysMessage(activeChar, "Usage: //mobgroup_nomove <groupId> <on|off>");
			return;
		}
		
		MobGroup group = MobGroupTable.getInstance().getGroup(groupId);
		if (group == null)
		{
			BuilderUtil.sendSysMessage(activeChar, "Invalid group specified.");
			return;
		}
		
		if (enabled.equalsIgnoreCase("on") || enabled.equalsIgnoreCase("true"))
		{
			group.setNoMoveMode(true);
		}
		else if (enabled.equalsIgnoreCase("off") || enabled.equalsIgnoreCase("false"))
		{
			group.setNoMoveMode(false);
		}
		else
		{
			BuilderUtil.sendSysMessage(activeChar, "Incorrect command arguments.");
		}
	}
	
	private void doAnimation(Player activeChar)
	{
		Broadcast.toSelfAndKnownPlayersInRadius(activeChar, new MagicSkillUsePacket(activeChar, 1008, 1, TimeSpan.FromMilliseconds(4000), TimeSpan.Zero), 1500);
		activeChar.sendPacket(new SetupGaugePacket(activeChar.ObjectId, 0, TimeSpan.FromMilliseconds(4000)));
	}
	
	private void attackGrp(string command, Player activeChar)
	{
		int groupId;
		int othGroupId;
		
		try
		{
			groupId = int.Parse(command.Split(" ")[1]);
			othGroupId = int.Parse(command.Split(" ")[2]);
		}
		catch (Exception e)
		{
			BuilderUtil.sendSysMessage(activeChar, "Usage: //mobgroup_attackgrp <groupId> <TargetGroupId>");
			return;
		}
		
		MobGroup group = MobGroupTable.getInstance().getGroup(groupId);
		if (group == null)
		{
			BuilderUtil.sendSysMessage(activeChar, "Invalid group specified.");
			return;
		}
		
		MobGroup othGroup = MobGroupTable.getInstance().getGroup(othGroupId);
		if (othGroup == null)
		{
			BuilderUtil.sendSysMessage(activeChar, "Incorrect target group.");
			return;
		}
		
		group.setAttackGroup(othGroup);
	}
	
	private void invul(string command, Player activeChar)
	{
		int groupId;
		string enabled;
		
		try
		{
			groupId = int.Parse(command.Split(" ")[1]);
			enabled = command.Split(" ")[2];
		}
		catch (Exception e)
		{
			BuilderUtil.sendSysMessage(activeChar, "Usage: //mobgroup_invul <groupId> <on|off>");
			return;
		}
		
		MobGroup group = MobGroupTable.getInstance().getGroup(groupId);
		if (group == null)
		{
			BuilderUtil.sendSysMessage(activeChar, "Invalid group specified.");
			return;
		}
		
		if (enabled.equalsIgnoreCase("on") || enabled.equalsIgnoreCase("true"))
		{
			group.setInvul(true);
		}
		else if (enabled.equalsIgnoreCase("off") || enabled.equalsIgnoreCase("false"))
		{
			group.setInvul(false);
		}
		else
		{
			BuilderUtil.sendSysMessage(activeChar, "Incorrect command arguments.");
		}
	}
	
	private void teleportGroup(string command, Player activeChar)
	{
		int groupId;
		string targetPlayerStr = null;
		Player targetPlayer = null;
		
		try
		{
			groupId = int.Parse(command.Split(" ")[1]);
			targetPlayerStr = command.Split(" ")[2];
			if (targetPlayerStr != null)
			{
				targetPlayer = World.getInstance().getPlayer(targetPlayerStr);
			}
			
			if (targetPlayer == null)
			{
				targetPlayer = activeChar;
			}
		}
		catch (Exception e)
		{
			BuilderUtil.sendSysMessage(activeChar, "Usage: //mobgroup_teleport <groupId> [playerName]");
			return;
		}
		
		MobGroup group = MobGroupTable.getInstance().getGroup(groupId);
		if (group == null)
		{
			BuilderUtil.sendSysMessage(activeChar, "Invalid group specified.");
			return;
		}
		
		group.teleportGroup(activeChar);
	}
	
	private void showGroupList(Player activeChar)
	{
		BuilderUtil.sendSysMessage(activeChar, "======= <Mob Groups> =======");
		foreach (MobGroup mobGroup in MobGroupTable.getInstance().getGroups())
		{
			activeChar.sendMessage(mobGroup.getGroupId() + ": " + mobGroup.getActiveMobCount() + " alive out of " + mobGroup.getMaxMobCount() + " of NPC ID " + mobGroup.getTemplate().getId() + " (" + mobGroup.getStatus() + ")");
		}
		
		activeChar.sendPacket(SystemMessageId.EMPTY_3);
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}

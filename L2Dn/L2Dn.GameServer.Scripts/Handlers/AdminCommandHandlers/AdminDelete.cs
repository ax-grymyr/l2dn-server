using System.Globalization;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Spawns;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author Mobius
 */
public class AdminDelete: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_delete", // supports range parameter
		"admin_delete_group", // for territory spawns
    ];

	public bool useAdminCommand(string command, Player activeChar)
	{
		if (command.contains("group"))
		{
			handleDeleteGroup(activeChar);
		}
		else if (command.startsWith("admin_delete"))
		{
			string[] split = command.Split(" ");
			if (split.Length <= 1 || !int.TryParse(split[1], CultureInfo.InvariantCulture, out int range))
				range = 0;

			handleDelete(activeChar, range);
		}
		return true;
	}

	private void handleDelete(Player player, int range)
	{
		if (range > 0)
		{
			World.getInstance().forEachVisibleObjectInRange<Npc>(player, range, target => deleteNpc(player, target));
			return;
		}

		WorldObject? obj = player.getTarget();
		if (obj is Npc npc)
		{
			deleteNpc(player, npc);
		}
		else
		{
			BuilderUtil.sendSysMessage(player, "Incorrect target.");
		}
	}

	private void handleDeleteGroup(Player player)
	{
		WorldObject? obj = player.getTarget();
		if (obj is Npc npc)
		{
			deleteGroup(player, npc);
		}
		else
		{
			BuilderUtil.sendSysMessage(player, "Incorrect target.");
		}
	}

	private void deleteNpc(Player player, Npc target)
	{
		Spawn spawn = target.getSpawn();
		if (spawn != null)
		{
			NpcSpawnTemplate? npcSpawnTemplate = spawn.getNpcSpawnTemplate();
			SpawnGroup? group = npcSpawnTemplate != null ? npcSpawnTemplate.getGroup() : null;
			List<SpawnTerritory> territories = group != null ? group.getTerritories() : [];
			if (territories.Count == 0)
			{
				SpawnTemplate? spawnTemplate = npcSpawnTemplate != null ? npcSpawnTemplate.getSpawnTemplate() : null;
				if (spawnTemplate != null)
				{
					territories = spawnTemplate.getTerritories();
				}
			}
			if (territories.Count == 0)
			{
				target.deleteMe();
				spawn.stopRespawn();
				if (DbSpawnManager.getInstance().isDefined(spawn.getId()))
				{
					DbSpawnManager.getInstance().deleteSpawn(spawn, true);
				}
				else
				{
					SpawnTable.getInstance().deleteSpawn(spawn, true);
				}
				BuilderUtil.sendSysMessage(player, "Deleted " + target.getName() + " from " + target.ObjectId + ".");
			}
			else
			{
				AdminCommandHandler.getInstance().useAdminCommand(player, AdminDelete.ADMIN_COMMANDS[1], true);
			}
		}
	}

	private void deleteGroup(Player player, Npc target)
	{
		Spawn spawn = target.getSpawn();
		if (spawn != null)
		{
			NpcSpawnTemplate? npcSpawnTemplate = spawn.getNpcSpawnTemplate();
			SpawnGroup? group = npcSpawnTemplate != null ? npcSpawnTemplate.getGroup() : null;
			List<SpawnTerritory> territories = group != null ? group.getTerritories() : [];
			bool simpleTerritory = false;
			if (territories.Count == 0)
			{
				SpawnTemplate? spawnTemplate = npcSpawnTemplate != null ? npcSpawnTemplate.getSpawnTemplate() : null;
				if (spawnTemplate != null)
				{
					territories = spawnTemplate.getTerritories();
					simpleTerritory = true;
				}
			}
			if (territories.Count == 0)
			{
				BuilderUtil.sendSysMessage(player, "Incorrect target.");
			}
			else
			{
				target.deleteMe();
				spawn.stopRespawn();
				if (DbSpawnManager.getInstance().isDefined(spawn.getId()))
				{
					DbSpawnManager.getInstance().deleteSpawn(spawn, true);
				}
				else
				{
					SpawnTable.getInstance().deleteSpawn(spawn, true);
				}

				if (group != null)
				{
					foreach (NpcSpawnTemplate template in group.getSpawns())
					{
						template.despawn();
					}
				}
				else if (simpleTerritory && npcSpawnTemplate != null)
				{
					npcSpawnTemplate.despawn();
				}

				BuilderUtil.sendSysMessage(player, "Deleted " + target.getName() + " group from " + target.ObjectId + ".");
			}
		}
	}

	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
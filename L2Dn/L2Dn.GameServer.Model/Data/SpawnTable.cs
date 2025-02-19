using System.Runtime.CompilerServices;
using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Spawns;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data;

/**
 * Spawn data retriever.
 * @author Zoey76, Mobius
 */
public class SpawnTable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(SpawnTable));
	private static readonly Map<int, Set<Spawn>> _spawnTable = new();
	private const string OTHER_XML_FOLDER = "spawns/Others";

	/**
	 * Gets the spawn data.
	 * @return the spawn data
	 */
	public Map<int, Set<Spawn>> getSpawnTable()
	{
		return _spawnTable;
	}

	/**
	 * Gets the spawns for the NPC Id.
	 * @param npcId the NPC Id
	 * @return the spawn set for the given npcId
	 */
	public Set<Spawn> getSpawns(int npcId)
	{
		return _spawnTable.GetValueOrDefault(npcId, []);
	}

	/**
	 * Gets the spawn count for the given NPC ID.
	 * @param npcId the NPC Id
	 * @return the spawn count
	 */
	public int getSpawnCount(int npcId)
	{
		return getSpawns(npcId).Count;
	}

	/**
	 * Gets a spawn for the given NPC ID.
	 * @param npcId the NPC Id
	 * @return a spawn for the given NPC ID or {@code null}
	 */
	public Spawn? getAnySpawn(int npcId)
	{
		return getSpawns(npcId).FirstOrDefault();
	}

	/**
	 * Adds a new spawn to the spawn table.
	 * @param spawn the spawn to add
	 * @param store if {@code true} it'll be saved in the spawn XML files
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void addNewSpawn(Spawn spawn, bool store)
	{
		addSpawn(spawn);

		// if (store)
		// {
		// 	// Create output directory if it doesn't exist
		// 	File outputDirectory = new File(OTHER_XML_FOLDER);
		// 	if (!outputDirectory.exists())
		// 	{
		// 		bool result = false;
		// 		try
		// 		{
		// 			outputDirectory.mkdir();
		// 			result = true;
		// 		}
		// 		catch (Exception se)
		// 		{
		// 			// empty
		// 		}
		// 		if (result)
		// 		{
		// 			LOGGER.Info(GetType().Name + ": Created directory: " + OTHER_XML_FOLDER);
		// 		}
		// 	}
		//
		// 	// XML file for spawn
		// 	int x = ((spawn.getX() - World.WORLD_X_MIN) >> 15) + World.TILE_X_MIN;
		// 	int y = ((spawn.getY() - World.WORLD_Y_MIN) >> 15) + World.TILE_Y_MIN;
		// 	File spawnFile = new File(OTHER_XML_FOLDER + "/" + x + "_" + y + ".xml");
		//
		// 	// Write info to XML
		// 	String spawnId = String.valueOf(spawn.getId());
		// 	String spawnCount = String.valueOf(spawn.getAmount());
		// 	String spawnX = String.valueOf(spawn.getX());
		// 	String spawnY = String.valueOf(spawn.getY());
		// 	String spawnZ = String.valueOf(spawn.getZ());
		// 	String spawnHeading = String.valueOf(spawn.getHeading());
		// 	String spawnDelay = String.valueOf(spawn.getRespawnDelay() / 1000);
		// 	if (spawnFile.exists()) // update
		// 	{
		// 		File tempFile = new File(spawnFile.getAbsolutePath().substring(Config.DATAPACK_ROOT_PATH.getAbsolutePath().length() + 1).replace('\\', '/') + ".tmp");
		// 		try
		// 		{
		// 			BufferedReader reader = new BufferedReader(new FileReader(spawnFile));
		// 			BufferedWriter writer = new BufferedWriter(new FileWriter(tempFile));
		// 			String currentLine;
		// 			while ((currentLine = reader.readLine()) != null)
		// 			{
		// 				if (currentLine.Contains("</group>"))
		// 				{
		// 					NpcTemplate template = NpcData.getInstance().getTemplate(spawn.getId());
		// 					String title = template.getTitle();
		// 					String name = title.isEmpty() ? template.getName() : template.getName() + " - " + title;
		// 					writer.write("			<npc id=\"" + spawnId + (spawn.getAmount() > 1 ? "\" count=\"" + spawnCount : "") + "\" x=\"" + spawnX + "\" y=\"" + spawnY + "\" z=\"" + spawnZ + (spawn.getHeading() > 0 ? "\" heading=\"" + spawnHeading : "") + "\" respawnTime=\"" + spawnDelay + "sec\" /> <!-- " + name + " -->" + Config.EOL);
		// 					writer.write(currentLine + Config.EOL);
		// 					continue;
		// 				}
		// 				writer.write(currentLine + Config.EOL);
		// 			}
		// 			writer.close();
		// 			reader.close();
		// 			spawnFile.delete();
		// 			tempFile.renameTo(spawnFile);
		// 		}
		// 		catch (Exception e)
		// 		{
		// 			LOGGER.Warn(GetType().Name + ": Could not store spawn in the spawn XML files: " + e);
		// 		}
		// 	}
		// 	else // new file
		// 	{
		// 		try
		// 		{
		// 			NpcTemplate template = NpcData.getInstance().getTemplate(spawn.getId());
		// 			String title = template.getTitle();
		// 			String name = title.isEmpty() ? template.getName() : template.getName() + " - " + title;
		// 			BufferedWriter writer = new BufferedWriter(new FileWriter(spawnFile));
		// 			writer.write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine);
		// 			writer.write("<list xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"../../xsd/spawns.xsd\">" + Config.EOL);
		// 			writer.write("	<spawn name=\"" + x + "_" + y + "\">" + Environment.NewLine);
		// 			writer.write("		<group>" + Environment.NewLine);
		// 			writer.write("			<npc id=\"" + spawnId + (spawn.getAmount() > 1 ? "\" count=\"" + spawnCount : "") + "\" x=\"" + spawnX + "\" y=\"" + spawnY + "\" z=\"" + spawnZ + (spawn.getHeading() > 0 ? "\" heading=\"" + spawnHeading : "") + "\" respawnTime=\"" + spawnDelay + "sec\" /> <!-- " + name + " -->" + Config.EOL);
		// 			writer.write("		</group>" + Environment.NewLine);
		// 			writer.write("	</spawn>" + Environment.NewLine);
		// 			writer.write("</list>" + Environment.NewLine);
		// 			writer.close();
		// 			LOGGER.Info(GetType().Name + ": Created file: " + OTHER_XML_FOLDER + "/" + x + "_" + y + ".xml");
		// 		}
		// 		catch (Exception e)
		// 		{
		// 			LOGGER.Warn(GetType().Name + ": Spawn " + spawn + " could not be added to the spawn XML files: " + e);
		// 		}
		// 	}
		// }
	}

	/**
	 * Delete an spawn from the spawn table.
	 * @param spawn the spawn to delete
	 * @param update if {@code true} the spawn XML files will be updated
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void deleteSpawn(Spawn spawn, bool update)
	{
		if (!removeSpawn(spawn) && !update)
		{
			return;
		}

		// if (update)
		// {
		// 	int x = ((spawn.getX() - World.WORLD_X_MIN) >> 15) + World.TILE_X_MIN;
		// 	int y = ((spawn.getY() - World.WORLD_Y_MIN) >> 15) + World.TILE_Y_MIN;
		// 	NpcSpawnTemplate npcSpawnTemplate = spawn.getNpcSpawnTemplate();
		// 	File spawnFile = npcSpawnTemplate != null ? npcSpawnTemplate.getSpawnTemplate().getFile() : new File(OTHER_XML_FOLDER + "/" + x + "_" + y + ".xml");
		// 	File tempFile = new File(spawnFile.getAbsolutePath().substring(Config.DATAPACK_ROOT_PATH.getAbsolutePath().length() + 1).replace('\\', '/') + ".tmp");
		// 	try
		// 	{
		// 		BufferedReader reader = new BufferedReader(new FileReader(spawnFile));
		// 		BufferedWriter writer = new BufferedWriter(new FileWriter(tempFile));
		//
		// 		bool found = false; // in XML you can have more than one spawn with same coords
		// 		bool isMultiLine = false; // in case spawn has more stats
		// 		bool lastLineFound = false; // used to check for empty file
		// 		int lineCount = 0;
		// 		String currentLine;
		//
		// 		SpawnGroup group = npcSpawnTemplate != null ? npcSpawnTemplate.getGroup() : null;
		// 		List<SpawnTerritory> territories = group != null ? group.getTerritories() : new();
		// 		bool simpleTerritory = false;
		// 		if (territories.isEmpty())
		// 		{
		// 			SpawnTemplate spawnTemplate = npcSpawnTemplate != null ? npcSpawnTemplate.getSpawnTemplate() : null;
		// 			if (spawnTemplate != null)
		// 			{
		// 				territories = spawnTemplate.getTerritories();
		// 				simpleTerritory = true;
		// 			}
		// 		}
		//
		// 		if (territories.isEmpty())
		// 		{
		// 			String spawnId = String.valueOf(spawn.getId());
		// 			String spawnX = String.valueOf(npcSpawnTemplate != null ? npcSpawnTemplate.getSpawnLocation().getX() : spawn.getX());
		// 			String spawnY = String.valueOf(npcSpawnTemplate != null ? npcSpawnTemplate.getSpawnLocation().getY() : spawn.getY());
		// 			String spawnZ = String.valueOf(npcSpawnTemplate != null ? npcSpawnTemplate.getSpawnLocation().getZ() : spawn.getZ());
		//
		// 			while ((currentLine = reader.readLine()) != null)
		// 			{
		// 				if (!found)
		// 				{
		// 					if (isMultiLine)
		// 					{
		// 						if (currentLine.Contains("</npc>"))
		// 						{
		// 							found = true;
		// 						}
		// 						continue;
		// 					}
		// 					if (currentLine.Contains(spawnId) && currentLine.Contains(spawnX) && currentLine.Contains(spawnY) && currentLine.Contains(spawnZ))
		// 					{
		// 						if (!currentLine.Contains("/>") && !currentLine.Contains("</npc>"))
		// 						{
		// 							isMultiLine = true;
		// 						}
		// 						else
		// 						{
		// 							found = true;
		// 						}
		// 						continue;
		// 					}
		// 				}
		// 				writer.write(currentLine + Environment.NewLine);
		// 				if (currentLine.Contains("</list>"))
		// 				{
		// 					lastLineFound = true;
		// 				}
		// 				if (!lastLineFound)
		// 				{
		// 					lineCount++;
		// 				}
		// 			}
		// 		}
		// 		else
		// 		{
		// 			SEARCH: while ((currentLine = reader.readLine()) != null)
		// 			{
		// 				if (!found)
		// 				{
		// 					if (isMultiLine)
		// 					{
		// 						if (currentLine.Contains("</group>") || (simpleTerritory && currentLine.Contains("<territories>")))
		// 						{
		// 							found = true;
		// 						}
		// 						continue;
		// 					}
		// 					foreach (SpawnTerritory territory in territories)
		// 					{
		// 						if (currentLine.Contains('"' + territory.getName() + '"'))
		// 						{
		// 							isMultiLine = true;
		// 							continue SEARCH;
		// 						}
		// 					}
		// 				}
		// 				writer.write(currentLine + Environment.NewLine);
		// 				if (currentLine.Contains("</list>"))
		// 				{
		// 					lastLineFound = true;
		// 				}
		// 				if (!lastLineFound)
		// 				{
		// 					lineCount++;
		// 				}
		// 			}
		// 		}
		//
		// 		writer.close();
		// 		reader.close();
		// 		spawnFile.delete();
		// 		tempFile.renameTo(spawnFile);
		// 		// Delete empty file
		// 		if (lineCount < 8)
		// 		{
		// 			LOGGER.Info(GetType().Name + ": Deleted empty file: " + spawnFile.getAbsolutePath().substring(Config.DATAPACK_ROOT.getAbsolutePath().length() + 1).replace('\\', '/'));
		// 			spawnFile.delete();
		// 		}
		// 	}
		// 	catch (Exception e)
		// 	{
		// 		LOGGER.Warn(GetType().Name + ": Spawn " + spawn + " could not be removed from the spawn XML files: " + e);
		// 	}
		// }
	}

	/**
	 * Add a spawn to the spawn set if present, otherwise add a spawn set and add the spawn to the newly created spawn set.
	 * @param spawn the NPC spawn to add
	 */
	private void addSpawn(Spawn spawn)
	{
		_spawnTable.computeIfAbsent(spawn.getId(), k => new()).add(spawn);
	}

	/**
	 * Remove a spawn from the spawn set, if the spawn set is empty, remove it as well.
	 * @param spawn the NPC spawn to remove
	 * @return {@code true} if the spawn was successfully removed, {@code false} otherwise
	 */
	private bool removeSpawn(Spawn spawn)
	{
		Set<Spawn>? set = _spawnTable.get(spawn.getId());
		if (set != null)
		{
			bool removed = set.remove(spawn);
			if (set.isEmpty())
			{
				_spawnTable.remove(spawn.getId());
			}
			set.ForEach(notifyRemoved);
			return removed;
		}
		notifyRemoved(spawn);
		return false;
	}

	private void notifyRemoved(Spawn spawn)
	{
		if (spawn != null && spawn.getLastSpawn() != null && spawn.getNpcSpawnTemplate() != null)
		{
			spawn.getNpcSpawnTemplate().notifyDespawnNpc(spawn.getLastSpawn());
		}
	}

	/**
	 * Execute a procedure over all spawns.<br>
	 * <font size="4" color="red">Do not use it!</font>
	 * @param function the function to execute
	 * @return {@code true} if all procedures were executed, {@code false} otherwise
	 */
	public bool forEachSpawn(Func<Spawn, bool> function)
	{
		foreach (Set<Spawn> set in _spawnTable.Values)
		{
			foreach (Spawn spawn in set)
			{
				if (!function(spawn))
				{
					return false;
				}
			}
		}
		return true;
	}

	public static SpawnTable getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly SpawnTable INSTANCE = new SpawnTable();
	}
}
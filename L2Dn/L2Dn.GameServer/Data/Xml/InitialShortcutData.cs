using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class holds the Initial Shortcuts information.<br>
 * What shortcuts get each newly created character.
 * @author Zoey76
 */
public class InitialShortcutData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(InitialShortcutData));
	
	private readonly Map<ClassId, List<Shortcut>> _initialShortcutData = new();
	private readonly List<Shortcut> _initialGlobalShortcutList = new();
	private readonly Map<int, Macro> _macroPresets = new();
	
	/**
	 * Instantiates a new initial shortcuts data.
	 */
	protected InitialShortcutData()
	{
		load();
	}
	
	public void load()
	{
		_initialShortcutData.clear();
		_initialGlobalShortcutList.Clear();
		
		parseDatapackFile("data/stats/initialShortcuts.xml");
		
		LOGGER.Info(GetType().Name + ": Loaded " + _initialGlobalShortcutList.size() + " initial global shortcuts data.");
		LOGGER.Info(GetType().Name + ": Loaded " + _initialShortcutData.size() + " initial shortcuts data.");
		LOGGER.Info(GetType().Name + ": Loaded " + _macroPresets.size() + " macro presets.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equals(n.getNodeName()))
			{
				for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					switch (d.getNodeName())
					{
						case "shortcuts":
						{
							parseShortcuts(d);
							break;
						}
						case "macros":
						{
							parseMacros(d);
							break;
						}
					}
				}
			}
		}
	}
	
	/**
	 * Parses a shortcut.
	 * @param d the node
	 */
	private void parseShortcuts(Node d)
	{
		NamedNodeMap attrs = d.getAttributes();
		Node classIdNode = attrs.getNamedItem("classId");
		List<Shortcut> list = new();
		for (Node c = d.getFirstChild(); c != null; c = c.getNextSibling())
		{
			if ("page".equals(c.getNodeName()))
			{
				attrs = c.getAttributes();
				int pageId = parseInteger(attrs, "pageId");
				for (Node b = c.getFirstChild(); b != null; b = b.getNextSibling())
				{
					if ("slot".equals(b.getNodeName()))
					{
						list.add(createShortcut(pageId, b));
					}
				}
			}
		}
		
		if (classIdNode != null)
		{
			_initialShortcutData.put(ClassId.getClassId(int.Parse(classIdNode.getNodeValue())), list);
		}
		else
		{
			_initialGlobalShortcutList.addAll(list);
		}
	}
	
	/**
	 * Parses a macro.
	 * @param d the node
	 */
	private void parseMacros(Node d)
	{
		for (Node c = d.getFirstChild(); c != null; c = c.getNextSibling())
		{
			if ("macro".equals(c.getNodeName()))
			{
				NamedNodeMap attrs = c.getAttributes();
				if (!parseBoolean(attrs, "enabled", true))
				{
					continue;
				}
				
				int macroId = parseInteger(attrs, "macroId");
				int icon = parseInteger(attrs, "icon");
				String name = parseString(attrs, "name");
				String description = parseString(attrs, "description");
				String acronym = parseString(attrs, "acronym");
				List<MacroCmd> commands = new ArrayList<>(1);
				int entry = 0;
				for (Node b = c.getFirstChild(); b != null; b = b.getNextSibling())
				{
					if ("command".equals(b.getNodeName()))
					{
						attrs = b.getAttributes();
						MacroType type = parseEnum(attrs, MacroType.class, "type");
						int d1 = 0;
						int d2 = 0;
						String cmd = b.getTextContent();
						switch (type)
						{
							case MacroType.SKILL:
							{
								d1 = parseInteger(attrs, "skillId"); // Skill ID
								d2 = parseInteger(attrs, "skillLevel", 0); // Skill level
								break;
							}
							case MacroType.ACTION:
							{
								// Not handled by client.
								d1 = parseInteger(attrs, "actionId");
								break;
							}
							case MacroType.TEXT:
							{
								// Doesn't have numeric parameters.
								break;
							}
							case MacroType.SHORTCUT:
							{
								d1 = parseInteger(attrs, "page"); // Page
								d2 = parseInteger(attrs, "slot", 0); // Slot
								break;
							}
							case MacroType.ITEM:
							{
								// Not handled by client.
								d1 = parseInteger(attrs, "itemId");
								break;
							}
							case MacroType.DELAY:
							{
								d1 = parseInteger(attrs, "delay"); // Delay in seconds
								break;
							}
						}
						commands.add(new MacroCmd(entry++, type, d1, d2, cmd));
					}
				}
				_macroPresets.put(macroId, new Macro(macroId, icon, name, description, acronym, commands));
			}
		}
	}
	
	/**
	 * Parses a node an create a shortcut from it.
	 * @param pageId the page ID
	 * @param b the node to parse
	 * @return the new shortcut
	 */
	private Shortcut createShortcut(int pageId, Node b)
	{
		NamedNodeMap attrs = b.getAttributes();
		int slotId = parseInteger(attrs, "slotId");
		ShortcutType shortcutType = parseEnum(attrs, ShortcutType.class, "shortcutType");
		int shortcutId = parseInteger(attrs, "shortcutId");
		int shortcutLevel = parseInteger(attrs, "shortcutLevel", 0);
		int characterType = parseInteger(attrs, "characterType", 0);
		return new Shortcut(slotId, pageId, shortcutType, shortcutId, shortcutLevel, 0, characterType);
	}
	
	/**
	 * Gets the shortcut list.
	 * @param cId the class ID for the shortcut list
	 * @return the shortcut list for the give class ID
	 */
	public List<Shortcut> getShortcutList(ClassId cId)
	{
		return _initialShortcutData.get(cId);
	}
	
	/**
	 * Gets the shortcut list.
	 * @param cId the class ID for the shortcut list
	 * @return the shortcut list for the give class ID
	 */
	public List<Shortcut> getShortcutList(int cId)
	{
		return _initialShortcutData.get(ClassId.getClassId(cId));
	}
	
	/**
	 * Gets the global shortcut list.
	 * @return the global shortcut list
	 */
	public List<Shortcut> getGlobalMacroList()
	{
		return _initialGlobalShortcutList;
	}
	
	/**
	 * Register all the available shortcuts for the given player.
	 * @param player the player
	 */
	public void registerAllShortcuts(Player player)
	{
		if (player == null)
		{
			return;
		}
		
		// Register global shortcuts.
		foreach (Shortcut shortcut in _initialGlobalShortcutList)
		{
			int shortcutId = shortcut.getId();
			switch (shortcut.getType())
			{
				case ShortcutType.ITEM:
				{
					Item item = player.getInventory().getItemByItemId(shortcutId);
					if (item == null)
					{
						continue;
					}
					shortcutId = item.getObjectId();
					break;
				}
				case ShortcutType.SKILL:
				{
					if (!player.getSkills().containsKey(shortcutId))
					{
						continue;
					}
					break;
				}
				case ShortcutType.MACRO:
				{
					Macro macro = _macroPresets.get(shortcutId);
					if (macro == null)
					{
						continue;
					}
					player.registerMacro(macro);
					break;
				}
			}
			
			// Register shortcut
			Shortcut newShortcut = new Shortcut(shortcut.getSlot(), shortcut.getPage(), shortcut.getType(), shortcutId, shortcut.getLevel(), shortcut.getSubLevel(), shortcut.getCharacterType());
			player.sendPacket(new ShortCutRegister(newShortcut, player));
			player.registerShortCut(newShortcut);
		}
		
		// Register class specific shortcuts.
		if (_initialShortcutData.containsKey(player.getClassId()))
		{
			foreach (Shortcut shortcut in _initialShortcutData.get(player.getClassId()))
			{
				int shortcutId = shortcut.getId();
				switch (shortcut.getType())
				{
					case ShortcutType.ITEM:
					{
						Item item = player.getInventory().getItemByItemId(shortcutId);
						if (item == null)
						{
							continue;
						}
						shortcutId = item.getObjectId();
						break;
					}
					case ShortcutType.SKILL:
					{
						if (!player.getSkills().containsKey(shortcut.getId()))
						{
							continue;
						}
						break;
					}
					case ShortcutType.MACRO:
					{
						Macro macro = _macroPresets.get(shortcutId);
						if (macro == null)
						{
							continue;
						}
						player.registerMacro(macro);
						break;
					}
				}
				// Register shortcut
				Shortcut newShortcut = new Shortcut(shortcut.getSlot(), shortcut.getPage(), shortcut.getType(), shortcutId, shortcut.getLevel(), shortcut.getSubLevel(), shortcut.getCharacterType());
				player.sendPacket(new ShortCutRegister(newShortcut, player));
				player.registerShortCut(newShortcut);
			}
		}
	}
	
	/**
	 * Gets the single instance of InitialEquipmentData.
	 * @return single instance of InitialEquipmentData
	 */
	public static InitialShortcutData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly InitialShortcutData INSTANCE = new();
	}
}
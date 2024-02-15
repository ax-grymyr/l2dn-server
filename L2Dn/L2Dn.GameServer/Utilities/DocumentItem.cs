using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Items;
using NLog;

namespace L2Dn.GameServer.Utilities;

public class DocumentItem: DocumentBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(DocumentItem));
	
	private DocumentItemDataHolder _currentItem = null;
	private readonly List<ItemTemplate> _itemsInFile = new();
	
	private class DocumentItemDataHolder
	{
		public DocumentItemDataHolder()
		{
		}
		
		int id;
		String type;
		StatSet set;
		int currentLevel;
		ItemTemplate item;
	}
	
	public DocumentItem(string filePath): base(filePath)
	{
	}
	
	protected override StatSet getStatSet()
	{
		return _currentItem.set;
	}
	
	protected override String getTableValue(String name)
	{
		return _tables.get(name)[_currentItem.currentLevel];
	}
	
	protected override string getTableValue(String name, int idx)
	{
		return _tables.get(name)[idx - 1];
	}
	
	protected override void parseDocument(Document doc)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("item".equalsIgnoreCase(d.getNodeName()))
					{
						try
						{
							_currentItem = new DocumentItemDataHolder();
							parseItem(d);
							_itemsInFile.add(_currentItem.item);
							resetTable();
						}
						catch (Exception e)
						{
							LOGGER.log(Level.WARNING, "Cannot create item " + _currentItem.id, e);
						}
					}
				}
			}
		}
	}
	
	private void parseItem(Node node)
	{
		Node n = node;
		int itemId = Integer.parseInt(n.getAttributes().getNamedItem("id").getNodeValue());
		String className = n.getAttributes().getNamedItem("type").getNodeValue();
		String itemName = n.getAttributes().getNamedItem("name").getNodeValue();
		String additionalName = n.getAttributes().getNamedItem("additionalName") != null ? n.getAttributes().getNamedItem("additionalName").getNodeValue() : null;
		_currentItem.id = itemId;
		_currentItem.type = className;
		_currentItem.set = new StatSet();
		_currentItem.set.set("item_id", itemId);
		_currentItem.set.set("name", itemName);
		_currentItem.set.set("additionalName", additionalName);
		
		Node first = n.getFirstChild();
		for (n = first; n != null; n = n.getNextSibling())
		{
			if ("table".equalsIgnoreCase(n.getNodeName()))
			{
				if (_currentItem.item != null)
				{
					throw new IllegalStateException("Item created but table node found! Item " + itemId);
				}
				parseTable(n);
			}
			else if ("set".equalsIgnoreCase(n.getNodeName()))
			{
				if (_currentItem.item != null)
				{
					throw new IllegalStateException("Item created but set node found! Item " + itemId);
				}
				parseBeanSet(n, _currentItem.set, 1);
			}
			else if ("stats".equalsIgnoreCase(n.getNodeName()))
			{
				makeItem();
				for (Node b = n.getFirstChild(); b != null; b = b.getNextSibling())
				{
					if ("stat".equalsIgnoreCase(b.getNodeName()))
					{
						Stat type = Stat.valueOfXml(b.getAttributes().getNamedItem("type").getNodeValue());
						double value = Double.parseDouble(b.getTextContent());
						_currentItem.item.addFunctionTemplate(new FuncTemplate(null, null, "add", 0x00, type, value));
					}
				}
			}
			else if ("skills".equalsIgnoreCase(n.getNodeName()))
			{
				makeItem();
				for (Node b = n.getFirstChild(); b != null; b = b.getNextSibling())
				{
					if ("skill".equalsIgnoreCase(b.getNodeName()))
					{
						int id = parseInteger(b.getAttributes(), "id");
						int level = parseInteger(b.getAttributes(), "level");
						ItemSkillType type = parseEnum(b.getAttributes(), ItemSkillType.class, "type", ItemSkillType.NORMAL);
						int chance = parseInteger(b.getAttributes(), "type_chance", 100);
						int value = parseInteger(b.getAttributes(), "type_value", 0);
						_currentItem.item.addSkill(new ItemSkillHolder(id, level, type, chance, value));
					}
				}
			}
			else if ("capsuled_items".equalsIgnoreCase(n.getNodeName()))
			{
				makeItem();
				for (Node b = n.getFirstChild(); b != null; b = b.getNextSibling())
				{
					if ("item".equals(b.getNodeName()))
					{
						int id = parseInteger(b.getAttributes(), "id");
						long min = parseLong(b.getAttributes(), "min");
						long max = parseLong(b.getAttributes(), "max");
						double chance = parseDouble(b.getAttributes(), "chance");
						int minEnchant = parseInteger(b.getAttributes(), "minEnchant", 0);
						int maxEnchant = parseInteger(b.getAttributes(), "maxEnchant", 0);
						_currentItem.item.addCapsuledItem(new ExtractableProduct(id, min, max, chance, minEnchant, maxEnchant));
					}
				}
			}
			else if ("cond".equalsIgnoreCase(n.getNodeName()))
			{
				makeItem();
				Condition condition = parseCondition(n.getFirstChild(), _currentItem.item);
				Node msg = n.getAttributes().getNamedItem("msg");
				Node msgId = n.getAttributes().getNamedItem("msgId");
				if ((condition != null) && (msg != null))
				{
					condition.setMessage(msg.getNodeValue());
				}
				else if ((condition != null) && (msgId != null))
				{
					condition.setMessageId(Integer.decode(getValue(msgId.getNodeValue(), null)));
					Node addName = n.getAttributes().getNamedItem("addName");
					if ((addName != null) && (Integer.decode(getValue(msgId.getNodeValue(), null)) > 0))
					{
						condition.addName();
					}
				}
				_currentItem.item.attachCondition(condition);
			}
		}
		// bah! in this point item doesn't have to be still created
		makeItem();
	}
	
	private void makeItem()
	{
		// If item exists just reload the data.
		if (_currentItem.item != null)
		{
			_currentItem.item.set(_currentItem.set);
			return;
		}
		
		try
		{
			Constructor<?> itemClass = Class.forName("org.l2jmobius.gameserver.model.item." + _currentItem.type).getConstructor(StatSet.class);
			_currentItem.item = (ItemTemplate) itemClass.newInstance(_currentItem.set);
		}
		catch (Exception e)
		{
			throw new InvocationTargetException(e);
		}
	}
	
	public List<ItemTemplate> getItemList()
	{
		return _itemsInFile;
	}
	
	public override void load()
	{
	}
	
	public override void parseDocument(Document doc, File f)
	{
	}
}
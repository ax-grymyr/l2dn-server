using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Henna;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Serenitty
 */
public class HennaPatternPotentialData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(HennaPatternPotentialData));
	
	private readonly Map<int, int> _potenExpTable = new();
	private readonly Map<int, DyePotentialFee> _potenFees = new();
	private readonly Map<int, DyePotential> _potentials = new();
	private readonly List<ItemHolder> _enchancedReset = new();
	
	private int MAX_POTEN_LEVEL = 0;
	private int MAX_POTEN_EXP = 0;
	
	protected HennaPatternPotentialData()
	{
		load();
	}
	
	public void load()
	{
		_potenFees.clear();
		_potenExpTable.clear();
		_potentials.clear();
		_enchancedReset.Clear();
		parseDatapackFile("data/stats/hennaPatternPotential.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _potenFees.size() + " dye pattern fee data.");
		
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node m = doc.getFirstChild(); m != null; m = m.getNextSibling())
		{
			if ("list".equals(m.getNodeName()))
			{
				for (Node k = m.getFirstChild(); k != null; k = k.getNextSibling())
				{
					switch (k.getNodeName())
					{
						case "enchantFees":
						{
							for (Node n = k.getFirstChild(); n != null; n = n.getNextSibling())
							{
								if ("fee".equals(n.getNodeName()))
								{
									NamedNodeMap attrs = n.getAttributes();
									Node att;
									StatSet set = new StatSet();
									for (int i = 0; i < attrs.getLength(); i++)
									{
										att = attrs.item(i);
										set.set(att.getNodeName(), att.getNodeValue());
									}
									
									int step = parseInteger(attrs, "step");
									int itemId = 0;
									long itemCount = 0;
									int dailyCount = 0;
									Map<int, Double> enchantExp = new();
									List<ItemHolder> items = new();
									for (Node b = n.getFirstChild(); b != null; b = b.getNextSibling())
									{
										attrs = b.getAttributes();
										switch (b.getNodeName())
										{
											case "requiredItem":
											{
												itemId = parseInteger(attrs, "id");
												itemCount = Parse(attrs, "count", 1L);
												items.add(new ItemHolder(itemId, itemCount));
												break;
											}
											case "dailyCount":
											{
												dailyCount = int.Parse(b.getTextContent());
												break;
											}
											case "enchantExp":
											{
												enchantExp.put(parseInteger(attrs, "count"), parseDouble(attrs, "chance"));
												break;
											}
										}
									}
									_potenFees.put(step, new DyePotentialFee(step, items, dailyCount, enchantExp));
								}
							}
							break;
						}
						case "resetCount":
						{
							for (Node n = k.getFirstChild(); n != null; n = n.getNextSibling())
							{
								if ("reset".equalsIgnoreCase(n.getNodeName()))
								{
									StatSet set = new StatSet(parseAttributes(n));
									int itemId = set.getInt("itemid");
									int itemCount = set.getInt("count");
									if (ItemData.getInstance().getTemplate(itemId) == null)
									{
										LOGGER.Info(GetType().Name + ": Item with id " + itemId + " does not exist.");
									}
									else
									{
										_enchancedReset.add(new ItemHolder(itemId, itemCount));
									}
								}
							}
							break;
						}
						case "experiencePoints":
						{
							for (Node n = k.getFirstChild(); n != null; n = n.getNextSibling())
							{
								if ("hiddenPower".equals(n.getNodeName()))
								{
									NamedNodeMap attrs = n.getAttributes();
									Node att;
									StatSet set = new StatSet();
									for (int i = 0; i < attrs.getLength(); i++)
									{
										att = attrs.item(i);
										set.set(att.getNodeName(), att.getNodeValue());
									}
									
									int level = parseInteger(attrs, "level");
									int exp = parseInteger(attrs, "exp");
									_potenExpTable.put(level, exp);
									if (MAX_POTEN_LEVEL < level)
									{
										MAX_POTEN_LEVEL = level;
									}
									if (MAX_POTEN_EXP < exp)
									{
										MAX_POTEN_EXP = exp;
									}
								}
							}
							break;
						}
						case "hiddenPotentials":
						{
							for (Node n = k.getFirstChild(); n != null; n = n.getNextSibling())
							{
								if ("poten".equals(n.getNodeName()))
								{
									NamedNodeMap attrs = n.getAttributes();
									Node att;
									StatSet set = new StatSet();
									for (int i = 0; i < attrs.getLength(); i++)
									{
										att = attrs.item(i);
										set.set(att.getNodeName(), att.getNodeValue());
									}
									
									int id = parseInteger(attrs, "id");
									int slotId = parseInteger(attrs, "slotId");
									int maxSkillLevel = parseInteger(attrs, "maxSkillLevel");
									int skillId = parseInteger(attrs, "skillId");
									_potentials.put(id, new DyePotential(id, slotId, skillId, maxSkillLevel));
								}
							}
							break;
						}
					}
				}
			}
		}
	}
	
	public DyePotentialFee getFee(int step)
	{
		return _potenFees.get(step);
	}
	
	public int getMaxPotenEnchantStep()
	{
		return _potenFees.size();
	}
	
	public List<ItemHolder> getEnchantReset()
	{
		return _enchancedReset;
	}
	
	public int getExpForLevel(int level)
	{
		return _potenExpTable.get(level);
	}
	
	public int getMaxPotenLevel()
	{
		return MAX_POTEN_LEVEL;
	}
	
	public int getMaxPotenExp()
	{
		return MAX_POTEN_EXP;
	}
	
	public DyePotential getPotential(int potenId)
	{
		return _potentials.get(potenId);
	}
	
	public Skill getPotentialSkill(int potenId, int slotId, int level)
	{
		DyePotential potential = _potentials.get(potenId);
		if (potential == null)
		{
			return null;
		}
		if (potential.getSlotId() == slotId)
		{
			return potential.getSkill(level);
		}
		return null;
	}
	
	public ICollection<int> getSkillIdsBySlotId(int slotId)
	{
		List<int> skillIds = new();
		foreach (DyePotential potential in _potentials.values())
		{
			if (potential.getSlotId() == slotId)
			{
				skillIds.add(potential.getSkillId());
			}
		}
		return skillIds;
	}
	
	public static HennaPatternPotentialData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly HennaPatternPotentialData INSTANCE = new();
	}
}
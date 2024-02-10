using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Transforms;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * Transformation data.
 * @author UnAfraid
 */
public class TransformData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(TransformData));
	
	private readonly Map<int, Transform> _transformData = new();
	
	protected TransformData()
	{
		load();
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)] 
	public void load()
	{
		_transformData.clear();
		parseDatapackDirectory("data/stats/transformations", false);
		LOGGER.Info(GetType().Name + ": Loaded " + _transformData.size() + " transform templates.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("transform".equalsIgnoreCase(d.getNodeName()))
					{
						NamedNodeMap attrs = d.getAttributes();
						StatSet set = new StatSet();
						for (int i = 0; i < attrs.getLength(); i++)
						{
							Node att = attrs.item(i);
							set.set(att.getNodeName(), att.getNodeValue());
						}
						Transform transform = new Transform(set);
						for (Node cd = d.getFirstChild(); cd != null; cd = cd.getNextSibling())
						{
							bool isMale = "Male".equalsIgnoreCase(cd.getNodeName());
							if ("Male".equalsIgnoreCase(cd.getNodeName()) || "Female".equalsIgnoreCase(cd.getNodeName()))
							{
								TransformTemplate templateData = null;
								for (Node z = cd.getFirstChild(); z != null; z = z.getNextSibling())
								{
									switch (z.getNodeName())
									{
										case "common":
										{
											for (Node s = z.getFirstChild(); s != null; s = s.getNextSibling())
											{
												switch (s.getNodeName())
												{
													case "base":
													case "stats":
													case "defense":
													case "magicDefense":
													case "collision":
													case "moving":
													{
														attrs = s.getAttributes();
														for (int i = 0; i < attrs.getLength(); i++)
														{
															Node att = attrs.item(i);
															set.set(att.getNodeName(), att.getNodeValue());
														}
														break;
													}
												}
											}
											templateData = new TransformTemplate(set);
											transform.setTemplate(isMale, templateData);
											break;
										}
										case "skills":
										{
											if (templateData == null)
											{
												templateData = new TransformTemplate(set);
												transform.setTemplate(isMale, templateData);
											}
											for (Node s = z.getFirstChild(); s != null; s = s.getNextSibling())
											{
												if ("skill".equals(s.getNodeName()))
												{
													attrs = s.getAttributes();
													int skillId = parseInteger(attrs, "id");
													int skillLevel = parseInteger(attrs, "level");
													templateData.addSkill(new SkillHolder(skillId, skillLevel));
												}
											}
											break;
										}
										case "actions":
										{
											if (templateData == null)
											{
												templateData = new TransformTemplate(set);
												transform.setTemplate(isMale, templateData);
											}
											set.set("actions", z.getTextContent());
											int[] actions = set.getIntArray("actions", " ");
											templateData.setBasicActionList(actions);
											break;
										}
										case "additionalSkills":
										{
											if (templateData == null)
											{
												templateData = new TransformTemplate(set);
												transform.setTemplate(isMale, templateData);
											}
											for (Node s = z.getFirstChild(); s != null; s = s.getNextSibling())
											{
												if ("skill".equals(s.getNodeName()))
												{
													attrs = s.getAttributes();
													int skillId = parseInteger(attrs, "id");
													int skillLevel = parseInteger(attrs, "level");
													int minLevel = parseInteger(attrs, "minLevel");
													templateData.addAdditionalSkill(new AdditionalSkillHolder(skillId, skillLevel, minLevel));
												}
											}
											break;
										}
										case "items":
										{
											if (templateData == null)
											{
												templateData = new TransformTemplate(set);
												transform.setTemplate(isMale, templateData);
											}
											for (Node s = z.getFirstChild(); s != null; s = s.getNextSibling())
											{
												if ("item".equals(s.getNodeName()))
												{
													attrs = s.getAttributes();
													int itemId = parseInteger(attrs, "id");
													bool allowed = parseBoolean(attrs, "allowed");
													templateData.addAdditionalItem(new AdditionalItemHolder(itemId, allowed));
												}
											}
											break;
										}
										case "levels":
										{
											if (templateData == null)
											{
												templateData = new TransformTemplate(set);
												transform.setTemplate(isMale, templateData);
											}
											
											StatSet levelsSet = new StatSet();
											for (Node s = z.getFirstChild(); s != null; s = s.getNextSibling())
											{
												if ("level".equals(s.getNodeName()))
												{
													attrs = s.getAttributes();
													for (int i = 0; i < attrs.getLength(); i++)
													{
														Node att = attrs.item(i);
														levelsSet.set(att.getNodeName(), att.getNodeValue());
													}
												}
											}
											templateData.addLevelData(new TransformLevelData(levelsSet));
											break;
										}
									}
								}
							}
						}
						_transformData.put(transform.getId(), transform);
					}
				}
			}
		}
	}
	
	public Transform getTransform(int id)
	{
		return _transformData.get(id);
	}
	
	public static TransformData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly TransformData INSTANCE = new();
	}
}
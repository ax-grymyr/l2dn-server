using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Transforms;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * Transformation data.
 * @author UnAfraid
 */
public class TransformData: DataReaderBase
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
		
		LoadXmlDocuments(DataFileLocation.Data, "stats/transformations").ForEach(t =>
		{
			t.Document.Elements("list").Elements("transform").ForEach(x => loadElement(t.FilePath, x));
		});
		
		LOGGER.Info(GetType().Name + ": Loaded " + _transformData.size() + " transform templates.");
	}
	
	private void loadElement(string filePath, XElement element)
	{
		StatSet set = new StatSet(element);
		Transform transform = new Transform(set);

		XElement maleElement = element.Elements("Male").Single();
		XElement femaleElement = element.Elements("Female").Single();

		transform.setTemplate(true, loadTemplate(maleElement));
		transform.setTemplate(false, loadTemplate(femaleElement));
		
		_transformData.put(transform.getId(), transform);
	}

	private TransformTemplate loadTemplate(XElement element)
	{
		TransformTemplate templateData = null;
		StatSet set = new();
		
		foreach (XElement z in element.Elements())
		{
			switch (z.Name.LocalName)
			{
				case "common":
				{
					foreach (XElement s in z.Elements())
					{
						switch (s.Name.LocalName)
						{
							case "base":
							case "stats":
							case "defense":
							case "magicDefense":
							case "collision":
							case "moving":
							{
								foreach (XAttribute attribute in s.Attributes())
									set.set(attribute.Name.LocalName, attribute.Value);

								break;
							}
						}
					}

					templateData = new TransformTemplate(set);
					break;
				}
				case "skills":
				{
					if (templateData == null)
						templateData = new TransformTemplate(set);

					z.Elements("skill").ForEach(s =>
					{
						int skillId = s.GetAttributeValueAsInt32("id");
						int skillLevel = s.GetAttributeValueAsInt32("level");
						templateData.addSkill(new SkillHolder(skillId, skillLevel));
					});

					break;
				}
				case "actions":
				{
					if (templateData == null)
						templateData = new TransformTemplate(set);

					set.set("actions", z.Value);
					int[] actions = set.getIntArray("actions", " ");
					templateData.setBasicActionList(actions.ToImmutableArray());
					break;
				}
				case "additionalSkills":
				{
					if (templateData == null)
						templateData = new TransformTemplate(set);

					z.Elements("skill").ForEach(s =>
					{
						int skillId = s.GetAttributeValueAsInt32("id");
						int skillLevel = s.GetAttributeValueAsInt32("level");
						int minLevel = s.GetAttributeValueAsInt32("minLevel");
						templateData.addAdditionalSkill(new AdditionalSkillHolder(skillId, skillLevel, minLevel));
					});

					break;
				}
				case "items":
				{
					if (templateData == null)
						templateData = new TransformTemplate(set);

					z.Elements("item").ForEach(s =>
					{
						int itemId = s.GetAttributeValueAsInt32("id");
						bool allowed = s.GetAttributeValueAsBoolean("allowed");
						templateData.addAdditionalItem(new AdditionalItemHolder(itemId, allowed));
					});

					break;
				}
				case "levels":
				{
					if (templateData == null)
						templateData = new TransformTemplate(set);

					StatSet levelsSet = new StatSet();

					z.Elements("level").Attributes().ForEach(a =>
					{
						levelsSet.set(a.Name.LocalName, a.Value);
					});

					templateData.addLevelData(new TransformLevelData(levelsSet));
					break;
				}
			}
		}

		if (templateData is null)
			throw new InvalidOperationException("Invalid transformation data");
		
		return templateData;
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
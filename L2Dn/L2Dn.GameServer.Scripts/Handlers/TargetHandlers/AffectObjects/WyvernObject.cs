using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.StaticData;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers.AffectObjects;

/**
 * @author Nik
 */
public class WyvernObject: IAffectObjectHandler
{
	public bool checkAffectedObject(Creature creature, Creature target)
	{
		// TODO Check if this is proper. Not sure if this is the object we are looking for.
		return CategoryData.Instance.IsInCategory(CategoryType.WYVERN_GROUP, target.Id);
	}

	public AffectObject getAffectObjectType()
	{
		return AffectObject.WYVERN_OBJECT;
	}
}
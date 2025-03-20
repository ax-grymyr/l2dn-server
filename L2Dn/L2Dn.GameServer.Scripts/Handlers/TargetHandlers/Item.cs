using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers;

/**
 * TODO: Target item.
 * @author Nik
 */
public class Item: ITargetTypeHandler
{
	public TargetType getTargetType()
	{
		return TargetType.ITEM;
	}

	public WorldObject? getTarget(Creature creature, WorldObject? selectedTarget, Skill skill, bool forceUse, bool dontMove, bool sendMessage)
	{
		return (selectedTarget != null) && selectedTarget.isItem() ? selectedTarget : null;
	}
}
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Handlers;

public interface IActionShiftHandler
{
	bool action(Player player, WorldObject target, bool interact);
	
	InstanceType getInstanceType();
}
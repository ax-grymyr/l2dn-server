using L2Dn.GameServer.Model.Punishment;

namespace L2Dn.GameServer.Handlers;

/**
 * @author UnAfraid
 */
public interface IPunishmentHandler
{
	void onStart(PunishmentTask task);
	
	void onEnd(PunishmentTask task);
	
	PunishmentType getType();
}
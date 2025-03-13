using L2Dn.GameServer.Data;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.StaticData;

namespace L2Dn.GameServer.Scripts.Handlers.PlayerActions;

/**
 * Bot Report button player action handler.
 * @author Nik
 */
public class BotReport: IPlayerActionHandler
{
	public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
	{
		if (Config.BOTREPORT_ENABLE)
		{
			BotReportTable.getInstance().reportBot(player);
		}
		else
		{
			player.sendMessage("This feature is disabled.");
		}
	}

	public bool isPetAction()
	{
		return false;
	}
}
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

/**
 * @author UnAfraid
 */
public class TutorialClose: IBypassHandler
{
    private static readonly string[] COMMANDS = ["tutorial_close"];

	public bool useBypass(string command, Player player, Creature? target)
	{
		player.sendPacket(TutorialCloseHtmlPacket.STATIC_PACKET);
		player.getClient()?.HtmlActionValidator.ClearActions(HtmlActionScope.TUTORIAL_HTML);
		return false;
	}

	public string[] getBypassList()
	{
		return COMMANDS;
	}
}
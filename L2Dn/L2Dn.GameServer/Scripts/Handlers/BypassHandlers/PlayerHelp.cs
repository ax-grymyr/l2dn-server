using L2Dn.GameServer.Data;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

public class PlayerHelp: IBypassHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(PlayerHelp));

	private static readonly string[] COMMANDS =
	{
		"player_help"
	};
	
	public bool useBypass(String command, Player player, Creature target)
	{
		try
		{
			if (command.Length < 13)
			{
				return false;
			}
			
			String path = command.Substring(12);
			if (path.Contains(".."))
			{
				return false;
			}
			
			StringTokenizer st = new StringTokenizer(path);
			String[] cmd = st.nextToken().Split("#");

			HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, "html/help/" + cmd[0]);
			NpcHtmlMessagePacket html;
			if (cmd.Length > 1)
			{
				int itemId = int.Parse(cmd[1]);
				html = new NpcHtmlMessagePacket(0, itemId, helper);
			}
			else
			{
				html = new NpcHtmlMessagePacket(helper);
			}
			
			player.sendPacket(html);
		}
		catch (Exception e)
		{
			_logger.Warn("Exception in " + GetType().Name, e);
		}
		return true;
	}
	
	public String[] getBypassList()
	{
		return COMMANDS;
	}
}
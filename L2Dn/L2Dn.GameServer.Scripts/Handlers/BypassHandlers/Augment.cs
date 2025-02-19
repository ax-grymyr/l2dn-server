using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Variations;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

public class Augment: IBypassHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(Augment));

	private static readonly string[] COMMANDS = ["Augment"];

	public bool useBypass(string command, Player player, Creature? target)
	{
		if (target is null || !target.isNpc())
		{
			return false;
		}

		try
		{
			switch (int.Parse(command.Substring(8, 9).Trim()))
			{
				case 1:
				{
					player.sendPacket(ExShowVariationMakeWindowPacket.STATIC_PACKET);
					return true;
				}
				case 2:
				{
					player.sendPacket(ExShowVariationCancelWindowPacket.STATIC_PACKET);
					return true;
				}
			}
		}
		catch (Exception e)
		{
			_logger.Warn($"Exception in {GetType().Name}: {e}");
		}

		return false;
	}

	public string[] getBypassList()
	{
		return COMMANDS;
	}
}
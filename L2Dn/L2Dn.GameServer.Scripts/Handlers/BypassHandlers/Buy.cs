using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

public class Buy: IBypassHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(Buy));

	private static readonly string[] COMMANDS =
	{
		"Buy"
	};
	
	public bool useBypass(string command, Player player, Creature target)
	{
		if (!(target is Merchant))
		{
			return false;
		}
		
		try
		{
			StringTokenizer st = new StringTokenizer(command, " ");
			st.nextToken();
			
			if (st.countTokens() < 1)
			{
				return false;
			}
			
			((Merchant) target).showBuyWindow(player, int.Parse(st.nextToken()));
			return true;
		}
		catch (Exception e)
		{
			_logger.Warn("Exception in " + GetType().Name, e);
		}
		return false;
	}
	
	public string[] getBypassList()
	{
		return COMMANDS;
	}
}
using System.Runtime.CompilerServices;
using L2Dn.GameServer.Scripts.Handlers.BypassHandlers;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * @author nBd, UnAfraid
 */
public class BypassHandler: IHandler<IBypassHandler, String>
{
	private readonly Map<String, IBypassHandler> _datatable;
	
	protected BypassHandler()
	{
		_datatable = new();
		
		registerHandler(new Augment());
		registerHandler(new Buy());
		registerHandler(new ChatLink());
		registerHandler(new ClanWarehouse());
		registerHandler(new EnsoulWindow());
		registerHandler(new FindPvP());
		registerHandler(new Freight());
		registerHandler(new ItemAuctionLink());
		registerHandler(new Link());
		registerHandler(new Multisell());
		registerHandler(new NpcViewMod());
		registerHandler(new Observation());
		registerHandler(new PetExtractWindow());
		registerHandler(new QuestLink());
		registerHandler(new PlayerHelp());
		registerHandler(new PrivateWarehouse());
		registerHandler(new ReleaseAttribute());
		registerHandler(new SkillList());
		registerHandler(new SupportBlessing());
		registerHandler(new SupportMagic());
		registerHandler(new TerritoryStatus());
		registerHandler(new TutorialClose());
		registerHandler(new UpgradeEquipment());
		registerHandler(new VoiceCommand());
		registerHandler(new Wear());
	}
	
	public void registerHandler(IBypassHandler handler)
	{
		foreach (String element in handler.getBypassList())
		{
			_datatable.put(element.ToLower(), handler);
		}
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void removeHandler(IBypassHandler handler)
	{
		foreach (String element in handler.getBypassList())
		{
			_datatable.remove(element.ToLower());
		}
	}
	
	public IBypassHandler getHandler(String commandValue)
	{
		String command = commandValue;
		if (command.Contains(" "))
		{
			command = command.Substring(0, command.IndexOf(' '));
		}
		return _datatable.get(command.ToLower());
	}
	
	public int size()
	{
		return _datatable.size();
	}
	
	public static BypassHandler getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly BypassHandler INSTANCE = new BypassHandler();
	}
}
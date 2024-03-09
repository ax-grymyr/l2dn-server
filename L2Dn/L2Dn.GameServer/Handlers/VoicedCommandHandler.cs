using System.Runtime.CompilerServices;
using L2Dn.GameServer.Scripts.Handlers.UserCommandHandlers;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * @author UnAfraid
 */
public class VoicedCommandHandler: IHandler<IVoicedCommandHandler, String>
{
	private readonly Map<String, IVoicedCommandHandler> _datatable;
	
	protected VoicedCommandHandler()
	{
		_datatable = new();
		registerHandler(new ExperienceGain());
	}
	
	public void registerHandler(IVoicedCommandHandler handler)
	{
		foreach (String id in handler.getVoicedCommandList())
		{
			_datatable.put(id, handler);
		}
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void removeHandler(IVoicedCommandHandler handler)
	{
		foreach (String id in handler.getVoicedCommandList())
		{
			_datatable.remove(id);
		}
	}
	
	public IVoicedCommandHandler getHandler(String voicedCommand)
	{
		return _datatable.get(voicedCommand.Contains(" ") ? voicedCommand.Substring(0, voicedCommand.IndexOf(' ')) : voicedCommand);
	}
	
	public int size()
	{
		return _datatable.size();
	}
	
	public static VoicedCommandHandler getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly VoicedCommandHandler INSTANCE = new VoicedCommandHandler();
	}
}
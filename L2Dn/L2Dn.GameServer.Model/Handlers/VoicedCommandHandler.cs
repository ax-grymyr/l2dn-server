using System.Runtime.CompilerServices;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * @author UnAfraid
 */
public class VoicedCommandHandler: IHandler<IVoicedCommandHandler, string>
{
	private readonly Map<string, IVoicedCommandHandler> _datatable;
	
	protected VoicedCommandHandler()
	{
		_datatable = new();
	}
	
	public void registerHandler(IVoicedCommandHandler handler)
	{
		foreach (string id in handler.getVoicedCommandList())
		{
			_datatable.put(id, handler);
		}
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void removeHandler(IVoicedCommandHandler handler)
	{
		foreach (string id in handler.getVoicedCommandList())
		{
			_datatable.remove(id);
		}
	}
	
	public IVoicedCommandHandler getHandler(string voicedCommand)
	{
		return _datatable.get(voicedCommand.Contains(" ") ? voicedCommand.Substring(0, voicedCommand.IndexOf(' ')) : voicedCommand);
	}
	
	public int size()
	{
		return _datatable.Count;
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
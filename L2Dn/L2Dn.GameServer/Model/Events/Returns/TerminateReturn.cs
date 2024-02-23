namespace L2Dn.GameServer.Model.Events.Returns;

public class TerminateReturn: AbstractEventReturn
{
	private readonly bool _terminate;

	public TerminateReturn(bool terminate, bool @override, bool abort)
		: base(@override, abort)
	{
		_terminate = terminate;
	}

	/**
	 * @return {@code true} if execution has to be terminated, {@code false} otherwise.
	 */
	public bool terminate()
	{
		return _terminate;
	}
}
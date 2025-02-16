namespace L2Dn.GameServer.AI;

/// <summary>
/// Class for AI action after some event.
/// Has 2 array list for "work" and "break".
/// </summary>
public sealed class NextAction
{
	private readonly List<CtrlEvent> _events;
	private readonly List<CtrlIntention> _intentions;
	private readonly Action _action;

	/// <summary>
	/// Initializes a new instance of the <see cref="NextAction"/> class.
	/// </summary>
	/// <param name="events">The events.</param>
	/// <param name="intentions">The intentions.</param>
	/// <param name="action">The action.</param>
	public NextAction(List<CtrlEvent> events, List<CtrlIntention> intentions, Action action)
	{
		_events = events;
		_intentions = intentions;
		_action = action;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="NextAction"/> class.
	/// </summary>
	/// <param name="event">The event.</param>
	/// <param name="intention">The intention.</param>
	/// <param name="action">The action.</param>
	public NextAction(CtrlEvent @event, CtrlIntention intention, Action action)
	{
		_events = [@event];
		_intentions = [intention];
		_action = action;
	}

	public List<CtrlEvent> Events => _events;
	public List<CtrlIntention> Intentions => _intentions;
	public Action Action => _action;

	/// <summary>
	/// Invoke action.
	/// </summary>
	public void DoAction() => _action();
}
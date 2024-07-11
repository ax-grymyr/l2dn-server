using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Quests;

/**
 * Abstract event class.
 * @author JIV
 */
public abstract class Event: Quest
{
	public Event(): base(-1)
	{
	}
	
	public abstract bool eventStart(Player eventMaker);
	public abstract bool eventStop();
	public abstract bool eventBypass(Player player, string bypass);
}
namespace L2Dn.GameServer.Model.Quests;

/**
 * This class merely enumerates the three necessary states for all quests:<br>
 * <ul>
 * <li>CREATED: a quest state is created but the quest is not yet accepted.</li>
 * <li>STARTED: the player has accepted the quest. Quest is currently in progress</li>
 * <li>COMPLETED: the quest has been completed.</li>
 * </ul>
 * In addition, this class defines two functions for lookup and inverse lookup of the state given a name.<br>
 * This is useful only for saving the state values into the database with a more readable form and then being able to read the string back and remap them to their correct states.<br>
 * All quests have these and only these states.
 * @author Luis Arias; version 2 by Fulminus
 */
public class State
{
	public const byte CREATED = 0;
	public const byte STARTED = 1;
	public const byte COMPLETED = 2;
	
	/**
	 * Get the quest state's string representation from its byte value.
	 * @param state the byte value of the state
	 * @return the String representation of the quest state (default: Start)
	 */
	public static String getStateName(byte state)
	{
		switch (state)
		{
			case 1:
			{
				return "Started";
			}
			case 2:
			{
				return "Completed";
			}
			default:
			{
				return "Start";
			}
		}
	}
	
	/**
	 * Get the quest state's byte value from its string representation.
	 * @param statename the String representation of the state
	 * @return the byte value of the quest state (default: 0)
	 */
	public static byte getStateId(String statename)
	{
		switch (statename)
		{
			case "Started":
			{
				return 1;
			}
			case "Completed":
			{
				return 2;
			}
			default:
			{
				return 0;
			}
		}
	}
}
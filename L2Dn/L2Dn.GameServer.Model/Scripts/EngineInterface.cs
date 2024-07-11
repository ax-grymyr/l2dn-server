namespace L2Dn.GameServer.Scripts;

/**
 * @author Luis Arias
 */
public interface EngineInterface
{
	void addEventDrop(int[] items, int[] count, double chance, DateRange range);
	
	void onPlayerLogin(string message, DateRange range);
}

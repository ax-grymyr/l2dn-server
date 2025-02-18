namespace L2Dn.GameServer.Handlers;

/**
 * @author UnAfraid
 * @param <K>
 * @param <V>
 */
public interface IHandler<K, V>
{
	void registerHandler(K handler);

	void removeHandler(K handler);

	K? getHandler(V val);

	int size();
}
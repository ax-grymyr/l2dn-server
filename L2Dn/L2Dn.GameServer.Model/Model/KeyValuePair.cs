namespace L2Dn.GameServer.Model;

public class KeyValuePair<K, V>
{
    private readonly K _key;
    private readonly V _value;
	
    public KeyValuePair(K key, V value)
    {
        _key = key;
        _value = value;
    }
	
    public K getKey()
    {
        return _key;
    }
	
    public V getValue()
    {
        return _value;
    }
}

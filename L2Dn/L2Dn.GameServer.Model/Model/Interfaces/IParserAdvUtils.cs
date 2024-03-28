namespace L2Dn.GameServer.Model.Interfaces;

/**
 * More advanced interface for parsers.<br>
 * Allows usage of get methods without fall back value.<br>
 * @author xban1x
 */
public interface IParserAdvUtils : IParserUtils
{
    bool getBoolean(String key);
	
    byte getByte(String key);
	
    short getShort(String key);
	
    int getInt(String key);
	
    long getLong(String key);
	
    float getFloat(String key);
	
    double getDouble(String key);
	
    String getString(String key);
	
    TimeSpan getDuration(String key);

    T getEnum<T>(String key)
	    where T: struct, Enum;
}
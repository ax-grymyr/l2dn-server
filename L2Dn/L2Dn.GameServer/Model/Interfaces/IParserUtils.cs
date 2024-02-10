namespace L2Dn.GameServer.Model.Interfaces;

/**
 * Simple interface for parser, enforces of a fall back value.<br>
 * More suitable for developers not sure about their data.<br>
 * @author xban1x
 */
public interface IParserUtils
{
    bool getBoolean(String key, bool defaultValue);
	
    byte getByte(String key, byte defaultValue);
	
    short getShort(String key, short defaultValue);
	
    int getInt(String key, int defaultValue);
	
    long getLong(String key, long defaultValue);
	
    float getFloat(String key, float defaultValue);
	
    double getDouble(String key, double defaultValue);
	
    String getString(String key, String defaultValue);
	
    TimeSpan getDuration(String key, TimeSpan defaultValue);
	
    T getEnum<T>(String key, T defaultValue) 
	    where T: struct, Enum;
}
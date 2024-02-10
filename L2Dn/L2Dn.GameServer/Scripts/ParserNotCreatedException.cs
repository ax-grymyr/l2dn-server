namespace L2Dn.GameServer.Scripts;

public class ParserNotCreatedException: Exception
{
	public ParserNotCreatedException(): base("Parser could not be created!")
	{
	}
}
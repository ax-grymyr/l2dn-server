using System.Text;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Sql;

public class PetNameTable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(PetNameTable));
	
	public static PetNameTable getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	public bool doesPetNameExist(String name, int petNpcId)
	{
		bool result = true;
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement(
				"SELECT name FROM pets p, items i WHERE p.item_obj_id = i.object_id AND name=? AND i.item_id IN (?)");
			ps.setString(1, name);
			StringBuilder cond = new StringBuilder();
			if (!cond.ToString().isEmpty())
			{
				cond.Append(", ");
			}
			
			cond.Append(PetDataTable.getInstance().getPetItemsByNpc(petNpcId));
			ps.setString(2, cond.ToString());
			{
				ResultSet rs = ps.executeQuery();
				result = rs.next();
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Could not check existing petname:" + e);
		}
		return result;
	}
	
	public bool isValidPetName(String name)
	{
		bool result = true;
		if (!isAlphaNumeric(name))
		{
			return result;
		}
		
		Pattern pattern;
		try
		{
			pattern = Pattern.compile(Config.PET_NAME_TEMPLATE);
		}
		catch (PatternSyntaxException e) // case of illegal pattern
		{
			LOGGER.Warn(GetType().Name + ": Pet name pattern of config is wrong!");
			pattern = Pattern.compile(".*");
		}
		Matcher regexp = pattern.matcher(name);
		if (!regexp.matches())
		{
			result = false;
		}
		return result;
	}
	
	private bool isAlphaNumeric(String text)
	{
		bool result = true;
		foreach (char aChar in text)
		{
			if (!char.IsLetterOrDigit(aChar))
			{
				result = false;
				break;
			}
		}
		return result;
	}
	
	private static class SingletonHolder
	{
		public static readonly PetNameTable INSTANCE = new PetNameTable();
	}
}
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.StaticData;
using NLog;

namespace L2Dn.GameServer.Data.Sql;

public class PetNameTable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(PetNameTable));

	public static PetNameTable getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	public bool doesPetNameExist(string name, int petNpcId)
	{
		bool result = true;
		try
		{
			int itemId = PetDataTable.getInstance().getPetItemsByNpc(petNpcId);

			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			return (from pet in ctx.Pets
				from item in ctx.Items
				where pet.ItemObjectId == item.ObjectId && item.ItemId == itemId && pet.Name == name
				select pet.Name).Any();
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Could not check existing petname:" + e);
		}
		return result;
	}

	public bool isValidPetName(string name)
	{
		if (!isAlphaNumeric(name))
		{
			return false;
		}

		try
		{
			return Config.PET_NAME_TEMPLATE.IsMatch(name);
		}
		catch (Exception e) // case of illegal pattern
		{
			LOGGER.Warn(GetType().Name + ": Pet name pattern of config is wrong!: " + e);
		}

		// TODO: check length

		return true;
	}

	private static bool isAlphaNumeric(string text)
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
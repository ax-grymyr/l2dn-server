using L2Dn.GameServer.Db;
using NLog;

namespace L2Dn.GameServer.Model.Variables;

public class ItemVariables: AbstractVariables<DbItemVariable>
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ItemVariables));
	
	private readonly int _objectId;
	
	// Static Constants
	public const String VISUAL_ID = "visualId";
	public const String VISUAL_APPEARANCE_STONE_ID = "visualAppearanceStoneId";
	public const String VISUAL_APPEARANCE_LIFE_TIME = "visualAppearanceLifetime";
	public const String BLESSED = "blessed";
	
	public ItemVariables(int objectId)
	{
		_objectId = objectId;
		restoreMe();
	}
	
	public static bool hasVariables(int objectId)
	{
		// Restore previous variables.
		try 
		{
			using GameServerDbContext ctx = new();
			return ctx.ItemVariables.Any(r => r.ItemId == objectId);
		}
		catch (Exception e)
		{
			LOGGER.Error(nameof(ItemVariables) + ": Couldn't select variables count for: " + objectId + ": " + e);
			return false;
		}
	}

	protected override IQueryable<DbItemVariable> GetQuery(GameServerDbContext ctx)
	{
		return ctx.ItemVariables.Where(r => r.ItemId == _objectId);
	}

	protected override DbItemVariable CreateVar()
	{
		return new DbItemVariable()
		{
			ItemId = _objectId
		};
	}
}
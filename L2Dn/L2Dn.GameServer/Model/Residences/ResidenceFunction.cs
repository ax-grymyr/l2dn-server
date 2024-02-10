using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Residences;

/**
 * @author UnAfraid
 */
public class ResidenceFunction
{
	private readonly int _id;
	private readonly int _level;
	private long _expiration;
	private readonly AbstractResidence _residense;
	private ScheduledFuture<?> _task;

	public ResidenceFunction(int id, int level, long expiration, AbstractResidence residense)
	{
		_id = id;
		_level = level;
		_expiration = expiration;
		_residense = residense;
		init();
	}

	public ResidenceFunction(int id, int level, AbstractResidence residense)
	{
		_id = id;
		_level = level;
		ResidenceFunctionTemplate template = getTemplate();
		_expiration = Instant.now().toEpochMilli() + template.getDuration().toMillis();
		_residense = residense;
		init();
	}

	/**
	 * Initializes the function task
	 */
	private void init()
	{
		ResidenceFunctionTemplate template = getTemplate();
		if ((template != null) && (_expiration > System.currentTimeMillis()))
		{
			_task = ThreadPool.schedule(this::onFunctionExpiration, _expiration - System.currentTimeMillis());
		}
	}

	/**
	 * @return the function id
	 */
	public int getId()
	{
		return _id;
	}

	/**
	 * @return the function level
	 */
	public int getLevel()
	{
		return _level;
	}

	/**
	 * @return the expiration of this function instance
	 */
	public long getExpiration()
	{
		return _expiration;
	}

	/**
	 * @return the owner (clan) of this function instance
	 */
	public int getOwnerId()
	{
		return _residense.getOwnerId();
	}

	/**
	 * @return value of the function
	 */
	public double getValue()
	{
		ResidenceFunctionTemplate template = getTemplate();
		return template == null ? 0 : template.getValue();
	}

	/**
	 * @return the type of this function instance
	 */
	public ResidenceFunctionType getType()
	{
		ResidenceFunctionTemplate template = getTemplate();
		return template == null ? ResidenceFunctionType.NONE : template.getType();
	}

	/**
	 * @return the template of this function instance
	 */
	public ResidenceFunctionTemplate getTemplate()
	{
		return ResidenceFunctionsData.getInstance().getFunction(_id, _level);
	}

	/**
	 * The function invoked when task run, it either re-activate the function or removes it (In case clan doesn't cannot pay for it)
	 */
	private void onFunctionExpiration()
	{
		if (!reactivate())
		{
			_residense.removeFunction(this);

			Clan clan = ClanTable.getInstance().getClan(_residense.getOwnerId());
			if (clan != null)
			{
				clan.broadcastToOnlineMembers(new AgitDecoInfo(_residense));
			}
		}
	}

	/**
	 * @return {@code true} if function instance is re-activated successfully, {@code false} otherwise
	 */
	public bool reactivate()
	{
		ResidenceFunctionTemplate template = getTemplate();
		if (template == null)
		{
			return false;
		}

		Clan clan = ClanTable.getInstance().getClan(_residense.getOwnerId());
		if (clan == null)
		{
			return false;
		}

		ItemContainer wh = clan.getWarehouse();
		Item item = wh.getItemByItemId(template.getCost().getId());
		if ((item == null) || (item.getCount() < template.getCost().getCount()))
		{
			return false;
		}

		if (wh.destroyItem("FunctionFee", item, template.getCost().getCount(), null, this) != null)
		{
			_expiration = System.currentTimeMillis() + (template.getDuration().getSeconds() * 1000);
			init();
		}

		return true;
	}

	/**
	 * Cancels the task to {@link #onFunctionExpiration()}
	 */
	public void cancelExpiration()
	{
		if ((_task != null) && !_task.isDone())
		{
			_task.cancel(true);
		}

		_task = null;
	}
}
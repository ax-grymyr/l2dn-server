using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Residences;

/**
 * @author UnAfraid
 */
public class ResidenceFunction
{
    private readonly ResidenceFunctionTemplate _template;
	private readonly int _id;
	private readonly int _level;
	private DateTime _expiration;
	private readonly AbstractResidence _residense;
	private ScheduledFuture? _task;

    // TODO: pass template instead of id and level
	public ResidenceFunction(int id, int level, DateTime expiration, AbstractResidence residense)
    {
        _template = ResidenceFunctionsData.getInstance().getFunction(_id, _level) ??
            throw new ArgumentException($"Residence function template id={id}, level={level} not found");

		_id = id;
		_level = level;
		_expiration = expiration;
		_residense = residense;
		init();
	}

	public ResidenceFunction(int id, int level, AbstractResidence residense)
	{
        _template = ResidenceFunctionsData.getInstance().getFunction(_id, _level) ??
            throw new ArgumentException($"Residence function template id={id}, level={level} not found");

		_id = id;
		_level = level;
		_expiration = DateTime.UtcNow + _template.getDuration();
		_residense = residense;
		init();
	}

	/**
	 * Initializes the function task
	 */
	private void init()
	{
		if (_expiration > DateTime.UtcNow)
		{
			_task = ThreadPool.schedule(onFunctionExpiration, _expiration - DateTime.UtcNow);
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
	public DateTime getExpiration()
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
		ResidenceFunctionTemplate? template = getTemplate();
		return template == null ? 0 : template.getValue();
	}

	/**
	 * @return the type of this function instance
	 */
	public ResidenceFunctionType getType()
	{
		return _template.getType();
	}

	/**
	 * @return the template of this function instance
	 */
	public ResidenceFunctionTemplate getTemplate()
	{
		return _template;
	}

	/**
	 * The function invoked when task run, it either re-activate the function or removes it (In case clan doesn't cannot pay for it)
	 */
	private void onFunctionExpiration()
	{
		if (!reactivate())
		{
			_residense.removeFunction(this);

			Clan? clan = ClanTable.getInstance().getClan(_residense.getOwnerId());
			if (clan != null)
			{
				clan.broadcastToOnlineMembers(new AgitDecoInfoPacket(_residense));
			}
		}
	}

	/**
	 * @return {@code true} if function instance is re-activated successfully, {@code false} otherwise
	 */
	public bool reactivate()
	{
		ResidenceFunctionTemplate? template = getTemplate();
		if (template == null)
		{
			return false;
		}

		Clan? clan = ClanTable.getInstance().getClan(_residense.getOwnerId());
		if (clan == null)
		{
			return false;
		}

		ItemContainer wh = clan.getWarehouse();
		Item? item = wh.getItemByItemId(template.getCost().getId());
		if (item == null || item.getCount() < template.getCost().getCount())
		{
			return false;
		}

		if (wh.destroyItem("FunctionFee", item, template.getCost().getCount(), null, this) != null)
		{
			_expiration = DateTime.UtcNow + template.getDuration();
			init();
		}

		return true;
	}

	/**
	 * Cancels the task to {@link #onFunctionExpiration()}
	 */
	public void cancelExpiration()
	{
		if (_task != null && !_task.isDone())
		{
			_task.cancel(true);
		}

		_task = null;
	}
}
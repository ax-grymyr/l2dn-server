using L2Dn.GameServer.Model.Punishment;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author UnAfraid
 */
public class PunishmentHolder
{
	private readonly Map<string, Map<PunishmentType, PunishmentTask>> _holder = new();
	
	/**
	 * Stores the punishment task in the Map.
	 * @param task
	 */
	public void addPunishment(PunishmentTask task)
	{
		if (!task.isExpired())
		{
			_holder.computeIfAbsent(task.getKey().ToString(), k => new()).put(task.getType(), task);
		}
	}
	
	/**
	 * Removes previously stopped task from the Map.
	 * @param task
	 */
	public void stopPunishment(PunishmentTask task)
	{
		string key = task.getKey().ToString();
		if (_holder.containsKey(key))
		{
			task.stopPunishment();
			Map<PunishmentType, PunishmentTask> punishments = _holder.get(key);
			punishments.remove(task.getType());
			if (punishments.isEmpty())
			{
				_holder.remove(key);
			}
		}
	}
	
	public void stopPunishment(PunishmentType type)
	{
		foreach (Map<PunishmentType, PunishmentTask> punishments in _holder.values())
		{
			foreach (PunishmentTask task in punishments.values())
			{
				if (task.getType() == type)
				{
					task.stopPunishment();
					string key = task.getKey().ToString();
					punishments.remove(task.getType());
					if (punishments.isEmpty())
					{
						_holder.remove(key);
					}
				}
			}
		}
	}
	
	/**
	 * @param key
	 * @param type
	 * @return {@code true} if Map contains the current key and type, {@code false} otherwise.
	 */
	public bool hasPunishment(string key, PunishmentType type)
	{
		return getPunishment(key, type) != null;
	}
	
	/**
	 * @param key
	 * @param type
	 * @return {@link PunishmentTask} by specified key and type if exists, null otherwise.
	 */
	public PunishmentTask getPunishment(string key, PunishmentType type)
	{
		if (_holder.containsKey(key))
		{
			return _holder.get(key).get(type);
		}
		return null;
	}
}

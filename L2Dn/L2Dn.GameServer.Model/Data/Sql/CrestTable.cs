using System.Runtime.CompilerServices;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;
using Crest = L2Dn.GameServer.Model.Crest;

namespace L2Dn.GameServer.Data.Sql;

/**
 * Loads and saves crests from database.
 * @author NosBit
 */
public class CrestTable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(CrestTable));
	
	private readonly Map<int, Crest> _crests = new();
	private readonly AtomicInteger _nextId = new AtomicInteger(1);
	
	protected CrestTable()
	{
		load();
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void load()
	{
		_crests.Clear();
		Set<int> crestsInUse = new();
		foreach (Clan clan in ClanTable.getInstance().getClans())
		{
			if (clan.getCrestId() != null)
			{
				crestsInUse.add(clan.getCrestId().Value);
			}
			
			if (clan.getCrestLargeId() != null)
			{
				crestsInUse.add(clan.getCrestLargeId().Value);
			}
			
			if (clan.getAllyCrestId() != null)
			{
				crestsInUse.add(clan.getAllyCrestId().Value);
			}
		}
		
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var crests = ctx.Crests.OrderByDescending(c => c.Id);
			foreach (var crest in crests)
			{
				int id = crest.Id;
				if (_nextId.get() <= id)
				{
					_nextId.set(id + 1);
				}
				
				// delete all unused crests except the last one we dont want to reuse
				// a crest id because client will display wrong crest if its reused
				if (!crestsInUse.Contains(id) && (id != (_nextId.get() - 1)))
				{
					removeCrest(id);
					continue;
				}
				
				byte[] data = crest.Data;
				CrestType crestType = (CrestType)crest.Type;
				if (Enum.IsDefined(crestType))
				{
					_crests.put(id, new Crest(id, data, crestType));
				}
				else
				{
					LOGGER.Warn("Unknown crest type found in database. Type: " + crest.Type);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("There was an error while loading crests from database:", e);
		}
		
		LOGGER.Info(GetType().Name + ": Loaded " + _crests.Count + " Crests.");
		
		foreach (Clan clan in ClanTable.getInstance().getClans())
		{
			if ((clan.getCrestId() != null) && (getCrest(clan.getCrestId().Value) == null))
			{
				LOGGER.Info("Removing non-existent crest for clan " + clan.getName() + " [" + clan.getId() + "], crestId:" + clan.getCrestId());
				clan.setCrestId(0);
				clan.changeClanCrest(0);
			}
			
			if ((clan.getCrestLargeId() != null) && (getCrest(clan.getCrestLargeId().Value) == null))
			{
				LOGGER.Info("Removing non-existent large crest for clan " + clan.getName() + " [" + clan.getId() + "], crestLargeId:" + clan.getCrestLargeId());
				clan.setCrestLargeId(0);
				clan.changeLargeCrest(0);
			}
			
			if ((clan.getAllyCrestId() != null) && (getCrest(clan.getAllyCrestId().Value) == null))
			{
				LOGGER.Info("Removing non-existent ally crest for clan " + clan.getName() + " [" + clan.getId() + "], allyCrestId:" + clan.getAllyCrestId());
				clan.setAllyCrestId(0);
				clan.changeAllyCrest(0, true);
			}
		}
	}
	
	/**
	 * @param crestId The crest id
	 * @return {@code Crest} if crest is found, {@code null} if crest was not found.
	 */
	public Crest getCrest(int crestId)
	{
		return _crests.get(crestId);
	}
	
	/**
	 * Creates a {@code Crest} object and inserts it in database and cache.
	 * @param data
	 * @param crestType
	 * @return {@code Crest} on success, {@code null} on failure.
	 */
	public Crest createCrest(byte[] data, CrestType crestType)
	{
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			Crest crest = new Crest(_nextId.getAndIncrement(), data, crestType);
			ctx.Crests.Add(new()
			{
				Id = crest.getId(),
				Data = crest.getData(),
				Type = (byte)crest.getType()
			});

			ctx.SaveChanges();
			_crests.put(crest.getId(), crest);
			return crest;
		}
		catch (Exception e)
		{
			LOGGER.Warn("There was an error while saving crest in database:", e);
		}
		return null;
	}
	
	/**
	 * Removes crest from database and cache.
	 * @param crestId the id of crest to be removed.
	 */
	public void removeCrest(int crestId)
	{
		_crests.remove(crestId);
		
		// avoid removing last crest id we dont want to lose index...
		// because client will display wrong crest if its reused
		if (crestId == (_nextId.get() - 1))
		{
			return;
		}
		
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.Crests.Where(c => c.Id == crestId).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Warn("There was an error while deleting crest from database:", e);
		}
	}
	
	/**
	 * @return The next crest id.
	 */
	public int getNextId()
	{
		return _nextId.getAndIncrement();
	}
	
	public static CrestTable getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly CrestTable INSTANCE = new CrestTable();
	}
}
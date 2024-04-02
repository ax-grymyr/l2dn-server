using L2Dn.GameServer.Db;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.Model;

public class Couple
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(Couple));
	
	private int _id = 0;
	private int _player1Id = 0;
	private int _player2Id = 0;
	private bool _maried = false;
	private DateTime _affiancedDate;
	private DateTime? _weddingDate;
	
	public Couple(int coupleId)
	{
		_id = coupleId;

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var record = ctx.CharacterCouples.SingleOrDefault(r => r.Id == _id);
			if (record != null)
			{
				_player1Id = record.Character1Id;
				_player2Id = record.Character2Id;
				_maried = record.Married;
				_affiancedDate = record.AffianceDate;
				_weddingDate = record.WeddingDate;
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Exception: Couple.load(): " + e);
		}
	}
	
	public Couple(Player player1, Player player2)
	{
		_player1Id = player1.getObjectId();
		_player2Id = player2.getObjectId();
		
		_affiancedDate = DateTime.UtcNow;
		_weddingDate = null;
		
		try 
		{
			_id = IdManager.getInstance().getNextId();

			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.CharacterCouples.Add(new DbCharacterCouple()
			{
				Id = _id,
				Character1Id = _player1Id,
				Character2Id = _player2Id,
				Married = false,
				AffianceDate = _affiancedDate,
				WeddingDate = _weddingDate
			});

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not create couple: " + e);
		}
	}
	
	public void marry()
	{
		try 
		{
			_weddingDate = DateTime.UtcNow;
			_maried = true;

			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.CharacterCouples.Where(r => r.Id == _id).ExecuteUpdate(s =>
				s.SetProperty(r => r.Married, true).SetProperty(r => r.WeddingDate, _weddingDate));
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not marry: " + e);
		}
	}
	
	public void divorce()
	{
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.CharacterCouples.Where(r => r.Id == _id).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Exception: Couple.divorce(): " + e);
		}
	}
	
	public int getId()
	{
		return _id;
	}
	
	public int getPlayer1Id()
	{
		return _player1Id;
	}
	
	public int getPlayer2Id()
	{
		return _player2Id;
	}
	
	public bool getMaried()
	{
		return _maried;
	}
	
	public DateTime getAffiancedDate()
	{
		return _affiancedDate;
	}
	
	public DateTime? getWeddingDate()
	{
		return _weddingDate;
	}
}

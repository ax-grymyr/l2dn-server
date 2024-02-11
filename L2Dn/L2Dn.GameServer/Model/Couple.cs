using L2Dn.GameServer.Model.Actor;
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
	private DateTime _weddingDate;
	
	public Couple(int coupleId)
	{
		_id = coupleId;
		
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement("SELECT * FROM mods_wedding WHERE id = ?");
			ps.setInt(1, _id);
			ResultSet rs = ps.executeQuery();
				while (rs.next())
				{
					_player1Id = rs.getInt("player1Id");
					_player2Id = rs.getInt("player2Id");
					_maried = rs.getBoolean("married");
					_affiancedDate = Calendar.getInstance();
					_affiancedDate.setTimeInMillis(rs.getLong("affianceDate"));
					
					_weddingDate = Calendar.getInstance();
					_weddingDate.setTimeInMillis(rs.getLong("weddingDate"));
				}
		}
		catch (Exception e)
		{
			LOGGER.Error("Exception: Couple.load(): " + e);
		}
	}
	
	public Couple(Player player1, Player player2)
	{
		long currentTime = System.currentTimeMillis();
		_player1Id = player1.getObjectId();
		_player2Id = player2.getObjectId();
		
		_affiancedDate = Calendar.getInstance();
		_affiancedDate.setTimeInMillis(currentTime);
		
		_weddingDate = Calendar.getInstance();
		_weddingDate.setTimeInMillis(currentTime);
		
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement(
				"INSERT INTO mods_wedding (id, player1Id, player2Id, married, affianceDate, weddingDate) VALUES (?, ?, ?, ?, ?, ?)");
			_id = IdManager.getInstance().getNextId();
			ps.setInt(1, _id);
			ps.setInt(2, _player1Id);
			ps.setInt(3, _player2Id);
			ps.setBoolean(4, false);
			ps.setLong(5, _affiancedDate.getTimeInMillis());
			ps.setLong(6, _weddingDate.getTimeInMillis());
			ps.execute();
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
			using GameServerDbContext ctx = new();
			PreparedStatement ps =
				con.prepareStatement("UPDATE mods_wedding set married = ?, weddingDate = ? where id = ?");
			ps.setBoolean(1, true);
			_weddingDate = Calendar.getInstance();
			ps.setLong(2, _weddingDate.getTimeInMillis());
			ps.setInt(3, _id);
			ps.execute();
			_maried = true;
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
			using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement("DELETE FROM mods_wedding WHERE id=?");
			ps.setInt(1, _id);
			ps.execute();
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
	
	public DateTime getWeddingDate()
	{
		return _weddingDate;
	}
}

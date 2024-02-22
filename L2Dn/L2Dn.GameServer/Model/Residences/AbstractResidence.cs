using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Residences;

/**
 * @author xban1x
 */
public abstract class AbstractResidence: ListenersContainer, INamable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AbstractResidence));
	
	protected ClanHallGrade _grade = ClanHallGrade.GRADE_NONE;
	
	private readonly int _residenceId;
	private String _name;
	private ResidenceZone _zone = null;
	private readonly Map<int, ResidenceFunction> _functions = new();
	private List<SkillLearn> _residentialSkills = new();
	
	public AbstractResidence(int residenceId)
	{
		_residenceId = residenceId;
		initResidentialSkills();
	}
	
	protected abstract void load();
	
	protected abstract void initResidenceZone();
	
	public abstract int getOwnerId();
	
	protected void initResidentialSkills()
	{
		_residentialSkills = SkillTreeData.getInstance().getAvailableResidentialSkills(getResidenceId());
	}
	
	/**
	 * Gets the grade of clan hall.
	 * @return grade of this {@link ClanHall} in {@link ClanHallGrade} enum.
	 */
	public ClanHallGrade getGrade()
	{
		return _grade;
	}
	
	public int getResidenceId()
	{
		return _residenceId;
	}
	
	public String getName()
	{
		return _name;
	}
	
	// TODO: Remove it later when both castles and forts are loaded from same table.
	public void setName(String name)
	{
		_name = name;
	}
	
	public ResidenceZone getResidenceZone()
	{
		return _zone;
	}
	
	protected void setResidenceZone(ResidenceZone zone)
	{
		_zone = zone;
	}
	
	public void giveResidentialSkills(Player player)
	{
		if ((_residentialSkills != null) && !_residentialSkills.isEmpty())
		{
			SocialClass playerSocialClass = player.getPledgeClass() + 1;
			foreach (SkillLearn skill  in  _residentialSkills)
			{
				SocialClass skillSocialClass = skill.getSocialClass();
				if ((skillSocialClass == null) || (playerSocialClass >= skillSocialClass))
				{
					player.addSkill(SkillData.getInstance().getSkill(skill.getSkillId(), skill.getSkillLevel()), false);
				}
			}
		}
	}
	
	public void removeResidentialSkills(Player player)
	{
		if ((_residentialSkills != null) && !_residentialSkills.isEmpty())
		{
			foreach (SkillLearn skill  in  _residentialSkills)
			{
				player.removeSkill(skill.getSkillId(), false);
			}
		}
	}
	
	/**
	 * Initializes all available functions for the current residence
	 */
	protected void initFunctions()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement("SELECT * FROM residence_functions WHERE residenceId = ?");
			ps.setInt(1, _residenceId);
			{
				ResultSet rs = ps.executeQuery();
				while (rs.next())
				{
					int id = rs.getInt("id");
					int level = rs.getInt("level");
					long expiration = rs.getLong("expiration");
					ResidenceFunction func = new ResidenceFunction(id, level, expiration, this);
					if ((expiration <= System.currentTimeMillis()) && !func.reactivate())
					{
						removeFunction(func);
						continue;
					}
					_functions.put(id, func);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Failed to initialize functions for residence: " + _residenceId + ": " + e);
		}
	}
	
	public void addFunction(int id, int level)
	{
		addFunction(new ResidenceFunction(id, level, this));
	}
	
	/**
	 * Adds new function and removes old if matches same id
	 * @param func
	 */
	public void addFunction(ResidenceFunction func)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement(
				"INSERT INTO residence_functions (id, level, expiration, residenceId) VALUES (?, ?, ?, ?) ON DUPLICATE KEY UPDATE level = ?, expiration = ?");
			ps.setInt(1, func.getId());
			ps.setInt(2, func.getLevel());
			ps.setLong(3, func.getExpiration());
			ps.setInt(4, _residenceId);
			ps.setInt(5, func.getLevel());
			ps.setLong(6, func.getExpiration());
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Failed to add function: " + func.getId() + " for residence: " + _residenceId + ": " + e);
		}
		finally
		{
			if (_functions.containsKey(func.getId()))
			{
				removeFunction(_functions.get(func.getId()));
			}
			_functions.put(func.getId(), func);
		}
	}
	
	/**
	 * Removes the specified function
	 * @param func
	 */
	public void removeFunction(ResidenceFunction func)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps =
				con.prepareStatement("DELETE FROM residence_functions WHERE residenceId = ? and id = ?");
			ps.setInt(1, _residenceId);
			ps.setInt(2, func.getId());
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Failed to remove function: " + func.getId() + " residence: " + _residenceId + ": " + e);
		}
		finally
		{
			func.cancelExpiration();
			_functions.remove(func.getId());
		}
	}
	
	/**
	 * Removes all functions
	 */
	public void removeFunctions()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement("DELETE FROM residence_functions WHERE residenceId = ?");
			ps.setInt(1, _residenceId);
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Failed to remove functions for residence: " + _residenceId + ": " + e);
		}
		finally
		{
			_functions.values().forEach(x => x.cancelExpiration());
			_functions.clear();
		}
	}
	
	/**
	 * @param type
	 * @return {@code true} if function is available, {@code false} otherwise
	 */
	public bool hasFunction(ResidenceFunctionType type)
	{
		foreach (ResidenceFunction function  in  _functions.values())
		{
			ResidenceFunctionTemplate template = function.getTemplate();
			if ((template != null) && (template.getType() == type))
			{
				return true;
			}
		}
		return false;
	}
	
	/**
	 * @param type
	 * @return the function template by type, null if not available
	 */
	public ResidenceFunction getFunction(ResidenceFunctionType type)
	{
		foreach (ResidenceFunction function  in  _functions.values())
		{
			if (function.getType() == type)
			{
				return function;
			}
		}
		return null;
	}
	
	/**
	 * @param id
	 * @param level
	 * @return the function by id and level, null if not available
	 */
	public ResidenceFunction getFunction(int id, int level)
	{
		foreach (ResidenceFunction func  in  _functions.values())
		{
			if ((func.getId() == id) && (func.getLevel() == level))
			{
				return func;
			}
		}
		return null;
	}
	
	/**
	 * @param id
	 * @return the function by id, null if not available
	 */
	public ResidenceFunction getFunction(int id)
	{
		foreach (ResidenceFunction func  in  _functions.values())
		{
			if (func.getId() == id)
			{
				return func;
			}
		}
		return null;
	}
	
	/**
	 * @param type
	 * @return level of function, 0 if not available
	 */
	public int getFunctionLevel(ResidenceFunctionType type)
	{
		ResidenceFunction func = getFunction(type);
		return func != null ? func.getLevel() : 0;
	}
	
	/**
	 * @param type
	 * @return the expiration of function by type, -1 if not available
	 */
	public long getFunctionExpiration(ResidenceFunctionType type)
	{
		ResidenceFunction function = null;
		foreach (ResidenceFunction func  in  _functions.values())
		{
			if (func.getTemplate().getType() == type)
			{
				function = func;
				break;
			}
		}
		return function != null ? function.getExpiration() : -1;
	}
	
	/**
	 * @return all avaible functions
	 */
	public ICollection<ResidenceFunction> getFunctions()
	{
		return _functions.values();
	}
	
	public override bool Equals(Object? obj)
	{
		return (obj is AbstractResidence) && (((AbstractResidence) obj).getResidenceId() == getResidenceId());
	}
	
	public override String ToString()
	{
		return _name + " (" + _residenceId + ")";
	}
}
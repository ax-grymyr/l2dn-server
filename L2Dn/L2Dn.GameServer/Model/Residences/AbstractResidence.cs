using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
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
			var query = ctx.ResidenceFunctions.Where(r => r.ResidenceId == _residenceId);
			foreach (var record in query)
			{
				int id = record.Id;
				int level = record.Level;
				DateTime expiration = record.Expiration;
				ResidenceFunction func = new ResidenceFunction(id, level, expiration, this);
				if ((expiration <= DateTime.UtcNow) && !func.reactivate())
				{
					removeFunction(func);
					continue;
				}

				_functions.put(id, func);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Failed to initialize functions for residence: " + _residenceId + ": " + e);
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
			int funcId = func.getId();
			var record = ctx.ResidenceFunctions.SingleOrDefault(r =>
				r.ResidenceId == _residenceId && r.Id == funcId);

			if (record == null)
			{
				record = new DbResidenceFunction();
				record.ResidenceId = _residenceId;
				record.Id = funcId;
				ctx.ResidenceFunctions.Add(record);
			}

			record.Level = func.getLevel();
			record.Expiration = func.getExpiration();
			ctx.SaveChanges();
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
			int funcId = func.getId();
			ctx.ResidenceFunctions.Where(r => r.ResidenceId == _residenceId && r.Id == funcId).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Failed to remove function: " + func.getId() + " residence: " + _residenceId + ": " + e);
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
			ctx.ResidenceFunctions.Where(r => r.ResidenceId == _residenceId).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Failed to remove functions for residence: " + _residenceId + ": " + e);
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
	public DateTime? getFunctionExpiration(ResidenceFunctionType type)
	{
		ResidenceFunction function = null;
		foreach (ResidenceFunction func in _functions.values())
		{
			if (func.getTemplate().getType() == type)
			{
				function = func;
				break;
			}
		}

		return function?.getExpiration();
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
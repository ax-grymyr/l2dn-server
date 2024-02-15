using L2Dn.GameServer.Model.Cubics;
using L2Dn.GameServer.Model.Cubics.Conditions;

namespace L2Dn.GameServer.Model.Actor.Templates;

public class CubicTemplate : CreatureTemplate , ICubicConditionHolder
{
	private readonly int _id;
	private readonly int _level;
	private readonly int _slot;
	private readonly int _duration;
	private readonly int _delay;
	private readonly int _maxCount;
	private readonly int _useUp;
	private readonly double _power;
	private readonly CubicTargetType _targetType;
	private readonly List<ICubicCondition> _conditions = new();
	private readonly List<CubicSkill> _skills = new();
	
	public CubicTemplate(StatSet set): base(set)
	{
		_id = set.getInt("id");
		_level = set.getInt("level");
		_slot = set.getInt("slot");
		_duration = set.getInt("duration");
		_delay = set.getInt("delay");
		_maxCount = set.getInt("maxCount");
		_useUp = set.getInt("useUp");
		_power = set.getDouble("power");
		_targetType = set.getEnum("targetType", CubicTargetType.TARGET);
	}
	
	public int getId()
	{
		return _id;
	}
	
	public int getLevel()
	{
		return _level;
	}
	
	public int getSlot()
	{
		return _slot;
	}
	
	public int getDuration()
	{
		return _duration;
	}
	
	public int getDelay()
	{
		return _delay;
	}
	
	public int getMaxCount()
	{
		return _maxCount;
	}
	
	public int getUseUp()
	{
		return _useUp;
	}
	
	public CubicTargetType getTargetType()
	{
		return _targetType;
	}
	
	public List<CubicSkill> getCubicSkills()
	{
		return _skills;
	}
	
	public override int getBasePAtk()
	{
		return (int) _power;
	}
	
	public override int getBaseMAtk()
	{
		return (int) _power;
	}
	
	public bool validateConditions(Cubic cubic, Creature owner, WorldObject target)
	{
		if (_conditions.Count == 0)
		{
			return true;
		}
		
		foreach (ICubicCondition condition in _conditions)
		{
			if (!condition.test(cubic, owner, target))
			{
				return false;
			}
		}
		
		return true;
	}
	
	public void addCondition(ICubicCondition condition)
	{
		_conditions.Add(condition);
	}

	public override String ToString()
	{
		return "Cubic id: " + _id + " level: " + _level + " slot: " + _slot + " duration: " + _duration + " delay: " +
		       _delay + " maxCount: " + _maxCount + " useUp: " + _useUp + " power: " + _power + Environment.NewLine +
		       "skills: " + _skills + Environment.NewLine + "conditions:" + _conditions + Environment.NewLine;
	}
}
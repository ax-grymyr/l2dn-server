using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Data.Sql;

/**
 * @author Nyaran
 */
public class SummonEffectTable
{
	/** Servitors **/
	// Map tree
	// => key: charObjectId, value: classIndex Map
	// --> key: classIndex, value: servitors Map
	// ---> key: servitorSkillId, value: Effects list
	private readonly Map<int, Map<int, Map<int, ICollection<SummonEffect>>>> _servitorEffects = new();
	
	public Map<int, Map<int, Map<int, ICollection<SummonEffect>>>> getServitorEffectsOwner()
	{
		return _servitorEffects;
	}
	
	public Map<int, ICollection<SummonEffect>> getServitorEffects(Player owner)
	{
		Map<int, Map<int, ICollection<SummonEffect>>> servitorMap = _servitorEffects.get(owner.getObjectId());
		if (servitorMap == null)
		{
			return null;
		}
		return servitorMap.get(owner.getClassIndex());
	}
	
	/** Pets **/
	private readonly Map<int, ICollection<SummonEffect>> _petEffects = new(); // key: petItemObjectId, value: Effects list
	
	public Map<int, ICollection<SummonEffect>> getPetEffects()
	{
		return _petEffects;
	}
	
	public class SummonEffect
	{
		private readonly Skill _skill;
		private readonly int _effectCurTime;
		
		public SummonEffect(Skill skill, int effectCurTime)
		{
			_skill = skill;
			_effectCurTime = effectCurTime;
		}
		
		public Skill getSkill()
		{
			return _skill;
		}
		
		public int getEffectCurTime()
		{
			return _effectCurTime;
		}
	}
	
	public static SummonEffectTable getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly SummonEffectTable INSTANCE = new SummonEffectTable();
	}
}
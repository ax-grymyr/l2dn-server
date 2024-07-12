using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * @author NosBit
 */
public class SkillConditionHandler
{
	private readonly Map<string, Func<StatSet, ISkillCondition>> _skillConditionHandlerFactories = new();

	private SkillConditionHandler()
	{
	}
	
	public void registerHandler(string name, Func<StatSet, ISkillCondition> handlerFactory)
	{
		_skillConditionHandlerFactories.put(name, handlerFactory);
	}
	
	public Func<StatSet, ISkillCondition> getHandlerFactory(string name)
	{
		return _skillConditionHandlerFactories.get(name);
	}
	
	public int size()
	{
		return _skillConditionHandlerFactories.Count;
	}
	
	private static class SingletonHolder
	{
		public static readonly SkillConditionHandler INSTANCE = new SkillConditionHandler();
	}
	
	public static SkillConditionHandler getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
}
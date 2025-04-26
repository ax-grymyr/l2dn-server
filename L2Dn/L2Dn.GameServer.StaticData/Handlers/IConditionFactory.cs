namespace L2Dn.GameServer.Handlers;

public interface IConditionFactory<in T>
{
    IConditionBase Create(T source);
}
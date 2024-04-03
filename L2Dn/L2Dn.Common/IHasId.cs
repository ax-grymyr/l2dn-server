namespace L2Dn;

public interface IHasId<out TId>
{
    TId Id { get; }
}
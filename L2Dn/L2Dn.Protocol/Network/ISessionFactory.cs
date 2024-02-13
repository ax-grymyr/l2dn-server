namespace L2Dn.Network;

public interface ISessionFactory<out TSession>
{
    TSession Create();
}
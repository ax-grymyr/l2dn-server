namespace L2Dn.GameServer.Model.Interfaces;

public interface IParameterized<T>
{
    T getParameters();
	
    void setParameters(T set);
}
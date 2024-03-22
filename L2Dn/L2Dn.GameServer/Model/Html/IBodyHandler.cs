using System.Text;

namespace L2Dn.GameServer.Model.Html;

public interface IBodyHandler<in T>
{
    void apply(int pages, T type, StringBuilder sb);
}
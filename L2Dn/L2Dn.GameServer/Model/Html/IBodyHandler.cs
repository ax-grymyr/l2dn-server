using System.Text;

namespace L2Dn.GameServer.Model.Html;

public interface IBodyHandler<in T>
{
    void apply(int pages, T type, StringBuilder sb);

    public void create(IEnumerable<T> elements, int pages, int start, int elementsPerPage, StringBuilder sb)
    {
        int i = 0;
        foreach (T element in elements)
        {
            if (i++ < start)
            {
                continue;
            }

            apply(pages, element, sb);

            if (i >= (elementsPerPage + start))
            {
                break;
            }
        }
    }
}
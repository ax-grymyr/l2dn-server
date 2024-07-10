using System.Numerics;

namespace L2Dn.Extensions;

public static class ListExtensions
{
    public static T? FindById<TId, T>(this List<T> list, TId id)
        where TId: IEqualityOperators<TId, TId, bool>
        where T: IHasId<TId>
    {
        foreach (T item in list)
        {
            if (id == item.Id)
                return item;
        }

        return default;
    }
}
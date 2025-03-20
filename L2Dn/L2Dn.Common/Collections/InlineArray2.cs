using System.Runtime.CompilerServices;

namespace L2Dn.Collections;

[InlineArray(2)]
public struct InlineArray2<T>: IInlineArray<T>
{
    public T Values;
    public int Length => 2;
    public T GetItem(int index) => this[index];
    public void SetItem(int index, T value) => this[index] = value;
}
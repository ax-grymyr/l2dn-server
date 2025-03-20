using System.Runtime.CompilerServices;

namespace L2Dn.Collections;

[InlineArray(7)]
public struct InlineArray7<T>: IInlineArray<T>
{
    public T Values;
    public int Length => 7;
    public T GetItem(int index) => this[index];
    public void SetItem(int index, T value) => this[index] = value;
}
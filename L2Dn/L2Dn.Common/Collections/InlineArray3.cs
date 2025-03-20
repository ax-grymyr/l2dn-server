using System.Runtime.CompilerServices;

namespace L2Dn.Collections;

[InlineArray(3)]
public struct InlineArray3<T>: IInlineArray<T>
{
    public T Values;
    public int Length => 3;
    public T GetItem(int index) => this[index];
    public void SetItem(int index, T value) => this[index] = value;
}
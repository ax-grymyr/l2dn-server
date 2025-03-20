using System.Runtime.CompilerServices;

namespace L2Dn.Collections;

[InlineArray(4)]
public struct InlineArray4<T>: IInlineArray<T>
{
    public T Values;
    public int Length => 4;
    public T GetItem(int index) => this[index];
    public void SetItem(int index, T value) => this[index] = value;
}
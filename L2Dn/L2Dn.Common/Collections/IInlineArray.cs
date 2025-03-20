namespace L2Dn.Collections;

public interface IInlineArray
{
    int Length { get; }
}

public interface IInlineArray<T>: IInlineArray
{
    T GetItem(int index);
    void SetItem(int index, T value);
}
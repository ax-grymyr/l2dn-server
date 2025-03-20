namespace L2Dn.Collections;

public interface IFixedBitSet
{
    static abstract int Capacity { get; }
    int Count { get; }
    bool this[int index] { get; set; }
    void SetBit(int index);
    void ClearBit(int index);
    void Clear();
}
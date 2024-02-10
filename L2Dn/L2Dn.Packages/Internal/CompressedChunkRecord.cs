namespace L2Dn.Packages.Internal;

internal struct CompressedChunkRecord: ISerializableObject
{
    /// <summary>
    /// The start offset of this chunk in the completely uncompressed package.
    /// </summary>
    public uint UncompressedOffset;

    /// <summary>
    /// The uncompressed size of this chunk.
    /// </summary>
    public uint UncompressedChunk;

    /// <summary>
    /// The start offset of this chunk in the compressed package file.
    /// </summary>
    public uint CompressedOffset;

    /// <summary>
    /// The compressed size of this chunk.
    /// </summary>
    public uint CompressedChunk;

    public void Read(UBinaryReader reader)
    {
        UncompressedOffset = reader.ReadUInt32();
        UncompressedChunk = reader.ReadUInt32();
        CompressedOffset = reader.ReadUInt32();
        CompressedChunk = reader.ReadUInt32();
    }
}

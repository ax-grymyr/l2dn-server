using L2Dn.GameServer.Model.Holders;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Collections;

public readonly struct ExCollectionRegisterPacket: IOutgoingPacket
{
    private readonly bool _success;
    private readonly int _collectionId;
    private readonly int _index;
    private readonly ItemEnchantHolder _collectionInfo;

    public ExCollectionRegisterPacket(bool success, int collectionId, int index, ItemEnchantHolder collectionInfo)
    {
        _success = success;
        _collectionId = collectionId;
        _index = index;
        _collectionInfo = collectionInfo;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_COLLECTION_REGISTER);

        writer.WriteInt16((short)_collectionId);
        writer.WriteByte(_success); // success
        writer.WriteByte(0); // recursive reward
        writer.WriteInt16(249); // 256 - size so far
        writer.WriteByte((byte)_index); // slot index
        writer.WriteInt32(_collectionInfo.Id); // item classId
        writer.WriteInt16((short)_collectionInfo.EnchantLevel); // enchant level
        writer.WriteByte(0); // is blessed
        writer.WriteByte(0); // blessed conditions
        writer.WriteInt32((int)_collectionInfo.Count); // amount
    }
}
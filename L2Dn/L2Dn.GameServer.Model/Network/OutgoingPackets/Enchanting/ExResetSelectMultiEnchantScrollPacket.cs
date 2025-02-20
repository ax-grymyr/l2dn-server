using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Enchanting;

public readonly struct ExResetSelectMultiEnchantScrollPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly int _scrollObjectId;
    private readonly int _resultType;

    public ExResetSelectMultiEnchantScrollPacket(Player player, int scrollObjectId, int resultType)
    {
        _player = player;
        _scrollObjectId = scrollObjectId;
        _resultType = resultType;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        EnchantItemRequest? request = _player.getRequest<EnchantItemRequest>();
        if (request == null)
        {
            return;
        }

        if (request.getEnchantingScroll() == null)
        {
            request.setEnchantingScroll(_scrollObjectId);
        }

        writer.WritePacketCode(OutgoingPacketCodes.EX_RES_SELECT_MULTI_ENCHANT_SCROLL);

        writer.WriteInt32(_scrollObjectId);
        writer.WriteInt32(_resultType);
    }
}
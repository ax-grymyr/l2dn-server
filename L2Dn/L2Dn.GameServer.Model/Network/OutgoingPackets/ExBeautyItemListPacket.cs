using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExBeautyItemListPacket: IOutgoingPacket
{
    private const int HAIR_TYPE = 0;
    private const int FACE_TYPE = 1;
    private const int COLOR_TYPE = 2;

    private readonly int _colorCount;
    private readonly BeautyData _beautyData;
    private readonly Map<int, List<BeautyItem>> _colorData;

    public ExBeautyItemListPacket(Player player)
    {
        _colorData = new Map<int, List<BeautyItem>>();
        _beautyData = BeautyShopData.Instance.GetBeautyData(player.getRace(), player.getAppearance().getSex()) ??
            throw new InvalidOperationException(
                $"No beauty data for race {player.getRace()} and sex {player.getAppearance().getSex()}"); // TODO: null checking hack

        foreach (BeautyItem hair in _beautyData.HairList.Values)
        {
            List<BeautyItem> colors = new();
            foreach (BeautyItem color in hair.Colors.Values)
            {
                colors.Add(color);
                _colorCount++;
            }

            _colorData[hair.Id] = colors;
        }
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BEAUTY_ITEM_LIST);

        writer.WriteInt32(HAIR_TYPE);
        writer.WriteInt32(_beautyData.HairList.Count);
        foreach (BeautyItem hair in _beautyData.HairList.Values)
        {
            writer.WriteInt32(0); // ?
            writer.WriteInt32(hair.Id);
            writer.WriteInt32(hair.Adena);
            writer.WriteInt32(hair.ResetAdena);
            writer.WriteInt32(hair.BeautyShopTicket);
            writer.WriteInt32(1); // Limit
        }

        writer.WriteInt32(FACE_TYPE);
        writer.WriteInt32(_beautyData.FaceList.Count);
        foreach (BeautyItem face in _beautyData.FaceList.Values)
        {
            writer.WriteInt32(0); // ?
            writer.WriteInt32(face.Id);
            writer.WriteInt32(face.Adena);
            writer.WriteInt32(face.ResetAdena);
            writer.WriteInt32(face.BeautyShopTicket);
            writer.WriteInt32(1); // Limit
        }

        writer.WriteInt32(COLOR_TYPE);
        writer.WriteInt32(_colorCount);
        foreach (var entry in _colorData)
        {
            foreach (BeautyItem color in entry.Value)
            {
                writer.WriteInt32(entry.Key);
                writer.WriteInt32(color.Id);
                writer.WriteInt32(color.Adena);
                writer.WriteInt32(color.ResetAdena);
                writer.WriteInt32(color.BeautyShopTicket);
                writer.WriteInt32(1);
            }
        }
    }
}
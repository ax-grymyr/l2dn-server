using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.BeautyShop;
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
        _beautyData = BeautyShopData.getInstance().getBeautyData(player.getRace(), player.getAppearance().getSex());
        foreach (BeautyItem hair in _beautyData.getHairList().values())
        {
            List<BeautyItem> colors = new();
            foreach (BeautyItem color in hair.getColors().values())
            {
                colors.Add(color);
                _colorCount++;
            }
            
            _colorData.put(hair.getId(), colors);
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BEAUTY_ITEM_LIST);
        
        writer.WriteInt32(HAIR_TYPE);
        writer.WriteInt32(_beautyData.getHairList().size());
        foreach (BeautyItem hair in _beautyData.getHairList().values())
        {
            writer.WriteInt32(0); // ?
            writer.WriteInt32(hair.getId());
            writer.WriteInt32(hair.getAdena());
            writer.WriteInt32(hair.getResetAdena());
            writer.WriteInt32(hair.getBeautyShopTicket());
            writer.WriteInt32(1); // Limit
        }
        writer.WriteInt32(FACE_TYPE);
        writer.WriteInt32(_beautyData.getFaceList().size());
        foreach (BeautyItem face in _beautyData.getFaceList().values())
        {
            writer.WriteInt32(0); // ?
            writer.WriteInt32(face.getId());
            writer.WriteInt32(face.getAdena());
            writer.WriteInt32(face.getResetAdena());
            writer.WriteInt32(face.getBeautyShopTicket());
            writer.WriteInt32(1); // Limit
        }
        writer.WriteInt32(COLOR_TYPE);
        writer.WriteInt32(_colorCount);
        foreach (var entry in _colorData)
        {
            foreach (BeautyItem color in entry.Value)
            {
                writer.WriteInt32(entry.Key);
                writer.WriteInt32(color.getId());
                writer.WriteInt32(color.getAdena());
                writer.WriteInt32(color.getResetAdena());
                writer.WriteInt32(color.getBeautyShopTicket());
                writer.WriteInt32(1);
            }
        }
    }
}
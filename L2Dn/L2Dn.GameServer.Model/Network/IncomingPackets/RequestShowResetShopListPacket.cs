using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.BeautyShop;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestShowResetShopListPacket: IIncomingPacket<GameSession>
{
    private int _hairId;
    private int _faceId;
    private int _colorId;

    public void ReadContent(PacketBitReader reader)
    {
        _hairId = reader.ReadInt32();
        _faceId = reader.ReadInt32();
        _colorId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        BeautyData beautyData = BeautyShopData.getInstance().getBeautyData(player.getRace(), player.getAppearance().getSex());
        int requiredAdena = 0;
        if (_hairId > 0)
        {
            BeautyItem hair = beautyData.getHairList().get(_hairId);
            if (hair == null)
            {
                player.sendPacket(new ExResponseBeautyRegistResetPacket(player,
                    ExResponseBeautyRegistResetPacket.RESTORE, ExResponseBeautyRegistResetPacket.FAILURE));
                
                return ValueTask.CompletedTask;
            }
			
            requiredAdena += hair.getResetAdena();
            if (_colorId > 0)
            {
                BeautyItem color = hair.getColors().get(_colorId);
                if (color == null)
                {
                    player.sendPacket(new ExResponseBeautyRegistResetPacket(player,
                        ExResponseBeautyRegistResetPacket.RESTORE, ExResponseBeautyRegistResetPacket.FAILURE));
                    
                    return ValueTask.CompletedTask;
                }
				
                requiredAdena += color.getResetAdena();
            }
        }
		
        if (_faceId > 0)
        {
            BeautyItem face = beautyData.getFaceList().get(_faceId);
            if (face == null)
            {
                player.sendPacket(new ExResponseBeautyRegistResetPacket(player,
                    ExResponseBeautyRegistResetPacket.RESTORE, ExResponseBeautyRegistResetPacket.FAILURE));
                
                return ValueTask.CompletedTask;
            }
			
            requiredAdena += face.getResetAdena();
        }
		
        if ((player.getAdena() < requiredAdena))
        {
            player.sendPacket(new ExResponseBeautyRegistResetPacket(player, ExResponseBeautyRegistResetPacket.RESTORE,
                ExResponseBeautyRegistResetPacket.FAILURE));
            
            return ValueTask.CompletedTask;
        }
		
        if ((requiredAdena > 0) && !player.reduceAdena(GetType().Name, requiredAdena, null, true))
        {
            player.sendPacket(new ExResponseBeautyRegistResetPacket(player, ExResponseBeautyRegistResetPacket.RESTORE,
                ExResponseBeautyRegistResetPacket.FAILURE));
            
            return ValueTask.CompletedTask;
        }
		
        player.getVariables().remove("visualHairId");
        player.getVariables().remove("visualHairColorId");
        player.getVariables().remove("visualFaceId");

        player.sendPacket(new ExResponseBeautyRegistResetPacket(player, ExResponseBeautyRegistResetPacket.RESTORE,
            ExResponseBeautyRegistResetPacket.SUCCESS));
        
        return ValueTask.CompletedTask;
    }
}
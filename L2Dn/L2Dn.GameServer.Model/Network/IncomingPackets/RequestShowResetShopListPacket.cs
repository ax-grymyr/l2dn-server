using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
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

        BeautyData? beautyData = BeautyShopData.Instance
            .GetBeautyData(player.getRace(), player.getAppearance().getSex());

        int requiredAdena = 0;
        if (_hairId > 0)
        {
            BeautyItem? hair = beautyData?.HairList.GetValueOrDefault(_hairId);
            if (hair == null)
            {
                player.sendPacket(new ExResponseBeautyRegistResetPacket(player,
                    ExResponseBeautyRegistResetPacket.RESTORE, ExResponseBeautyRegistResetPacket.FAILURE));

                return ValueTask.CompletedTask;
            }

            requiredAdena += hair.ResetAdena;
            if (_colorId > 0)
            {
                BeautyItem? color = hair.Colors.GetValueOrDefault(_colorId);
                if (color == null)
                {
                    player.sendPacket(new ExResponseBeautyRegistResetPacket(player,
                        ExResponseBeautyRegistResetPacket.RESTORE, ExResponseBeautyRegistResetPacket.FAILURE));

                    return ValueTask.CompletedTask;
                }

                requiredAdena += color.ResetAdena;
            }
        }

        if (_faceId > 0)
        {
            BeautyItem? face = beautyData?.FaceList.GetValueOrDefault(_faceId);
            if (face == null)
            {
                player.sendPacket(new ExResponseBeautyRegistResetPacket(player,
                    ExResponseBeautyRegistResetPacket.RESTORE, ExResponseBeautyRegistResetPacket.FAILURE));

                return ValueTask.CompletedTask;
            }

            requiredAdena += face.ResetAdena;
        }

        if (player.getAdena() < requiredAdena)
        {
            player.sendPacket(new ExResponseBeautyRegistResetPacket(player, ExResponseBeautyRegistResetPacket.RESTORE,
                ExResponseBeautyRegistResetPacket.FAILURE));

            return ValueTask.CompletedTask;
        }

        if (requiredAdena > 0 && !player.reduceAdena(GetType().Name, requiredAdena, null, true))
        {
            player.sendPacket(new ExResponseBeautyRegistResetPacket(player, ExResponseBeautyRegistResetPacket.RESTORE,
                ExResponseBeautyRegistResetPacket.FAILURE));

            return ValueTask.CompletedTask;
        }

        player.getVariables().Remove("visualHairId");
        player.getVariables().Remove("visualHairColorId");
        player.getVariables().Remove("visualFaceId");

        player.sendPacket(new ExResponseBeautyRegistResetPacket(player, ExResponseBeautyRegistResetPacket.RESTORE,
            ExResponseBeautyRegistResetPacket.SUCCESS));

        return ValueTask.CompletedTask;
    }
}
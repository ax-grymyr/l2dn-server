using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.BeautyShop;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestRegistBeautyPacket: IIncomingPacket<GameSession>
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

		BeautyData? beautyData = BeautyShopData.getInstance()
			.getBeautyData(player.getRace(), player.getAppearance().getSex());

		int requiredAdena = 0;
		int requiredBeautyShopTicket = 0;
		if (_hairId > 0)
		{
			BeautyItem? hair = beautyData?.getHairList().GetValueOrDefault(_hairId);
			if (hair == null)
			{
				player.sendPacket(new ExResponseBeautyRegistResetPacket(player,
					ExResponseBeautyRegistResetPacket.CHANGE, ExResponseBeautyRegistResetPacket.FAILURE));

				player.sendPacket(new ExResponseBeautyListPacket(player, ExResponseBeautyListPacket.SHOW_FACESHAPE));

				return ValueTask.CompletedTask;
			}

			if (hair.getId() != player.getVisualHair())
			{
				requiredAdena += hair.getAdena();
				requiredBeautyShopTicket += hair.getBeautyShopTicket();
			}

			if (_colorId > 0)
			{
				BeautyItem? color = hair.getColors().GetValueOrDefault(_colorId);
				if (color == null)
				{
					player.sendPacket(new ExResponseBeautyRegistResetPacket(player,
						ExResponseBeautyRegistResetPacket.CHANGE, ExResponseBeautyRegistResetPacket.FAILURE));

					player.sendPacket(new ExResponseBeautyListPacket(player,
						ExResponseBeautyListPacket.SHOW_FACESHAPE));

					return ValueTask.CompletedTask;
				}

				requiredAdena += color.getAdena();
				requiredBeautyShopTicket += color.getBeautyShopTicket();
			}
		}

		if (_faceId > 0 && _faceId != player.getVisualFace())
		{
			BeautyItem? face = beautyData?.getFaceList().GetValueOrDefault(_faceId);
			if (face == null)
			{
				player.sendPacket(new ExResponseBeautyRegistResetPacket(player,
					ExResponseBeautyRegistResetPacket.CHANGE, ExResponseBeautyRegistResetPacket.FAILURE));

				player.sendPacket(new ExResponseBeautyListPacket(player, ExResponseBeautyListPacket.SHOW_FACESHAPE));
				return ValueTask.CompletedTask;
			}

			requiredAdena += face.getAdena();
			requiredBeautyShopTicket += face.getBeautyShopTicket();
		}

		if (player.getAdena() < requiredAdena || player.getBeautyTickets() < requiredBeautyShopTicket)
		{
			player.sendPacket(new ExResponseBeautyRegistResetPacket(player, ExResponseBeautyRegistResetPacket.CHANGE,
				ExResponseBeautyRegistResetPacket.FAILURE));

			player.sendPacket(new ExResponseBeautyListPacket(player, ExResponseBeautyListPacket.SHOW_FACESHAPE));
			return ValueTask.CompletedTask;
		}

		if (requiredAdena > 0 && !player.reduceAdena(GetType().Name, requiredAdena, null, true))
		{
			player.sendPacket(new ExResponseBeautyRegistResetPacket(player, ExResponseBeautyRegistResetPacket.CHANGE,
				ExResponseBeautyRegistResetPacket.FAILURE));

			player.sendPacket(new ExResponseBeautyListPacket(player, ExResponseBeautyListPacket.SHOW_FACESHAPE));
			return ValueTask.CompletedTask;
		}

		if (requiredBeautyShopTicket > 0 &&
		    !player.reduceBeautyTickets(GetType().Name, requiredBeautyShopTicket, null, true))
		{
			player.sendPacket(new ExResponseBeautyRegistResetPacket(player, ExResponseBeautyRegistResetPacket.CHANGE,
				ExResponseBeautyRegistResetPacket.FAILURE));

			player.sendPacket(new ExResponseBeautyListPacket(player, ExResponseBeautyListPacket.SHOW_FACESHAPE));
			return ValueTask.CompletedTask;
		}

		if (_hairId > 0)
		{
			player.setVisualHair(_hairId);
		}

		if (_colorId > 0)
		{
			player.setVisualHairColor(_colorId);
		}

		if (_faceId > 0)
		{
			player.setVisualFace(_faceId);
		}

		player.sendPacket(new ExResponseBeautyRegistResetPacket(player, ExResponseBeautyRegistResetPacket.CHANGE,
			ExResponseBeautyRegistResetPacket.SUCCESS));

		return ValueTask.CompletedTask;
	}
}
using L2Dn.Extensions;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Commission;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Network.OutgoingPackets.Commission;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Commission;

public struct RequestCommissionListPacket: IIncomingPacket<GameSession>
{
    private int _treeViewDepth;
    private int _itemType;
    private int _type;
    private int _grade;
    private string _query;

    public void ReadContent(PacketBitReader reader)
    {
        _treeViewDepth = reader.ReadInt32();
        _itemType = reader.ReadInt32();
        _type = reader.ReadInt32();
        _grade = reader.ReadInt32();
        _query = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		if (!ItemCommissionManager.isPlayerAllowedToInteract(player))
		{
			player.sendPacket(ExCloseCommissionPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}
		
		Predicate<ItemTemplate>? filter = null;
		switch (_treeViewDepth)
		{
			case 1:
			{
				CommissionTreeType commissionTreeType = (CommissionTreeType)_itemType;
				if (Enum.IsDefined(commissionTreeType))
				{
					filter = i => commissionTreeType.GetCommissionItemTypes().Contains(i.getCommissionItemType());
				}
				
				break;
			}
			
			case 2:
			{
				CommissionItemType commissionItemType = (CommissionItemType)_itemType;
				if (Enum.IsDefined(commissionItemType))
				{
					filter = i => i.getCommissionItemType() == commissionItemType;
				}
				
				break;
			}
		}
		
		switch (_type)
		{
			case 0: // General
			{
				//filter = filter.and(i => true); // TODO: condition
				break;
			}
			case 1: // Rare
			{
				//filter = filter.and(i => true); // TODO: condition
				break;
			}
		}

		CrystalType? crystalType = _grade switch
		{
			0 => CrystalType.NONE,
			1 => CrystalType.D,
			2 => CrystalType.C,
			3 => CrystalType.B,
			4 => CrystalType.A,
			5 => CrystalType.S,
			6 => CrystalType.S80,
			7 => CrystalType.R,
			8 => CrystalType.R95,
			9 => CrystalType.R99,
			_ => null
		};

		if (crystalType is not null)
		{
			CrystalType crystalTypeValue = crystalType.Value;
			if (filter is null)
				filter = i => i.getCrystalType() == crystalTypeValue;
			else
				filter = filter.And(i => i.getCrystalType() == crystalTypeValue);
		}

		if (!string.IsNullOrEmpty(_query))
		{
			string query = _query;
			if (filter is null)
				filter = i => i.getName().toLowerCase().contains(query.toLowerCase());
			else
				filter = filter.And(i => i.getName().toLowerCase().contains(query.toLowerCase()));
		}
		
		ItemCommissionManager.getInstance().showAuctions(player, filter ?? (_ => true));
        
        return ValueTask.CompletedTask;
    }
}
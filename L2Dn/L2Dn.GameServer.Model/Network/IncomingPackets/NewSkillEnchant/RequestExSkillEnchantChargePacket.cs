using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.NewSkillEnchant;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.NewSkillEnchant;

public struct RequestExSkillEnchantChargePacket: IIncomingPacket<GameSession>
{
    private int _skillId;
    private List<ItemHolder>? _itemList;

    public void ReadContent(PacketBitReader reader)
    {
        _skillId = reader.ReadInt32();
        reader.ReadInt32(); // level
        reader.ReadInt32(); // sublevel
        int size = reader.ReadInt32();
        if (size > 0 && size < (ushort.MaxValue - 20) / 12)
        {
            _itemList = new List<ItemHolder>(size);
            for (int i = 0; i < size; i++)
            {
                int objectId = reader.ReadInt32();
                long count = reader.ReadInt64();
                _itemList.Add(new ItemHolder(objectId, count));
            }
        }
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
	    Player? player = session.Player;
	    if (player == null || _itemList is null)
		    return ValueTask.CompletedTask;

	    Skill? skill = player.getKnownSkill(_skillId);
	    if (skill == null)
		    return ValueTask.CompletedTask;

	    SkillEnchantHolder? skillEnchantHolder = SkillEnchantData.Instance.GetSkillEnchant(skill.Id);
	    if (skillEnchantHolder == null)
		    return ValueTask.CompletedTask;

	    EnchantStarHolder? starHolder = SkillEnchantData.Instance.GetEnchantStar(skillEnchantHolder.StarLevel);
	    if (starHolder == null)
		    return ValueTask.CompletedTask;

	    long curExp = player.getSkillEnchantExp(starHolder.Level);
	    long feeAdena = 0;
	    foreach (ItemHolder itemCharge in _itemList)
	    {
		    Item? item = player.getInventory().getItemByObjectId(itemCharge.Id);
		    if (item == null)
		    {
			    PacketLogger.Instance.Warn(GetType().Name + " Player" + player.getName() +
			                               " trying charge skill exp enchant with not exist item by objectId - " +
			                               itemCharge.Id);

			    continue;
		    }

		    EnchantItemExpHolder? itemExpHolder =
			    SkillEnchantData.Instance.GetEnchantItem(starHolder.Level, item.Id);
		    if (itemExpHolder != null)
		    {
			    feeAdena = itemCharge.getCount() * starHolder.FeeAdena;
			    if (player.getAdena() < feeAdena)
			    {
				    player.sendPacket(SystemMessageId.NOT_ENOUGH_ADENA);
				    player.sendPacket(new ExSkillEnchantChargePacket(skill.Id, 0));
				    return ValueTask.CompletedTask;
			    }

			    if (itemExpHolder.StarLevel <= starHolder.Level)
			    {
				    curExp += itemExpHolder.Exp * itemCharge.getCount();
				    player.destroyItem("Charge", item, itemCharge.getCount(), null, true);
			    }
			    else
			    {
				    PacketLogger.Instance.Warn(GetType().Name + " Player" + player.getName() +
				                               " trying charge item with not support star level skillstarLevel-" +
				                               starHolder.Level + " itemStarLevel-" +
				                               itemExpHolder.StarLevel + " itemId-" + itemExpHolder.Id);
			    }
		    }
		    else
		    {
			    PacketLogger.Instance.Warn(GetType().Name + " Player" + player.getName() +
			                               " trying charge skill with missed item on XML  itemId-" + item.Id);
		    }
	    }

	    player.setSkillEnchantExp(starHolder.Level, Math.Min(starHolder.ExpMax, curExp));
	    player.reduceAdena("ChargeFee", feeAdena, null, true);
	    player.sendPacket(new ExSkillEnchantChargePacket(skill.Id, 0));
	    player.sendPacket(new ExSkillEnchantInfoPacket(skill, player));

	    return ValueTask.CompletedTask;
    }
}
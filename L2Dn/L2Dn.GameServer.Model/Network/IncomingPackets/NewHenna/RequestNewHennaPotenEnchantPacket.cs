using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Henna;
using L2Dn.GameServer.Network.OutgoingPackets.NewHenna;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Network.IncomingPackets.NewHenna;

public struct RequestNewHennaPotenEnchantPacket: IIncomingPacket<GameSession>
{
    private int _slotId;
    private int _costItemId;

    public void ReadContent(PacketBitReader reader)
    {
        _slotId = reader.ReadByte();
        _costItemId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		int dailyStep = player.getDyePotentialDailyStep();
		int dailyCount = player.getDyePotentialDailyCount();
		if (_slotId < 1 || _slotId > 4)
		{
			return ValueTask.CompletedTask;
		}

		HennaPoten? hennaPattern = player.getHennaPoten(_slotId);
		int enchantExp = hennaPattern.getEnchantExp();
		int fullExpNeeded = HennaPatternPotentialData.getInstance().getExpForLevel(hennaPattern.getEnchantLevel());
		if (enchantExp >= fullExpNeeded && hennaPattern.getEnchantLevel() == 20)
		{
			player.sendPacket(new NewHennaPotenEnchantPacket(_slotId, hennaPattern.getEnchantLevel(),
				hennaPattern.getEnchantExp(), dailyStep, dailyCount, hennaPattern.getActiveStep(), true));

			return ValueTask.CompletedTask;
		}

        DyePotentialFee? currentFee = HennaPatternPotentialData.getInstance().getFee(dailyStep);
		if (currentFee == null || dailyCount <= 0)
			return ValueTask.CompletedTask;

		int costItemId = _costItemId;
		ItemHolder? itemFee = currentFee.getItems().FirstOrDefault(ih => ih.getId() == costItemId);
		if (itemFee == null || !player.destroyItemByItemId(GetType().Name, itemFee.getId(), itemFee.getCount(), player, true))
			return ValueTask.CompletedTask;

		dailyCount -= 1;
		if (dailyCount <= 0 && dailyStep != HennaPatternPotentialData.getInstance().getMaxPotenEnchantStep())
		{
			dailyStep += 1;
			DyePotentialFee? newFee = HennaPatternPotentialData.getInstance().getFee(dailyStep);
			if (newFee != null)
			{
				dailyCount = 0;
			}
			player.setDyePotentialDailyCount(dailyCount);
			// player.setDyePotentialDailyStep(dailyStep);
		}
		else
		{
			player.setDyePotentialDailyCount(dailyCount);
		}

		double totalChance = 0;
		double random = Rnd.nextDouble() * 100;
		foreach (var entry in currentFee.getEnchantExp())
		{
			totalChance += entry.Value;
			if (random <= totalChance)
			{
				int increase = entry.Key;
				int newEnchantExp = hennaPattern.getEnchantExp() + increase;
				int PatternExpNeeded = HennaPatternPotentialData.getInstance().getExpForLevel(hennaPattern.getEnchantLevel());
				if (newEnchantExp >= PatternExpNeeded && hennaPattern.getEnchantLevel() < 20)
				{
					newEnchantExp -= PatternExpNeeded;
					if (hennaPattern.getEnchantLevel() < HennaPatternPotentialData.getInstance().getMaxPotenLevel())
					{
						hennaPattern.setEnchantLevel(hennaPattern.getEnchantLevel() + 1);
						player.applyDyePotenSkills();
					}
				}
				hennaPattern.setEnchantExp(newEnchantExp);
				hennaPattern.setSlotPosition(_slotId);

				player.sendPacket(new NewHennaPotenEnchantPacket(_slotId, hennaPattern.getEnchantLevel(),
					hennaPattern.getEnchantExp(), dailyStep, dailyCount, hennaPattern.getActiveStep(), true));

				return ValueTask.CompletedTask;
			}
		}

        return ValueTask.CompletedTask;
    }
}
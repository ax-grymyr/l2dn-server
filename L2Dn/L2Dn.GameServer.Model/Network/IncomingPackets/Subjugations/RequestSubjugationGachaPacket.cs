using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Network.OutgoingPackets.Subjugation;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Network.IncomingPackets.Subjugations;

public struct RequestSubjugationGachaPacket: IIncomingPacket<GameSession>
{
    private int _category;
    private int _amount;

    public void ReadContent(PacketBitReader reader)
    {
        _category = reader.ReadInt32();
        _amount = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        if (_amount < 1 || _amount * 20000L < 1)
            return ValueTask.CompletedTask;

        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        PurgePlayerHolder? playerKeys = player.getPurgePoints().get(_category);
        SubjugationHolder? subjugation = SubjugationData.Instance.GetSubjugation(_category);
        if (subjugation == null || subjugation.Items.Count == 0)
            return ValueTask.CompletedTask;

        double maxBound = subjugation.Items.Sum(x => x.Value);
        if (playerKeys != null && playerKeys.getKeys() >= _amount && player.getInventory().getAdena() > 20000L * _amount)
        {
            player.getInventory().reduceAdena("Purge Gacha", 20000L * _amount, player, null);
            int curKeys = playerKeys.getKeys() - _amount;
            player.getPurgePoints().put(_category, new PurgePlayerHolder(playerKeys.getPoints(), curKeys, 0));
            Map<int, int> rewards = new();
            for (int i = 0; i < _amount; i++)
            {
                double rate = 0;
                // TODO: does items order matter?
                foreach ((int itemId, double itemChance) in subjugation.Items)
                {
                    if (Rnd.get(maxBound - rate) < itemChance)
                    {
                        rewards.put(itemId, rewards.GetValueOrDefault(itemId) + 1);
                        player.addItem("Purge Gacha", itemId, 1, player, true);
                        break;
                    }

                    rate += itemChance;
                }
            }
            player.sendPacket(new ExSubjugationGachaUiPacket(curKeys));
            player.sendPacket(new ExSubjugationGachaPacket(rewards));
        }

        return ValueTask.CompletedTask;
    }
}
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Network.OutgoingPackets.Subjugation;
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

        PurgePlayerHolder playerKeys = player.getPurgePoints().get(_category);
        Map<int, double> subjugationData = SubjugationGacha.getInstance().getSubjugation(_category);
        KeyValuePair<int, double>[] subjugationDataArray = subjugationData.ToArray();
        double maxBound = subjugationDataArray.Sum(x => x.Value);
        if (playerKeys != null && playerKeys.getKeys() >= _amount && player.getInventory().getAdena() > 20000L * _amount)
        {
            player.getInventory().reduceAdena("Purge Gacha", 20000L * _amount, player, null);
            int curKeys = playerKeys.getKeys() - _amount;
            player.getPurgePoints().put(_category, new PurgePlayerHolder(playerKeys.getPoints(), curKeys, 0));
            Map<int, int> rewards = new();
            for (int i = 0; i < _amount; i++)
            {
                double rate = 0;
                for (int index = 0; index < subjugationDataArray.Length; index++)
                {
                    double itemChance = subjugationDataArray[index].Value;
                    if (Rnd.get(maxBound - rate) < itemChance)
                    {
                        int itemId = subjugationDataArray[index].Key;
                        rewards.put(itemId, rewards.getOrDefault(itemId, 0) + 1);
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
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.RandomCraft;

public readonly struct ExCraftRandomInfoPacket: IOutgoingPacket
{
    private readonly Player _player;
	
    public ExCraftRandomInfoPacket(Player player)
    {
        _player = player;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CRAFT_RANDOM_INFO);

        List<RandomCraftRewardItemHolder> rewards = _player.getRandomCraft().getRewards();
        int size = 5;
        writer.WriteInt32(size); // size
        for (int i = 0; i < rewards.Count; i++)
        {
            RandomCraftRewardItemHolder holder = rewards[i];
            if ((holder != null) && (holder.getItemId() != 0))
            {
                writer.WriteByte(holder.isLocked()); // Locked
                writer.WriteInt32(holder.getLockLeft()); // Rolls it will stay locked
                writer.WriteInt32(holder.getItemId()); // Item id
                writer.WriteInt64(holder.getItemCount()); // Item count
            }
            else
            {
                writer.WriteByte(0);
                writer.WriteInt32(0);
                writer.WriteInt32(0);
                writer.WriteInt64(0);
            }
            size--;
        }
        // Write missing
        for (int i = size; i > 0; i--)
        {
            writer.WriteByte(0);
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteInt64(0);
        }
    }
}
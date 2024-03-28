using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowSeedInfoPacket: IOutgoingPacket
{
    private readonly List<SeedProduction> _seeds;
    private readonly int _manorId;
    private readonly bool _hideButtons;
	
    public ExShowSeedInfoPacket(int manorId, bool nextPeriod, bool hideButtons)
    {
        _manorId = manorId;
        _hideButtons = hideButtons;
        CastleManorManager manor = CastleManorManager.getInstance();
        _seeds = (nextPeriod && !manor.isManorApproved()) ? null : manor.getSeedProduction(manorId, nextPeriod);
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_SEED_INFO);
        
        writer.WriteByte(_hideButtons); // Hide "Seed Purchase" button
        writer.WriteInt32(_manorId); // Manor ID
        writer.WriteInt32(0); // Unknown
        if (_seeds == null)
        {
            writer.WriteInt32(0);
            return;
        }
        writer.WriteInt32(_seeds.Count);
        foreach (SeedProduction seed in _seeds)
        {
            writer.WriteInt32(seed.getId()); // Seed id
            writer.WriteInt64(seed.getAmount()); // Left to buy
            writer.WriteInt64(seed.getStartAmount()); // Started amount
            writer.WriteInt64(seed.getPrice()); // Sell Price
            Seed s = CastleManorManager.getInstance().getSeed(seed.getId());
            if (s == null)
            {
                writer.WriteInt32(0); // Seed level
                writer.WriteByte(1); // Reward 1
                writer.WriteInt32(0); // Reward 1 - item id
                writer.WriteByte(1); // Reward 2
                writer.WriteInt32(0); // Reward 2 - item id
            }
            else
            {
                writer.WriteInt32(s.getLevel()); // Seed level
                writer.WriteByte(1); // Reward 1
                writer.WriteInt32(s.getReward(1)); // Reward 1 - item id
                writer.WriteByte(1); // Reward 2
                writer.WriteInt32(s.getReward(2)); // Reward 2 - item id
            }
        }
    }
}
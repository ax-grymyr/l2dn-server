using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowSeedSettingPacket: IOutgoingPacket
{
    private readonly int _manorId;
    private readonly Set<Seed> _seeds;
    private readonly Map<int, SeedProduction> _current;
    private readonly Map<int, SeedProduction> _next;

    public ExShowSeedSettingPacket(int manorId)
    {
        CastleManorManager manor = CastleManorManager.getInstance();
        _manorId = manorId;
        _seeds = manor.getSeedsForCastle(_manorId);
        _current = new Map<int, SeedProduction>();
        _next = new Map<int, SeedProduction>();
        foreach (Seed s in _seeds)
        {
            // Current period
            SeedProduction? sp = manor.getSeedProduct(manorId, s.getSeedId(), false);
            if (sp != null)
            {
                _current.put(s.getSeedId(), sp);
            }
            // Next period
            sp = manor.getSeedProduct(manorId, s.getSeedId(), true);
            if (sp != null)
            {
                _next.put(s.getSeedId(), sp);
            }
        }
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_SEED_SETTING);

        writer.WriteInt32(_manorId); // manor id
        writer.WriteInt32(_seeds.size()); // size
        foreach (Seed s in _seeds)
        {
            writer.WriteInt32(s.getSeedId()); // seed id
            writer.WriteInt32(s.getLevel()); // level
            writer.WriteByte(1);
            writer.WriteInt32(s.getReward(1)); // reward 1 id
            writer.WriteByte(1);
            writer.WriteInt32(s.getReward(2)); // reward 2 id
            writer.WriteInt32(s.getSeedLimit()); // next sale limit
            writer.WriteInt32(s.getSeedReferencePrice()); // price for castle to produce 1
            writer.WriteInt32(s.getSeedMinPrice()); // min seed price
            writer.WriteInt32(s.getSeedMaxPrice()); // max seed price

            // Current period
            if (_current.TryGetValue(s.getSeedId(), out SeedProduction? sp))
            {
                writer.WriteInt64(sp.getStartAmount()); // sales
                writer.WriteInt64(sp.getPrice()); // price
            }
            else
            {
                writer.WriteInt64(0);
                writer.WriteInt64(0);
            }

            // Next period
            if (_next.TryGetValue(s.getSeedId(), out sp))
            {
                writer.WriteInt64(sp.getStartAmount()); // sales
                writer.WriteInt64(sp.getPrice()); // price
            }
            else
            {
                writer.WriteInt64(0);
                writer.WriteInt64(0);
            }
        }
    }
}
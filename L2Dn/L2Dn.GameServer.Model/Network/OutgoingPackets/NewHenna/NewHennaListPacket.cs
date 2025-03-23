using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Henna;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.NewHenna;

public readonly struct NewHennaListPacket: IOutgoingPacket
{
    private readonly HennaPoten[] _hennaId;
    private readonly int _dailyStep;
    private readonly int _dailyCount;
    private readonly int _availableSlots;
    private readonly int _resetCount;
    private readonly int _sendType;
    private readonly List<ItemHolder> _resetData;

    public NewHennaListPacket(Player player, int sendType)
    {
        _dailyStep = player.getDyePotentialDailyStep();
        _dailyCount = player.getDyePotentialDailyCount();
        _hennaId = player.getHennaPotenList();
        _availableSlots = player.getAvailableHennaSlots();
        _resetCount = player.getDyePotentialDailyEnchantReset();
        _resetData = HennaPatternPotentialData.getInstance().getEnchantReset();
        _sendType = sendType;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_NEW_HENNA_LIST);

        writer.WriteByte((byte)_sendType); // SendType
        writer.WriteInt16((short)_dailyStep);
        writer.WriteInt16((short)_dailyCount);
        writer.WriteInt16((short)(_resetCount + 1)); // ResetCount
        writer.WriteInt16((short)_resetData.Count); // ResetMaxCount

        ItemHolder? resetInfo = _resetCount >= 0 && _resetCount < _resetData.Count ? _resetData[_resetCount] : null;
        if (resetInfo != null)
        {
            writer.WriteInt32(1);
            writer.WriteInt32(resetInfo.Id);
            writer.WriteInt64(resetInfo.Count);
        }
        else
        {
            writer.WriteInt32(0);
        }

        // hennaInfoList
        writer.WriteInt32(_hennaId.Length);
        for (int i = 1; i <= _hennaId.Length; i++)
        {
            HennaPoten hennaPoten = _hennaId[i - 1];
            Henna? henna = _hennaId[i - 1].getHenna();
            writer.WriteInt32(henna != null ? henna.getDyeId() : 0);
            writer.WriteInt32(hennaPoten.getPotenId());
            writer.WriteByte(i != _availableSlots);
            writer.WriteInt16((short)hennaPoten.getEnchantLevel());
            writer.WriteInt32(hennaPoten.getEnchantExp());
            writer.WriteInt16((short)hennaPoten.getActiveStep());
            writer.WriteInt16((short)_dailyStep);
            writer.WriteInt16((short)_dailyCount);
        }
    }
}
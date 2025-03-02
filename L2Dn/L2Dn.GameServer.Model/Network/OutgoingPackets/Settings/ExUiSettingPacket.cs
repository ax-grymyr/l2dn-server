using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Variables;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Settings;

public readonly struct ExUiSettingPacket: IOutgoingPacket
{
    private readonly byte[]? _uiKeyMapping;

    public ExUiSettingPacket(Player player)
    {
        _uiKeyMapping = player.getVariables().Get<byte[]>(PlayerVariables.UI_KEY_MAPPING);
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_UI_SETTING);

        if (_uiKeyMapping != null)
        {
            writer.WriteInt32(_uiKeyMapping.Length);
            writer.WriteBytes(_uiKeyMapping);
        }
        else
        {
            writer.WriteInt32(0);
        }
    }
}
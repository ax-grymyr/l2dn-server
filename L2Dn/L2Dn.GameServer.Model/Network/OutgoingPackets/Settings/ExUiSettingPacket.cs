using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Variables;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Settings;

public readonly struct ExUiSettingPacket: IOutgoingPacket
{
    public const string SPLIT_VAR = "	";

    private readonly byte[]? _uiKeyMapping;

    public ExUiSettingPacket(Player player)
    {
        if (player.getVariables().hasVariable(PlayerVariables.UI_KEY_MAPPING))
        {
            _uiKeyMapping = player.getVariables().getByteArray(PlayerVariables.UI_KEY_MAPPING, SPLIT_VAR);
        }
        else
        {
            _uiKeyMapping = null;
        }
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
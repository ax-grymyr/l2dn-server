using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Variables;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Settings;

public struct RequestSaveKeyMappingPacket: IIncomingPacket<GameSession>
{
    public const string SPLIT_VAR = "	";
	
    private byte[]? _uiKeyMapping;

    public void ReadContent(PacketBitReader reader)
    {
        int dataSize = reader.ReadInt32();
        if (dataSize > 0 && dataSize < 65530)
        {
            _uiKeyMapping = reader.ReadBytes(dataSize).ToArray();
        }
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (!Config.STORE_UI_SETTINGS || player == null || _uiKeyMapping == null ||
            session.State != GameSessionState.InGame)
        {
            return ValueTask.CompletedTask;
        }

        string uiKeyMapping = "";
        foreach (byte b in _uiKeyMapping)
        {
            uiKeyMapping += b + SPLIT_VAR;
        }
        
        player.getVariables().set(PlayerVariables.UI_KEY_MAPPING, uiKeyMapping);
        
        return ValueTask.CompletedTask;
    }
}
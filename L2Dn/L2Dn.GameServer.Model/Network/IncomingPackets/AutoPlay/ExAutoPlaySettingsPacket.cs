using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.OutgoingPackets.AutoPlay;
using L2Dn.GameServer.TaskManagers;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.AutoPlay;

public struct ExAutoPlaySettingsPacket: IIncomingPacket<GameSession>
{
    private short _options;
    private bool _active;
    private bool _pickUp;
    private short _nextTargetMode;
    private bool _shortRange;
    private int _potionPercent;
    private int _petPotionPercent;
    private bool _respectfulHunting;
    private byte _macroIndex;

    public void ReadContent(PacketBitReader reader)
    {
        _options = reader.ReadInt16();
        _active = reader.ReadByte() == 1;
        _pickUp = reader.ReadByte() == 1;
        _nextTargetMode = reader.ReadInt16();
        _shortRange = reader.ReadByte() == 1;
        _potionPercent = reader.ReadInt32();
        _petPotionPercent = reader.ReadInt32(); // 272
        _respectfulHunting = reader.ReadByte() == 1;
        _macroIndex = reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        // Skip first run. Fixes restored settings been overwritten.
        // Client sends a disabled ExAutoPlaySetting upon player login.
        if (player.hasResumedAutoPlay())
        {
            player.setResumedAutoPlay(false);
            return ValueTask.CompletedTask;
        }

        ExAutoPlaySettingSendPacket autoPlaySettingPacket = new ExAutoPlaySettingSendPacket(_options, _active, _pickUp,
            _nextTargetMode, _shortRange, _potionPercent, _respectfulHunting, _petPotionPercent);

        connection.Send(ref autoPlaySettingPacket);

        player.getAutoPlaySettings().setAutoPotionPercent(_potionPercent);

        if (!Config.ENABLE_AUTO_PLAY)
            return ValueTask.CompletedTask;

        List<int> settings =
        [
            _options,
            _active ? 1 : 0,
            _pickUp ? 1 : 0,
            _nextTargetMode,
            _shortRange ? 1 : 0,
            _potionPercent,
            _respectfulHunting ? 1 : 0,
            _petPotionPercent,
            _macroIndex
        ];

        player.getVariables().Set(PlayerVariables.AUTO_USE_SETTINGS, settings);

        player.getAutoPlaySettings().setOptions(_options);
        player.getAutoPlaySettings().setPickup(_pickUp);
        player.getAutoPlaySettings().setNextTargetMode(_nextTargetMode);
        player.getAutoPlaySettings().setShortRange(_shortRange);
        player.getAutoPlaySettings().setRespectfulHunting(_respectfulHunting);
        player.getAutoPlaySettings().setAutoPetPotionPercent(_petPotionPercent);
        player.getAutoPlaySettings().setMacroIndex(_macroIndex);

        if (_active)
        {
            AutoPlayTaskManager.getInstance().startAutoPlay(player);
        }
        else
        {
            AutoPlayTaskManager.getInstance().stopAutoPlay(player);
        }

        return ValueTask.CompletedTask;
    }
}
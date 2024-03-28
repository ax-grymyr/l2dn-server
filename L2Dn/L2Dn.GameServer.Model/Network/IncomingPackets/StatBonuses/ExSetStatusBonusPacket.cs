using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Model.Variables;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.StatBonuses;

public struct ExSetStatusBonusPacket: IIncomingPacket<GameSession>
{
    private int _str;
    private int _dex;
    private int _con;
    private int _int;
    private int _wit;
    private int _men;

    public void ReadContent(PacketBitReader reader)
    {
        reader.ReadInt16(); // unknown
        reader.ReadInt16(); // total bonus
        _str = reader.ReadInt16();
        _dex = reader.ReadInt16();
        _con = reader.ReadInt16();
        _int = reader.ReadInt16();
        _wit = reader.ReadInt16();
        _men = reader.ReadInt16();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_str < 0 || _dex < 0 || _con < 0 || _int < 0 || _wit < 0 || _men < 0)
            return ValueTask.CompletedTask;

        int usedPoints = player.getVariables().getInt(PlayerVariables.STAT_POINTS, 0);
        int effectBonus = (int)player.getStat().getValue(Stat.ELIXIR_USAGE_LIMIT, 0);
        int elixirsAvailable = player.getVariables().getInt(PlayerVariables.ELIXIRS_AVAILABLE, 0) + effectBonus;
        int currentPoints = _str + _dex + _con + _int + _wit + _men;
        int possiblePoints = player.getLevel() < 76 ? 0 : player.getLevel() - 75 + elixirsAvailable - usedPoints;
        if (possiblePoints <= 0 || currentPoints > possiblePoints)
            return ValueTask.CompletedTask;

        if (_str > 0)
        {
            player.getVariables().set(PlayerVariables.STAT_STR,
                player.getVariables().getInt(PlayerVariables.STAT_STR, 0) + _str);
        }

        if (_dex > 0)
        {
            player.getVariables().set(PlayerVariables.STAT_DEX,
                player.getVariables().getInt(PlayerVariables.STAT_DEX, 0) + _dex);
        }

        if (_con > 0)
        {
            player.getVariables().set(PlayerVariables.STAT_CON,
                player.getVariables().getInt(PlayerVariables.STAT_CON, 0) + _con);
        }

        if (_int > 0)
        {
            player.getVariables().set(PlayerVariables.STAT_INT,
                player.getVariables().getInt(PlayerVariables.STAT_INT, 0) + _int);
        }

        if (_wit > 0)
        {
            player.getVariables().set(PlayerVariables.STAT_WIT,
                player.getVariables().getInt(PlayerVariables.STAT_WIT, 0) + _wit);
        }

        if (_men > 0)
        {
            player.getVariables().set(PlayerVariables.STAT_MEN,
                player.getVariables().getInt(PlayerVariables.STAT_MEN, 0) + _men);
        }

        player.getStat().recalculateStats(true);

        // Calculate stat increase skills.
        player.calculateStatIncreaseSkills();
        player.updateUserInfo();

        return ValueTask.CompletedTask;
    }
}
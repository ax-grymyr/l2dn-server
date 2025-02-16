using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExOlympiadUserInfoPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly Participant _par = null;
    private readonly int _curHp;
    private readonly int _maxHp;
    private readonly int _curCp;
    private readonly int _maxCp;

    public ExOlympiadUserInfoPacket(Player player)
    {
        _player = player;
        if (_player != null)
        {
            _curHp = (int)_player.getCurrentHp();
            _maxHp = _player.getMaxHp();
            _curCp = (int)_player.getCurrentCp();
            _maxCp = _player.getMaxCp();
        }
        else
        {
            _curHp = 0;
            _maxHp = 100;
            _curCp = 0;
            _maxCp = 100;
        }
    }

    public ExOlympiadUserInfoPacket(Participant par)
    {
        _par = par;
        _player = par.getPlayer();
        if (_player != null)
        {
            _curHp = (int)_player.getCurrentHp();
            _maxHp = _player.getMaxHp();
            _curCp = (int)_player.getCurrentCp();
            _maxCp = _player.getMaxCp();
        }
        else
        {
            _curHp = 0;
            _maxHp = 100;
            _curCp = 0;
            _maxCp = 100;
        }
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_OLYMPIAD_USER_INFO);
        if (_player != null)
        {
            writer.WriteByte((byte)_player.getOlympiadSide());
            writer.WriteInt32(_player.ObjectId);
            writer.WriteString(_player.getName());
            writer.WriteInt32((int)_player.getClassId());
        }
        else
        {
            writer.WriteByte((byte)_par.getSide());
            writer.WriteInt32(_par.getObjectId());
            writer.WriteString(_par.getName());
            writer.WriteInt32((int)_par.getBaseClass());
        }

        writer.WriteInt32(_curHp);
        writer.WriteInt32(_maxHp);
        writer.WriteInt32(_curCp);
        writer.WriteInt32(_maxCp);
    }
}
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExOlympiadUserInfoPacket: IOutgoingPacket
{
    private readonly int _olympiadSide;
    private readonly int _playerObjectId;
    private readonly string _playerName;
    private readonly CharacterClass _playerClass;
    private readonly int _curHp;
    private readonly int _maxHp;
    private readonly int _curCp;
    private readonly int _maxCp;

    public ExOlympiadUserInfoPacket(Player player)
    {
        _olympiadSide = player.getOlympiadSide();
        _playerObjectId = player.ObjectId;
        _playerName = player.getName();
        _playerClass = player.getClassId();

        _curHp = (int)player.getCurrentHp();
        _maxHp = player.getMaxHp();
        _curCp = (int)player.getCurrentCp();
        _maxCp = player.getMaxCp();
    }

    public ExOlympiadUserInfoPacket(Participant par)
    {
        _olympiadSide = par.getSide();
        _playerObjectId = par.getObjectId();
        _playerName = par.getName();
        _playerClass = par.getBaseClass();

        Player player = par.getPlayer();
        if (player != null)
        {
            _curHp = (int)player.getCurrentHp();
            _maxHp = player.getMaxHp();
            _curCp = (int)player.getCurrentCp();
            _maxCp = player.getMaxCp();
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

        writer.WriteByte((byte)_olympiadSide);
        writer.WriteInt32(_playerObjectId);
        writer.WriteString(_playerName);
        writer.WriteInt32((int)_playerClass);

        writer.WriteInt32(_curHp);
        writer.WriteInt32(_maxHp);
        writer.WriteInt32(_curCp);
        writer.WriteInt32(_maxCp);
    }
}
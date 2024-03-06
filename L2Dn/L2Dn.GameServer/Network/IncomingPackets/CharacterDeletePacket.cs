using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Players;
using L2Dn.GameServer.Model.Events.Returns;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct CharacterDeletePacket: IIncomingPacket<GameSession>
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(CharacterDeletePacket)); 
    private int _charSlot;

    public void ReadContent(PacketBitReader reader)
    {
        _charSlot = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        try
        {
            CharacterDeleteFailReason failType = CharacterPacketHelper.MarkToDeleteChar(session, _charSlot);
            switch (failType)
            {
                case CharacterDeleteFailReason.None: // Success
                {
                    CharacterDeleteSuccessPacket deleteSuccessPacket = new();
                    connection.Send(ref deleteSuccessPacket);

                    CharSelectInfoPackage charInfo = session.Characters[_charSlot];
                    if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_DELETE, Containers.Players()))
                    {
                        EventDispatcher.getInstance().notifyEvent<AbstractEventReturn>(
                            new OnPlayerDelete(charInfo.getObjectId(), charInfo.getName(), session),
                            Containers.Players());
                    }

                    session.Characters = CharacterPacketHelper.LoadCharacterSelectInfo(session.AccountId);
                    break;
                }
                default:
                {
                    CharacterDeleteFailPacket characterDeleteFailPacket = new(failType);
                    connection.Send(ref characterDeleteFailPacket);
                    break;
                }
            }
        }
        catch (Exception e)
        {
            _logger.Error(e);
        }

        CharacterListPacket characterListPacket = new(session.AccountId, session.AccountName, session.Characters);
        connection.Send(ref characterListPacket);
        return ValueTask.CompletedTask;
    }
}

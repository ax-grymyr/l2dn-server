using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct CharacterDeletePacket: IIncomingPacket<GameSession>
{
    private int _charSlot;

    public void ReadContent(PacketBitReader reader)
    {
        _charSlot = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        if (session.Characters is null)
        {
            // Characters must be loaded in AuthLoginPacket
            connection.Close();
            return ValueTask.CompletedTask;
        }
        
        try
        {
            CharacterDeleteFailReason failType = session.Characters.MarkToDelete(_charSlot, out CharacterInfo? charInfo);
            if (failType != CharacterDeleteFailReason.None)
            {
                CharacterDeleteFailPacket characterDeleteFailPacket = new(failType);
                connection.Send(ref characterDeleteFailPacket);
            }
            else if (charInfo is null)
            {
                CharacterDeleteFailPacket characterDeleteFailPacket = new(CharacterDeleteFailReason.Unknown);
                connection.Send(ref characterDeleteFailPacket);
            }
            else
            {
                CharacterDeleteSuccessPacket deleteSuccessPacket = new();
                connection.Send(ref deleteSuccessPacket);

                if (GlobalEvents.Players.HasSubscribers<OnPlayerDelete>())
                {
                    GlobalEvents.Players.Notify(new OnPlayerDelete(charInfo.Id, charInfo.Name,
                        session));
                }
            }
        }
        catch (Exception e)
        {
            PacketLogger.Instance.Error(e);
        }

        CharacterListPacket characterListPacket = new(session.PlayKey1, session.AccountName, session.Characters);
        connection.Send(ref characterListPacket);
        return ValueTask.CompletedTask;
    }
}

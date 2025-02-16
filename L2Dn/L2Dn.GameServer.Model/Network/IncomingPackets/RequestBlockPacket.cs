using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestBlockPacket: IIncomingPacket<GameSession>
{
    private const int BLOCK = 0;
    private const int UNBLOCK = 1;
    private const int BLOCKLIST = 2;
    private const int ALLBLOCK = 3;
    private const int ALLUNBLOCK = 4;
    
    private int _type;
    private string? _name;

    public void ReadContent(PacketBitReader reader)
    {
        _type = reader.ReadInt32(); // 0x00 - block, 0x01 - unblock, 0x03 - allblock, 0x04 - allunblock
        if (_type == BLOCK || _type == UNBLOCK)
            _name = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        switch (_type)
        {
            case BLOCK:
            case UNBLOCK:
            {
                if (string.IsNullOrEmpty(_name))
                    return ValueTask.CompletedTask;
                
                // TODO: Save in database? :P
                if (FakePlayerData.getInstance().isTalkable(_name))
                {
                    if (_type == BLOCK)
                    {
                        SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_HAS_BEEN_ADDED_TO_YOUR_IGNORE_LIST);
                        sm.Params.addString(FakePlayerData.getInstance().getProperName(_name));
                        player.sendPacket(sm);
                    }
                    else
                    {
                        SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_HAS_BEEN_REMOVED_FROM_YOUR_IGNORE_LIST);
                        sm.Params.addString(FakePlayerData.getInstance().getProperName(_name));
                        player.sendPacket(sm);
                    }

                    return ValueTask.CompletedTask;
                }
				
                // can't use block/unblock for locating invisible characters
                int targetId = CharInfoTable.getInstance().getIdByName(_name);
                if (targetId <= 0)
                {
                    // Incorrect player name.
                    player.sendPacket(SystemMessageId.ERROR_WHEN_ADDING_A_USER_TO_YOUR_IGNORE_LIST);
                    return ValueTask.CompletedTask;
                }
				
                int targetAccessLevel = CharInfoTable.getInstance().getAccessLevelById(targetId);
                if (targetAccessLevel > 0)
                {
                    // Cannot block a GM character.
                    player.sendPacket(SystemMessageId.YOU_CANNOT_BAN_A_GM);
                    return ValueTask.CompletedTask;
                }
				
                if (player.ObjectId == targetId)
                    return ValueTask.CompletedTask;
				
                if (_type == BLOCK)
                {
                    BlockList.addToBlockList(player, targetId);
                }
                else
                {
                    BlockList.removeFromBlockList(player, targetId);
                }
                
                break;
            }
            
            case BLOCKLIST:
            {
                BlockList.sendListToOwner(player);
                break;
            }
            
            case ALLBLOCK:
            {
                player.sendPacket(SystemMessageId.MESSAGE_REFUSAL_MODE);
                BlockList.setBlockAll(player, true);
                break;
            }
            
            case ALLUNBLOCK:
            {
                player.sendPacket(SystemMessageId.MESSAGE_ACCEPTANCE_MODE);
                BlockList.setBlockAll(player, false);
                break;
            }
            
            default:
            {
                PacketLogger.Instance.Warn("Unknown 0xA9 block type: " + _type);
                break;
            }
        }

        return ValueTask.CompletedTask;
    }
}
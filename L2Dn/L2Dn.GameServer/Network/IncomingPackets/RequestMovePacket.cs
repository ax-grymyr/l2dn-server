﻿using L2Dn.GameServer.AI;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Sayune;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

internal struct RequestMovePacket: IIncomingPacket<GameSession>
{
    private int _targetX;
    private int _targetY;
    private int _targetZ;
    private int _originX;
    private int _originY;
    private int _originZ;
    private int _movementMode;

    public void ReadContent(PacketBitReader reader)
    {
        _targetX = reader.ReadInt32();
        _targetY = reader.ReadInt32();
        _targetZ = reader.ReadInt32();
        _originX = reader.ReadInt32();
        _originY = reader.ReadInt32();
        _originZ = reader.ReadInt32();
        _movementMode = reader.ReadInt32(); // is 0 if cursor keys are used 1 if mouse is used
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;
		
		if ((Config.PLAYER_MOVEMENT_BLOCK_TIME > 0) && !player.isGM() && (player.getNotMoveUntil() > DateTime.UtcNow))
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_MOVE_WHILE_SPEAKING_TO_AN_NPC_ONE_MOMENT_PLEASE);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}
		
		if ((_targetX == _originX) && (_targetY == _originY) && (_targetZ == _originZ))
		{
			player.sendPacket(new StopMovePacket(player));
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}
		
		// Check for possible door logout and move over exploit. Also checked at ValidatePosition.
		if (DoorData.getInstance().checkIfDoorsBetween(player.getLastServerPosition(), player.getLocation(), player.getInstanceWorld()))
		{
			player.stopMove(player.getLastServerPosition());
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}
		
		if (_movementMode == 1)
		{
			player.setCursorKeyMovement(false);
			if (player.Events.HasSubscribers<OnPlayerMoveRequest>())
			{
				OnPlayerMoveRequest onPlayerMoveRequest = new(player, new Location(_targetX, _targetY, _targetZ));
				if (player.Events.Notify(onPlayerMoveRequest) && onPlayerMoveRequest.Terminate)
				{
					player.sendPacket(ActionFailedPacket.STATIC_PACKET);
					return ValueTask.CompletedTask;
				}
			}
		}
		else // 0
		{
			if (!Config.ENABLE_KEYBOARD_MOVEMENT)
			{
				return ValueTask.CompletedTask;
			}

			player.setCursorKeyMovement(true);
			player.setLastServerPosition(player.getX(), player.getY(), player.getZ());
		}
		
		// Correcting targetZ from floor level to head level.
		// Client is giving floor level as targetZ, but that floor level doesn't match our current geodata and teleport coordinates as good as head level!
		// L2J uses floor, not head level as char coordinates. This is some sort of incompatibility fix. Validate position packets sends head level.
		_targetZ += player.getTemplate().getCollisionHeight();
		
		AdminTeleportType teleMode = player.getTeleMode();
		switch (teleMode)
		{
			case AdminTeleportType.DEMONIC:
			{
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				player.teleToLocation(new Location(_targetX, _targetY, _targetZ));
				player.setTeleMode(AdminTeleportType.NORMAL);
				break;
			}
			case AdminTeleportType.SAYUNE:
			{
				player.sendPacket(new ExFlyMovePacket(player, SayuneType.ONE_WAY_LOC, -1, [new SayuneEntry(false, -1, _targetX, _targetY, _targetZ)]));
				player.setXYZ(_targetX, _targetY, _targetZ);
				Broadcast.toKnownPlayers(player, new ExFlyMoveBroadcastPacket(player, SayuneType.ONE_WAY_LOC, -1, new Location(_targetX, _targetY, _targetZ)));
				player.setTeleMode(AdminTeleportType.NORMAL);
				break;
			}
			case AdminTeleportType.CHARGE:
			{
				player.setXYZ(_targetX, _targetY, _targetZ);
				Broadcast.toSelfAndKnownPlayers(player, new MagicSkillUsePacket(player, 30012, 10, TimeSpan.FromMilliseconds(500), TimeSpan.Zero));
				Broadcast.toSelfAndKnownPlayers(player, new FlyToLocationPacket(player, _targetX, _targetY, _targetZ, FlyType.CHARGE));
				Broadcast.toSelfAndKnownPlayers(player, new MagicSkillLaunchedPacket(player, 30012, 10));
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				break;
			}
			default:
			{
				double dx = _targetX - player.getX();
				double dy = _targetY - player.getY();
				// Can't move if character is confused, or trying to move a huge distance
				if (player.isControlBlocked() || dx * dx + dy * dy > 98010000) // 9900*9900
				{
					player.sendPacket(ActionFailedPacket.STATIC_PACKET);
					return ValueTask.CompletedTask;
				}
				
				player.getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, new Location(_targetX, _targetY, _targetZ));
				break;
			}
		}
		
		// Remove queued skill upon move request.
		if (player.getQueuedSkill() != null)
		{
			player.setQueuedSkill(null, null, false, false);
		}
		
		// Mobius: Check spawn protections.
		player.onActionRequest();

        return ValueTask.CompletedTask;
    }
}
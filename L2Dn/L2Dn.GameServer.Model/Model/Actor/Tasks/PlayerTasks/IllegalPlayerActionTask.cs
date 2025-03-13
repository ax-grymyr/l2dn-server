using System.Text;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Punishment;
using L2Dn.GameServer.Network;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;

/**
 * Task that handles illegal player actions.
 */
public class IllegalPlayerActionTask: Runnable
{
	private static readonly Logger AUDIT_LOGGER = LogManager.GetLogger("audit");

	private readonly string _message;
	private readonly IllegalActionPunishmentType _punishment;
	private readonly Player _actor;

	public IllegalPlayerActionTask(Player actor, string message, IllegalActionPunishmentType punishment)
	{
		_message = message;
		_punishment = punishment;
		_actor = actor;

		switch (punishment)
		{
			case IllegalActionPunishmentType.KICK:
			{
				_actor.sendMessage("You will be kicked for illegal action, GM informed.");
				break;
			}
			case IllegalActionPunishmentType.KICKBAN:
			{
				if (!_actor.isGM())
				{
					_actor.setAccessLevel(-1, false, true);
					_actor.setAccountAccesslevel(-1);
				}
				_actor.sendMessage("You are banned for illegal action, GM informed.");
				break;
			}
			case IllegalActionPunishmentType.JAIL:
			{
				_actor.sendMessage("Illegal action performed!");
				_actor.sendMessage("You will be teleported to GM Consultation Service area and jailed.");
				break;
			}
		}
	}

	public void run()
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("AUDIT, ");
		sb.Append(_message);
		sb.Append(", ");
		sb.Append(_actor);
		sb.Append(", ");
		sb.Append(_punishment);
		AUDIT_LOGGER.Info(sb.ToString());

		if (!_actor.isGM())
		{
			switch (_punishment)
			{
				case IllegalActionPunishmentType.BROADCAST:
				{
					AdminData.getInstance().broadcastMessageToGMs(_message);
					return;
				}
				case IllegalActionPunishmentType.KICK:
				{
					LeaveWorldPacket packet = LeaveWorldPacket.STATIC_PACKET;
					Disconnection.of(_actor).defaultSequence(ref packet);
					break;
				}
				case IllegalActionPunishmentType.KICKBAN:
				{
					PunishmentManager.getInstance().startPunishment(new PunishmentTask(_actor.ObjectId.ToString(),
						PunishmentAffect.CHARACTER, PunishmentType.BAN,
						DateTime.UtcNow + TimeSpan.FromSeconds(Config.DEFAULT_PUNISH_PARAM),
						_message, GetType().Name));
					break;
				}
				case IllegalActionPunishmentType.JAIL:
				{
					PunishmentManager.getInstance().startPunishment(new PunishmentTask(_actor.ObjectId.ToString(),
						PunishmentAffect.CHARACTER, PunishmentType.JAIL,
						DateTime.UtcNow + TimeSpan.FromSeconds(Config.DEFAULT_PUNISH_PARAM), _message,
						GetType().Name));
					break;
				}
			}
		}
	}
}
using System.Globalization;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Events.Impl.Attackables;
using L2Dn.GameServer.Model.Events.Impl.Npcs;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class FriendlyNpc : Attackable
{
	private bool _isAutoAttackable = true;

	public FriendlyNpc(NpcTemplate template): base(template)
	{
		InstanceType = InstanceType.FriendlyNpc;
		setCanReturnToSpawnPoint(false);
	}

	public override bool isAttackable()
	{
		return false;
	}

	public override bool isAutoAttackable(Creature attacker)
	{
		return _isAutoAttackable && !attacker.isPlayable() && !(attacker is FriendlyNpc);
	}

	public override void setAutoAttackable(bool value)
	{
		_isAutoAttackable = value;
	}

	public override void addDamage(Creature attacker, int damage, Skill? skill)
	{
		if (!attacker.isPlayable() && !(attacker is FriendlyNpc))
		{
			base.addDamage(attacker, damage, skill);
		}

		Events.Notify(new OnAttackableAttack(null, this, damage, skill, false));
	}

	public override void addDamageHate(Creature attacker, long damage, long aggro)
	{
		if (!attacker.isPlayable() && !(attacker is FriendlyNpc))
		{
			base.addDamageHate(attacker, damage, aggro);
		}
	}

	public override bool doDie(Creature? killer)
	{
		// Kill the Npc (the corpse disappeared after 7 seconds)
		if (!base.doDie(killer))
		{
			return false;
		}

		// Notify to scripts.
		if (killer != null)
		{
			Events.Notify(new OnAttackableKill(null, this, false));
		}

		return true;
	}

	public override void onAction(Player player, bool interact)
	{
		if (!canTarget(player))
		{
			return;
		}

		// Check if the Player already target the GuardInstance
		if (ObjectId != player.getTargetId())
		{
			// Set the target of the Player player
			player.setTarget(this);
		}
		else if (interact)
		{
			// Calculate the distance between the Player and the Npc
			if (!canInteract(player))
			{
				// Set the Player Intention to AI_INTENTION_INTERACT
				player.getAI().setIntention(CtrlIntention.AI_INTENTION_INTERACT, this);
			}
			else
			{
				player.setLastFolkNPC(this);

				// Open a chat window on client with the text of the GuardInstance
				if (Events.HasSubscribers<OnNpcQuestStart>())
				{
					player.setLastQuestNpcObject(ObjectId);
				}

				if (Events.HasSubscribers<OnNpcFirstTalk>())
				{
					Events.NotifyAsync(new OnNpcFirstTalk(this, player));
				}
				else
				{
					showChatWindow(player, 0);
				}
			}
		}
		// Send a Server->Client ActionFailed to the Player in order to avoid that the client wait another packet
		player.sendPacket(ActionFailedPacket.STATIC_PACKET);
	}

	public override string getHtmlPath(int npcId, int value, Player? player)
	{
		string pom = "";
		if (value == 0)
		{
			pom = npcId.ToString(CultureInfo.InvariantCulture);
		}
		else
		{
			pom = npcId + "-" + value;
		}
		return "html/default/" + pom + ".htm";
	}

	protected override CreatureAI initAI()
	{
		return new FriendlyNpcAI(this);
	}
}
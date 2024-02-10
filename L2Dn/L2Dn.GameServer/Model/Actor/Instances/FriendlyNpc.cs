using System.Globalization;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Events;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class FriendlyNpc : Attackable
{
	private bool _isAutoAttackable = true;
	
	public FriendlyNpc(NpcTemplate template): base(template)
	{
		setInstanceType(InstanceType.FriendlyNpc);
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
	
	public override void addDamage(Creature attacker, int damage, Skill skill)
	{
		if (!attacker.isPlayable() && !(attacker is FriendlyNpc))
		{
			base.addDamage(attacker, damage, skill);
		}
		
		if (attacker.isAttackable() && EventDispatcher.getInstance().hasListener(EventType.ON_ATTACKABLE_ATTACK, this))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnAttackableAttack(null, this, damage, skill, false), this);
		}
	}
	
	public override void addDamageHate(Creature attacker, long damage, long aggro)
	{
		if (!attacker.isPlayable() && !(attacker is FriendlyNpc))
		{
			base.addDamageHate(attacker, damage, aggro);
		}
	}
	
	public override bool doDie(Creature killer)
	{
		// Kill the Npc (the corpse disappeared after 7 seconds)
		if (!base.doDie(killer))
		{
			return false;
		}
		
		// Notify to scripts.
		if ((killer != null) && killer.isAttackable() && EventDispatcher.getInstance().hasListener(EventType.ON_ATTACKABLE_KILL, this))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnAttackableKill(null, this, false), this);
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
		if (getObjectId() != player.getTargetId())
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
				if (hasListener(EventType.ON_NPC_QUEST_START))
				{
					player.setLastQuestNpcObject(getObjectId());
				}
				
				if (hasListener(EventType.ON_NPC_FIRST_TALK))
				{
					EventDispatcher.getInstance().notifyEventAsync(new OnNpcFirstTalk(this, player), this);
				}
				else
				{
					showChatWindow(player, 0);
				}
			}
		}
		// Send a Server->Client ActionFailed to the Player in order to avoid that the client wait another packet
		player.sendPacket(ActionFailed.STATIC_PACKET);
	}
	
	public override String getHtmlPath(int npcId, int value, Player player)
	{
		String pom = "";
		if (value == 0)
		{
			pom = npcId.ToString(CultureInfo.InvariantCulture);
		}
		else
		{
			pom = npcId + "-" + value;
		}
		return "data/html/default/" + pom + ".htm";
	}
	
	protected override CreatureAI initAI()
	{
		return new FriendlyNpcAI(this);
	}
}

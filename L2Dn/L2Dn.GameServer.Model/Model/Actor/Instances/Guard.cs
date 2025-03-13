using System.Globalization;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Events.Impl.Npcs;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Model.Actor.Instances;

/**
 * This class manages all Guards in the world. It inherits all methods from Attackable and adds some more such as tracking PK and aggressive Monster.
 */
public class Guard: Attackable
{
	/**
	 * Constructor of GuardInstance (use Creature and Npc constructor).<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Call the Creature constructor to set the _template of the GuardInstance (copy skills from template to object and link _calculators to NPC_STD_CALCULATOR)</li>
	 * <li>Set the name of the GuardInstance</li>
	 * <li>Create a RandomAnimation Task that will be launched after the calculated delay if the server allow it</li>
	 * </ul>
	 * @param template to apply to the NPC
	 */
	public Guard(NpcTemplate template): base(template)
	{
		InstanceType = InstanceType.Guard;
	}

	public override bool isAutoAttackable(Creature attacker)
	{
		if (attacker.isMonster() && !attacker.isFakePlayer())
		{
			return true;
		}

        Player? player = attacker.getActingPlayer();
		if (Config.FACTION_SYSTEM_ENABLED && Config.FACTION_GUARDS_ENABLED && attacker.isPlayable() && player != null)
		{
			if ((player.isGood() && getTemplate().isClan(Config.FACTION_EVIL_TEAM_NAME)) ||
			    (player.isEvil() && getTemplate().isClan(Config.FACTION_GOOD_TEAM_NAME)))
			{
				return true;
			}
		}

		return base.isAutoAttackable(attacker);
	}

	public override void addDamage(Creature attacker, int damage, Skill? skill)
	{
		base.addDamage(attacker, damage, skill);
		getAI().startFollow(attacker);
		addDamageHate(attacker, 0, 10);
		World.getInstance().forEachVisibleObjectInRange<Guard>(this, 500, guard =>
		{
			guard.getAI().startFollow(attacker);
			guard.addDamageHate(attacker, 0, 10);
		});
	}

	/**
	 * Set the home location of its GuardInstance.
	 */
	public override void onSpawn()
	{
		base.onSpawn();
		setRandomWalking(getTemplate().isRandomWalkEnabled());
		getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
		// check the region where this mob is, do not activate the AI if region is inactive.
		// final WorldRegion region = World.getInstance().getRegion(this);
		// if ((region != null) && (!region.isActive()))
		// {
		// getAI().stopAITask();
		// }
	}

	/**
	 * Return the pathfile of the selected HTML file in function of the GuardInstance Identifier and of the page number.<br>
	 * <br>
	 * <b><u>Format of the pathfile</u>:</b>
	 * <ul>
	 * <li>if page number = 0 : <b>data/html/guard/12006.htm</b> (npcId-page number)</li>
	 * <li>if page number > 0 : <b>data/html/guard/12006-1.htm</b> (npcId-page number)</li>
	 * </ul>
	 * @param npcId The Identifier of the Npc whose text must be display
	 * @param value The number of the page to display
	 */
	public override string getHtmlPath(int npcId, int value, Player? player)
	{
		string pom;
		if (value == 0)
		{
			pom = npcId.ToString(CultureInfo.InvariantCulture);
		}
		else
		{
			pom = npcId + "-" + value;
		}

		return "html/guard/" + pom + ".htm";
	}

	/**
	 * Manage actions when a player click on the GuardInstance.<br>
	 * <br>
	 * <b><u>Actions on first click on the GuardInstance (Select it)</u>:</b>
	 * <ul>
	 * <li>Set the GuardInstance as target of the Player player (if necessary)</li>
	 * <li>Send a Server->Client packet MyTargetSelected to the Player player (display the select window)</li>
	 * <li>Set the Player Intention to AI_INTENTION_IDLE</li>
	 * <li>Send a Server->Client packet ValidateLocation to correct the GuardInstance position and heading on the client</li>
	 * </ul>
	 * <br>
	 * <b><u>Actions on second click on the GuardInstance (Attack it/Interact with it)</u>:</b>
	 * <ul>
	 * <li>If Player is in the _aggroList of the GuardInstance, set the Player Intention to AI_INTENTION_ATTACK</li>
	 * <li>If Player is NOT in the _aggroList of the GuardInstance, set the Player Intention to AI_INTENTION_INTERACT (after a distance verification) and show message</li>
	 * </ul>
	 * <br>
	 * <b><u>Example of use</u>:</b>
	 * <ul>
	 * <li>Client packet : Action, AttackRequest</li>
	 * </ul>
	 * @param player The Player that start an action on the GuardInstance
	 */
	public override void onAction(Player player, bool interactValue)
	{
		if (!canTarget(player))
		{
			return;
		}

		bool interact = interactValue;
		if (Config.FACTION_SYSTEM_ENABLED && Config.FACTION_GUARDS_ENABLED &&
		    ((player.isGood() && getTemplate().isClan(Config.FACTION_EVIL_TEAM_NAME)) ||
		     (player.isEvil() && getTemplate().isClan(Config.FACTION_GOOD_TEAM_NAME))))
		{
			interact = false;
			// TODO: Fix normal targeting
			player.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, this);
		}

		if (isFakePlayer() && isInCombat())
		{
			interact = false;
			// TODO: Fix normal targeting
			player.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, this);
		}

		// Check if the Player already target the GuardInstance
		if (ObjectId != player.getTargetId())
		{
			// Set the target of the Player player
			player.setTarget(this);
		}
		else if (interact)
		{
			// Check if the Player is in the _aggroList of the GuardInstance
			if (isInAggroList(player))
			{
				// Set the Player Intention to AI_INTENTION_ATTACK
				player.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, this);
			}
			else
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
		}

		// Send a Server->Client ActionFailed to the Player in order to avoid that the client wait another packet
		player.sendPacket(ActionFailedPacket.STATIC_PACKET);
	}
}
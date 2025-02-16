using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Take Fort effect implementation.
 * @author Adry_85
 */
public class TakeFort: AbstractEffect
{
	public TakeFort(StatSet @params)
	{
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (!effector.isPlayer())
		{
			return;
		}
		
		Fort fort = FortManager.getInstance().getFort(effector);
		if ((fort != null) && (fort.getResidenceId() == FortManager.ORC_FORTRESS))
		{
			if (fort.getSiege().isInProgress())
			{
				fort.endOfSiege(effector.getClan());
				if (effector.isPlayer())
				{
					Player player = effector.getActingPlayer();
					FortSiegeManager.getInstance().dropCombatFlag(player, FortManager.ORC_FORTRESS);
					
					Message mail = new Message(player.ObjectId, "Orc Fortress", "", MailType.NPC);
					Mail attachment = mail.createAttachments();
					attachment.addItem("Orc Fortress", Inventory.ADENA_ID, 30_000_000, player, player);
					MailManager.getInstance().sendMessage(mail);
				}
			}
		}
	}
}
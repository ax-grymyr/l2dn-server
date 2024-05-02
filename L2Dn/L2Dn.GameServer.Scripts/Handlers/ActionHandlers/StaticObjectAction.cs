using L2Dn.GameServer.AI;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Geometry;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.ActionHandlers;

public class StaticObjectAction: IActionHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(StaticObjectAction));
	
	public bool action(Player player, WorldObject target, bool interact)
	{
		StaticObject staticObject = (StaticObject) target;
		if (staticObject.getType() < 0)
		{
			_logger.Info("StaticObject: StaticObject with invalid type! StaticObjectId: " + staticObject.getId());
		}
		
		// Check if the Player already target the Npc
		if (player.getTarget() != staticObject)
		{
			// Set the target of the Player player
			player.setTarget(staticObject);
		}
		else if (interact)
		{
			// Calculate the distance between the Player and the Npc
			if (!player.IsInsideRadius2D(staticObject, Npc.INTERACTION_DISTANCE))
			{
				// Notify the Player AI with AI_INTENTION_INTERACT
				player.getAI().setIntention(CtrlIntention.AI_INTENTION_INTERACT, staticObject);
			}
			else if (staticObject.getType() == 2)
			{
				string filename = (staticObject.getId() == 24230101)
					? "html/signboard/tomb_of_crystalgolem.htm"
					: "html/signboard/pvp_signboard.htm";
				
				HtmlContent html = HtmlContent.LoadFromFile(filename, player);
				NpcHtmlMessagePacket htmlPacket = new NpcHtmlMessagePacket(staticObject.getObjectId(), 0, html);
				player.sendPacket(htmlPacket);
			}
			else if (staticObject.getType() == 0)
			{
				player.sendPacket(staticObject.getMap());
			}
		}
		
		return true;
	}
	
	public InstanceType getInstanceType()
	{
		return InstanceType.StaticObject;
	}
}
using System.Text;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * Petition Manager
 * @author Tempy
 */
public class PetitionManager
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(PetitionManager));
	
	private readonly Map<int, Petition> _pendingPetitions;
	private readonly Map<int, Petition> _completedPetitions;
	
	protected PetitionManager()
	{
		_pendingPetitions = new();
		_completedPetitions = new();
	}
	
	public void clearCompletedPetitions()
	{
		int numPetitions = _pendingPetitions.Count;
		_completedPetitions.Clear();
		LOGGER.Info(GetType().Name +": Completed petition data cleared. " + numPetitions + " petitions removed.");
	}
	
	public void clearPendingPetitions()
	{
		int numPetitions = _pendingPetitions.Count;
		_pendingPetitions.Clear();
		LOGGER.Info(GetType().Name +": Pending petition queue cleared. " + numPetitions + " petitions removed.");
	}
	
	public bool acceptPetition(Player respondingAdmin, int petitionId)
	{
		if (!isValidPetition(petitionId))
		{
			return false;
		}
		
		Petition currPetition = _pendingPetitions.get(petitionId);
		if (currPetition.getResponder() != null)
		{
			return false;
		}
		
		currPetition.setResponder(respondingAdmin);
		currPetition.setState(PetitionState.IN_PROCESS);
		
		// Petition application accepted. (Send to Petitioner)
		currPetition.sendPetitionerPacket(new SystemMessagePacket(SystemMessageId.YOUR_GLOBAL_SUPPORT_REQUEST_WAS_RECEIVED_2));
		
		// Petition application accepted. Reciept No. is <ID>
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOUR_GLOBAL_SUPPORT_REQUEST_WAS_RECEIVED_REQUEST_NO_S1);
		sm.Params.addInt(currPetition.getId());
		currPetition.sendResponderPacket(sm);
		
		// Petition consultation with <Player> underway.
		sm = new SystemMessagePacket(SystemMessageId.A_GLOBAL_SUPPORT_CONSULTATION_C1_HAS_BEEN_STARTED);
		sm.Params.addString(currPetition.getPetitioner().getName());
		currPetition.sendResponderPacket(sm);
		
		// Set responder name on petitioner instance
		currPetition.getPetitioner().setLastPetitionGmName(currPetition.getResponder().getName());
		return true;
	}
	
	public bool cancelActivePetition(Player player)
	{
		foreach (Petition currPetition in _pendingPetitions.Values)
		{
			if ((currPetition.getPetitioner() != null) && (currPetition.getPetitioner().getObjectId() == player.getObjectId()))
			{
				return (currPetition.endPetitionConsultation(PetitionState.PETITIONER_CANCEL));
			}
			
			if ((currPetition.getResponder() != null) && (currPetition.getResponder().getObjectId() == player.getObjectId()))
			{
				return (currPetition.endPetitionConsultation(PetitionState.RESPONDER_CANCEL));
			}
		}
		
		return false;
	}
	
	public void checkPetitionMessages(Player petitioner)
	{
		if (petitioner != null)
		{
			foreach (Petition currPetition in _pendingPetitions.Values)
			{
				if (currPetition == null)
				{
					continue;
				}
				
				if ((currPetition.getPetitioner() != null) && (currPetition.getPetitioner().getObjectId() == petitioner.getObjectId()))
				{
					foreach (CreatureSayPacket logMessage in currPetition.getLogMessages())
					{
						petitioner.sendPacket(logMessage);
					}
					
					return;
				}
			}
		}
	}
	
	public bool endActivePetition(Player player)
	{
		if (!player.isGM())
		{
			return false;
		}
		
		foreach (Petition currPetition in _pendingPetitions.Values)
		{
			if (currPetition == null)
			{
				continue;
			}
			
			if ((currPetition.getResponder() != null) && (currPetition.getResponder().getObjectId() == player.getObjectId()))
			{
				return (currPetition.endPetitionConsultation(PetitionState.COMPLETED));
			}
		}
		
		return false;
	}
	
	public Map<int, Petition> getCompletedPetitions()
	{
		return _completedPetitions;
	}
	
	public Map<int, Petition> getPendingPetitions()
	{
		return _pendingPetitions;
	}
	
	public int getPendingPetitionCount()
	{
		return _pendingPetitions.Count;
	}
	
	public int getPlayerTotalPetitionCount(Player player)
	{
		if (player == null)
		{
			return 0;
		}
		
		int petitionCount = 0;
		foreach (Petition currPetition in _pendingPetitions.Values)
		{
			if (currPetition == null)
			{
				continue;
			}
			
			if ((currPetition.getPetitioner() != null) && (currPetition.getPetitioner().getObjectId() == player.getObjectId()))
			{
				petitionCount++;
			}
		}
		
		foreach (Petition currPetition in _completedPetitions.Values)
		{
			if (currPetition == null)
			{
				continue;
			}
			
			if ((currPetition.getPetitioner() != null) && (currPetition.getPetitioner().getObjectId() == player.getObjectId()))
			{
				petitionCount++;
			}
		}
		
		return petitionCount;
	}
	
	public bool isPetitionInProcess()
	{
		foreach (Petition currPetition in _pendingPetitions.Values)
		{
			if (currPetition == null)
			{
				continue;
			}
			
			if (currPetition.getState() == PetitionState.IN_PROCESS)
			{
				return true;
			}
		}
		
		return false;
	}
	
	public bool isPetitionInProcess(int petitionId)
	{
		if (!isValidPetition(petitionId))
		{
			return false;
		}
		
		Petition currPetition = _pendingPetitions.get(petitionId);
		return (currPetition.getState() == PetitionState.IN_PROCESS);
	}
	
	public bool isPlayerInConsultation(Player player)
	{
		if (player != null)
		{
			foreach (Petition currPetition in _pendingPetitions.Values)
			{
				if (currPetition == null)
				{
					continue;
				}
				
				if (currPetition.getState() != PetitionState.IN_PROCESS)
				{
					continue;
				}
				
				if (((currPetition.getPetitioner() != null) && (currPetition.getPetitioner().getObjectId() == player.getObjectId())) || ((currPetition.getResponder() != null) && (currPetition.getResponder().getObjectId() == player.getObjectId())))
				{
					return true;
				}
			}
		}
		
		return false;
	}
	
	public bool isPetitioningAllowed()
	{
		return Config.PETITIONING_ALLOWED;
	}
	
	public bool isPlayerPetitionPending(Player petitioner)
	{
		if (petitioner != null)
		{
			foreach (Petition currPetition in _pendingPetitions.Values)
			{
				if (currPetition == null)
				{
					continue;
				}
				
				if ((currPetition.getPetitioner() != null) && (currPetition.getPetitioner().getObjectId() == petitioner.getObjectId()))
				{
					return true;
				}
			}
		}
		
		return false;
	}
	
	private bool isValidPetition(int petitionId)
	{
		return _pendingPetitions.ContainsKey(petitionId);
	}
	
	public bool rejectPetition(Player respondingAdmin, int petitionId)
	{
		if (!_pendingPetitions.TryGetValue(petitionId, out Petition? currPetition))
		{
			return false;
		}
		
		if (currPetition.getResponder() != null)
		{
			return false;
		}
		
		currPetition.setResponder(respondingAdmin);
		return (currPetition.endPetitionConsultation(PetitionState.RESPONDER_REJECT));
	}
	
	public bool sendActivePetitionMessage(Player player, string messageText)
	{
		// if (!isPlayerInConsultation(player))
		// return false;
		CreatureSayPacket cs;
		foreach (Petition currPetition in _pendingPetitions.Values)
		{
			if (currPetition == null)
			{
				continue;
			}
			
			if ((currPetition.getPetitioner() != null) && (currPetition.getPetitioner().getObjectId() == player.getObjectId()))
			{
				cs = new CreatureSayPacket(player, ChatType.PETITION_PLAYER, player.getName(), messageText);
				currPetition.addLogMessage(cs);
				
				currPetition.sendResponderPacket(cs);
				currPetition.sendPetitionerPacket(cs);
				return true;
			}
			
			if ((currPetition.getResponder() != null) && (currPetition.getResponder().getObjectId() == player.getObjectId()))
			{
				cs = new CreatureSayPacket(player, ChatType.PETITION_GM, player.getName(), messageText);
				currPetition.addLogMessage(cs);
				
				currPetition.sendResponderPacket(cs);
				currPetition.sendPetitionerPacket(cs);
				return true;
			}
		}
		
		return false;
	}

	public void sendPendingPetitionList(Player player)
	{
		StringBuilder content = new StringBuilder(600 + (_pendingPetitions.Count * 300));
		content.Append(
			"<html><body><center><table width=270><tr><td width=45><button value=\"Main\" action=\"bypass " +
			"admin_admin\" width=45 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\">" +
			"</td><td width=180><center>Petition Menu</center></td><td width=45><button value=\"Back\" " +
			"action=\"bypass -h admin_admin7\" width=45 height=21 back=\"L2UI_ct1.button_df\" " +
			"fore=\"L2UI_ct1.button_df\"></td></tr></table><br><table width=\"270\"><tr><td>" +
			"<table width=\"270\"><tr><td><button value=\"Reset\" action=\"bypass -h admin_reset_petitions\" " +
			"width=\"80\" height=\"21\" back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td>" +
			"<td align=right><button value=\"Refresh\" action=\"bypass -h admin_view_petitions\" width=\"80\" " +
			"height=\"21\" back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr></table><br></td></tr>");

		if (_pendingPetitions.Count == 0)
		{
			content.Append("<tr><td>There are no currently pending petitions.</td></tr>");
		}
		else
		{
			content.Append("<tr><td><font color=\"LEVEL\">Current Petitions:</font><br></td></tr>");
		}

		bool color = true;
		int petcount = 0;
		foreach (Petition currPetition in _pendingPetitions.Values)
		{
			if (currPetition == null)
			{
				continue;
			}

			content.Append("<tr><td width=\"270\"><table width=\"270\" cellpadding=\"2\" bgcolor=" +
			                   (color ? "131210" : "444444") + "><tr><td width=\"130\">" +
			                   currPetition.getSubmitTime().ToString("yyyy-MM-dd HH:mm"));
			content.Append("</td><td width=\"140\" align=right><font color=\"" +
			                   (currPetition.getPetitioner().isOnline() ? "00FF00" : "999999") + "\">" +
			                   currPetition.getPetitioner().getName() + "</font></td></tr>");
			content.Append("<tr><td width=\"130\">");
			if (currPetition.getState() != PetitionState.IN_PROCESS)
			{
				content.Append(
					"<table width=\"130\" cellpadding=\"2\"><tr><td><button value=\"View\" action=\"bypass -h " +
					"admin_view_petition " + currPetition.getId() +
					"\" width=\"50\" height=\"21\" back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td><td>" +
					"<button value=\"Reject\" action=\"bypass -h admin_reject_petition " + currPetition.getId() +
					"\" width=\"50\" height=\"21\" back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\">" +
					"</td></tr></table>");
			}
			else
			{
				content.Append("<font color=\"" + (currPetition.getResponder().isOnline() ? "00FF00" : "999999") +
				                   "\">" + currPetition.getResponder().getName() + "</font>");
			}

			content.Append("</td>" + currPetition.getTypeAsString() + "<td width=\"140\" align=right>" +
			                   currPetition.getTypeAsString() + "</td></tr></table></td></tr>");
			color = !color;
			petcount++;
			if (petcount > 10)
			{
				content.Append(
					"<tr><td><font color=\"LEVEL\">There is more pending petition...</font><br></td></tr>");
				break;
			}
		}

		content.Append("</table></center></body></html>");

		HtmlContent htmlContent = HtmlContent.LoadFromText(content.ToString(), player);
		NpcHtmlMessagePacket htmlMsg = new NpcHtmlMessagePacket(null, 0, htmlContent);
		player.sendPacket(htmlMsg);
	}

	public int submitPetition(Player petitioner, string petitionText, int petitionType)
	{
		// Create a new petition instance and add it to the list of pending petitions.
		Petition newPetition = new Petition(petitioner, petitionText, petitionType);
		int newPetitionId = newPetition.getId();
		_pendingPetitions.put(newPetitionId, newPetition);
		
		// Notify all GMs that a new petition has been submitted.
		string msgContent = petitioner.getName() + " has submitted a new petition."; // (ID: " + newPetitionId + ").";
		AdminData.getInstance().broadcastToGMs(new CreatureSayPacket(petitioner, ChatType.HERO_VOICE, "Petition System", msgContent));
		return newPetitionId;
	}
	
	public void viewPetition(Player player, int petitionId)
	{
		if (!player.isGM())
		{
			return;
		}
		
		if (!isValidPetition(petitionId))
		{
			return;
		}
		
		Petition currPetition = _pendingPetitions.get(petitionId);

		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/petition.htm", player);
		htmlContent.Replace("%petition%", currPetition.getId().ToString());
		htmlContent.Replace("%time%", currPetition.getSubmitTime().ToString("yyyy-MM-dd HH:mm"));
		htmlContent.Replace("%type%", currPetition.getTypeAsString());
		htmlContent.Replace("%petitioner%", currPetition.getPetitioner().getName());
		htmlContent.Replace("%online%", currPetition.getPetitioner().isOnline() ? "00FF00" : "999999");
		htmlContent.Replace("%text%", currPetition.getContent());
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 0, htmlContent);
		player.sendPacket(html);
	}
	
	/**
	 * Gets the single instance of {@code PetitionManager}.
	 * @return single instance of {@code PetitionManager}
	 */
	public static PetitionManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly PetitionManager INSTANCE = new();
	}
}
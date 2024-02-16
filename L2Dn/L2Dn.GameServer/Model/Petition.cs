using System.Collections.ObjectModel;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Model;

/**
 * Petition
 * @author xban1x
 */
public class Petition
{
	private readonly DateTime _submitTime = DateTime.UtcNow;
	private readonly  int _id;
	private readonly  PetitionType _type;
	private PetitionState _state = PetitionState.PENDING;
	private readonly  String _content;
	private readonly  Set<CreatureSayPacket> _messageLog = new();
	private readonly  Player _petitioner;
	private Player _responder;
	
	public Petition(Player petitioner, String petitionText, int petitionType)
	{
		_id = IdManager.getInstance().getNextId();
		_type = Enum.GetValues<PetitionType>()[petitionType - 1];
		_content = petitionText;
		_petitioner = petitioner;
	}
	
	public bool addLogMessage(CreatureSayPacket cs)
	{
		return _messageLog.add(cs);
	}
	
	public ICollection<CreatureSayPacket> getLogMessages()
	{
		return _messageLog;
	}
	
	public bool endPetitionConsultation(PetitionState endState)
	{
		setState(endState);
		
		if ((_responder != null) && _responder.isOnline())
		{
			if (endState == PetitionState.RESPONDER_REJECT)
			{
				_petitioner.sendMessage("Your petition was rejected. Please try again later.");
			}
			else
			{
				// Ending petition consultation with <Player>.
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.A_GLOBAL_SUPPORT_CONSULTATION_C1_HAS_BEEN_FINISHED);
				sm.Params.addString(_petitioner.getName());
				_responder.sendPacket(sm);
				
				if (endState == PetitionState.PETITIONER_CANCEL)
				{
					// Receipt No. <ID> petition cancelled.
					sm = new SystemMessagePacket(SystemMessageId.REQUEST_NO_S1_TO_THE_GLOBAL_SUPPORT_WAS_CANCELLED);
					sm.Params.addInt(_id);
					_responder.sendPacket(sm);
				}
			}
		}
		
		// End petition consultation and inform them, if they are still online. And if petitioner is online, enable Evaluation button
		if ((_petitioner != null) && _petitioner.isOnline())
		{
			_petitioner.sendPacket(SystemMessageId.GLOBAL_SUPPORT_HAS_ALREADY_RESPONDED_TO_YOUR_REQUEST_PLEASE_GIVE_US_FEEDBACK_ON_THE_SERVICE_QUALITY);
			_petitioner.sendPacket(PetitionVotePacket.STATIC_PACKET);
		}
		
		PetitionManager.getInstance().getCompletedPetitions().put(getId(), this);
		return PetitionManager.getInstance().getPendingPetitions().remove(getId()) != null;
	}
	
	public String getContent()
	{
		return _content;
	}
	
	public int getId()
	{
		return _id;
	}
	
	public Player getPetitioner()
	{
		return _petitioner;
	}
	
	public Player getResponder()
	{
		return _responder;
	}
	
	public DateTime getSubmitTime()
	{
		return _submitTime;
	}
	
	public PetitionState getState()
	{
		return _state;
	}
	
	public String getTypeAsString()
	{
		return _type.ToString().Replace("_", " ");
	}
	
	public void sendPetitionerPacket<TPacket>(TPacket responsePacket)
		where TPacket: IOutgoingPacket
	{
		if ((_petitioner == null) || !_petitioner.isOnline())
		{
			// Allows petitioners to see the results of their petition when
			// they log back into the game.
			
			// endPetitionConsultation(PetitionState.Petitioner_Missing);
			return;
		}
		
		_petitioner.sendPacket(responsePacket);
	}
	
	public void sendResponderPacket<TPacket>(TPacket responsePacket)
	where TPacket: IOutgoingPacket
	{
		if ((_responder == null) || !_responder.isOnline())
		{
			endPetitionConsultation(PetitionState.RESPONDER_MISSING);
			return;
		}
		
		_responder.sendPacket(responsePacket);
	}
	
	public void setState(PetitionState state)
	{
		_state = state;
	}
	
	public void setResponder(Player respondingAdmin)
	{
		if (_responder != null)
		{
			return;
		}
		
		_responder = respondingAdmin;
	}
}
using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Model;

/**
 * This class manages requests (transactions) between two Player.
 * @author kriau
 */
public class Request
{
	private const int REQUEST_TIMEOUT = 15; // in secs
	
	protected Player _player;
	protected Player _partner;
	protected bool _isRequestor;
	protected bool _isAnswerer;
	protected ClientPacket _requestPacket;
	
	public Request(Player player)
	{
		_player = player;
	}
	
	protected void clear()
	{
		_partner = null;
		_requestPacket = null;
		_isRequestor = false;
		_isAnswerer = false;
	}
	
	/**
	 * Set the Player member of a transaction (ex : FriendInvite, JoinAlly, JoinParty...).
	 * @param partner
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	private void setPartner(Player partner)
	{
		_partner = partner;
	}
	
	/**
	 * @return the Player member of a transaction (ex : FriendInvite, JoinAlly, JoinParty...).
	 */
	public Player getPartner()
	{
		return _partner;
	}
	
	/**
	 * Set the packet that came from requester.
	 * @param packet
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	private void setRequestPacket(ClientPacket packet)
	{
		_requestPacket = packet;
	}
	
	/**
	 * Return the packet originally the came from requester.
	 * @return
	 */
	public ClientPacket getRequestPacket()
	{
		return _requestPacket;
	}
	
	/**
	 * Checks if request can be made and in success case puts both PC on request state.
	 * @param partner
	 * @param packet
	 * @return
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public bool setRequest(Player partner, ClientPacket packet)
	{
		if (partner == null)
		{
			_player.sendPacket(SystemMessageId.THE_TARGET_CANNOT_BE_INVITED);
			return false;
		}
		if (partner.getRequest().isProcessingRequest())
		{
			SystemMessage sm = new SystemMessage(SystemMessageId.C1_IS_ON_ANOTHER_TASK_PLEASE_TRY_AGAIN_LATER);
			sm.addString(partner.getName());
			_player.sendPacket(sm);
			return false;
		}
		if (isProcessingRequest())
		{
			_player.sendPacket(SystemMessageId.WAITING_FOR_ANOTHER_REPLY);
			return false;
		}
		
		_partner = partner;
		_requestPacket = packet;
		setOnRequestTimer(true);
		_partner.getRequest().setPartner(_player);
		_partner.getRequest().setRequestPacket(packet);
		_partner.getRequest().setOnRequestTimer(false);
		return true;
	}
	
	private void setOnRequestTimer(bool isRequestor)
	{
		_isRequestor = isRequestor;
		_isAnswerer = !isRequestor;
		ThreadPool.schedule(this::clear, REQUEST_TIMEOUT * 1000);
	}
	
	/**
	 * Clears PC request state. Should be called after answer packet receive.
	 */
	public void onRequestResponse()
	{
		if (_partner != null)
		{
			_partner.getRequest().clear();
		}
		clear();
	}
	
	/**
	 * @return {@code true} if a transaction is in progress.
	 */
	public bool isProcessingRequest()
	{
		return _partner != null;
	}
}
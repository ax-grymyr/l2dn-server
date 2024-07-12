using System.Globalization;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Punishment;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.VoicedCommandHandlers;

public class ChatAdmin: IVoicedCommandHandler
{
	private static readonly string[] VOICED_COMMANDS =
	{
		"banchat",
		"chatban",
		"unbanchat",
		"chatunban"
	};
	
	public bool useVoicedCommand(string command, Player activeChar, string @params)
	{
		if (!AdminData.getInstance().hasAccess(command, activeChar.getAccessLevel()))
		{
			return false;
		}
		
		switch (command)
		{
			case "banchat":
			case "chatban":
			{
				if (@params == null)
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: .banchat name [minutes]");
					return true;
				}
				
                StringTokenizer st = new StringTokenizer(@params);
				if (st.hasMoreTokens())
				{
					string name = st.nextToken();
					long expirationTime = 0;
					if (st.hasMoreTokens())
					{
						string token = st.nextToken();
						if (int.TryParse(token, CultureInfo.InvariantCulture, out int value))
						{
							expirationTime = value;
						}
					}
					
					int objId = CharInfoTable.getInstance().getIdByName(name);
					if (objId > 0)
					{
						Player player = World.getInstance().getPlayer(objId);
						if ((player == null) || !player.isOnline())
						{
							BuilderUtil.sendSysMessage(activeChar, "Player not online!");
							return false;
						}
						if (player.isChatBanned())
						{
							BuilderUtil.sendSysMessage(activeChar, "Player is already punished!");
							return false;
						}
						if (player == activeChar)
						{
							BuilderUtil.sendSysMessage(activeChar, "You can't ban yourself!");
							return false;
						}
						if (player.isGM())
						{
							BuilderUtil.sendSysMessage(activeChar, "You can't ban a GM!");
							return false;
						}
						if (AdminData.getInstance().hasAccess(command, player.getAccessLevel()))
						{
							BuilderUtil.sendSysMessage(activeChar, "You can't ban moderator!");
							return false;
						}

                        PunishmentManager.getInstance().startPunishment(new PunishmentTask(objId.ToString(),
                            PunishmentAffect.CHARACTER, PunishmentType.CHAT_BAN,
                            DateTime.UtcNow.AddHours(expirationTime), "Chat banned by moderator",
                            activeChar.getName()));
                        
						if (expirationTime > 0)
						{
							BuilderUtil.sendSysMessage(activeChar, "Player " + player.getName() + " chat banned for " + expirationTime + " minutes.");
						}
						else
						{
							BuilderUtil.sendSysMessage(activeChar, "Player " + player.getName() + " chat banned forever.");
						}
						player.sendMessage("Chat banned by moderator " + activeChar.getName());
						player.getEffectList().startAbnormalVisualEffect(AbnormalVisualEffect.NO_CHAT);
					}
					else
					{
						BuilderUtil.sendSysMessage(activeChar, "Player not found!");
						return false;
					}
				}
				break;
			}
			case "unbanchat":
			case "chatunban":
			{
				if (@params == null)
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: .unbanchat name");
					return true;
				}
				
                StringTokenizer st = new StringTokenizer(@params);
				if (st.hasMoreTokens())
				{
					string name = st.nextToken();
					int objId = CharInfoTable.getInstance().getIdByName(name);
					if (objId > 0)
					{
						Player player = World.getInstance().getPlayer(objId);
						if ((player == null) || !player.isOnline())
						{
							BuilderUtil.sendSysMessage(activeChar, "Player not online!");
							return false;
						}
						if (!player.isChatBanned())
						{
							BuilderUtil.sendSysMessage(activeChar, "Player is not chat banned!");
							return false;
						}
						
						PunishmentManager.getInstance().stopPunishment(objId.ToString(), PunishmentAffect.CHARACTER, PunishmentType.CHAT_BAN);
						BuilderUtil.sendSysMessage(activeChar, "Player " + player.getName() + " chat unbanned.");
						player.sendMessage("Chat unbanned by moderator " + activeChar.getName());
						player.getEffectList().stopAbnormalVisualEffect(AbnormalVisualEffect.NO_CHAT);
					}
					else
					{
						BuilderUtil.sendSysMessage(activeChar, "Player not found!");
						return false;
					}
				}
				break;
			}
		}
		return true;
	}
	
	public string[] getVoicedCommandList()
	{
		return VOICED_COMMANDS;
	}
}
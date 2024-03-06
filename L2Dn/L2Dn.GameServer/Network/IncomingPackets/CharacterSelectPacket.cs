using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Players;
using L2Dn.GameServer.Model.Events.Returns;
using L2Dn.GameServer.Model.Punishment;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct CharacterSelectPacket: IIncomingPacket<GameSession>
{
    private int _charSlot;
    private int _unk1; // new in C4
    private int _unk2;
    private int _unk3;
    private int _unk4;

    public void ReadContent(PacketBitReader reader)
    {
        _charSlot = reader.ReadInt32();
        _unk1 = reader.ReadInt16();
        _unk2 = reader.ReadInt32();
        _unk3 = reader.ReadInt32();
        _unk4 = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		// if (!client.getFloodProtectors().canSelectCharacter())
		// {
		// 	return;
		// }
		
		// if (SecondaryAuthData.getInstance().isEnabled() && !session.getSecondaryAuth().isAuthed())
		// {
		// 	client.getSecondaryAuth().openDialog();
		// 	return;
		// }
		
		// We should always be able to acquire the lock
		// But if we can't lock then nothing should be done (i.e. repeated packet)
		if (Monitor.TryEnter(session.PlayerLock))
		{
			try
			{
				// should always be null
				// but if not then this is repeated packet and nothing should be done here
				if (session.Player == null)
				{
					if (_charSlot < 0 || _charSlot >= session.Characters.Length)
						return ValueTask.CompletedTask;
					
					CharSelectInfoPackage info = session.Characters[_charSlot];
					
					// Disconnect offline trader.
					Player offlineTrader = World.getInstance().getPlayer(info.getObjectId());
					if (offlineTrader != null)
					{
						Disconnection.of(offlineTrader).storeMe().deleteMe();
					}
					
					// Banned?
					if (PunishmentManager.getInstance().hasPunishment(info.getObjectId().ToString(), PunishmentAffect.CHARACTER, PunishmentType.BAN)
						|| PunishmentManager.getInstance().hasPunishment(session.AccountName, PunishmentAffect.ACCOUNT, PunishmentType.BAN)
						|| PunishmentManager.getInstance().hasPunishment(session.IpAddress.ToString(), PunishmentAffect.IP, PunishmentType.BAN))
					{
						ServerClosePacket serverClosePacket = new();
						connection.Send(ref serverClosePacket, SendPacketOptions.CloseAfterSending);
						return ValueTask.CompletedTask;
					}
					
					// Selected character is banned (compatibility with previous versions).
					if (info.getAccessLevel() < 0)
					{
						ServerClosePacket serverClosePacket = new();
						connection.Send(ref serverClosePacket, SendPacketOptions.CloseAfterSending);
						return ValueTask.CompletedTask;
					}

					if ((Config.DUALBOX_CHECK_MAX_PLAYERS_PER_IP > 0) && !AntiFeedManager.getInstance()
						    .tryAddClient(AntiFeedManager.GAME_ID, session, Config.DUALBOX_CHECK_MAX_PLAYERS_PER_IP))
					{
						HtmlPacketHelper helper =
							new HtmlPacketHelper(DataFileLocation.Data, "html/mods/IPRestriction.htm");
						
						helper.Replace("%max%", AntiFeedManager.getInstance()
							.getLimit(session, Config.DUALBOX_CHECK_MAX_PLAYERS_PER_IP).ToString());

						NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(helper);
						connection.Send(ref msg);
						return ValueTask.CompletedTask;
					}

					if (Config.FACTION_SYSTEM_ENABLED && Config.FACTION_BALANCE_ONLINE_PLAYERS)
					{
						if (info.isGood() && (World.getInstance().getAllGoodPlayers().Count >=
						                      (World.getInstance().getAllEvilPlayers().Count +
						                       Config.FACTION_BALANCE_PLAYER_EXCEED_LIMIT)))
						{
							HtmlPacketHelper helper =
								new HtmlPacketHelper(DataFileLocation.Data, "html/mods/Faction/ExceededOnlineLimit.htm");

							helper.Replace("%more%", Config.FACTION_GOOD_TEAM_NAME);
							helper.Replace("%less%", Config.FACTION_EVIL_TEAM_NAME);

							NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(helper);
							connection.Send(ref msg);
							return ValueTask.CompletedTask;
						}

						if (info.isEvil() && (World.getInstance().getAllEvilPlayers().Count >=
						                      (World.getInstance().getAllGoodPlayers().Count +
						                       Config.FACTION_BALANCE_PLAYER_EXCEED_LIMIT)))
						{
							HtmlPacketHelper helper =
								new HtmlPacketHelper(DataFileLocation.Data, "html/mods/Faction/ExceededOnlineLimit.htm");

							helper.Replace("%more%", Config.FACTION_EVIL_TEAM_NAME);
							helper.Replace("%less%", Config.FACTION_GOOD_TEAM_NAME);

							NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(helper);
							connection.Send(ref msg);
							return ValueTask.CompletedTask;
						}
					}

					// load up character from disk
					Player player = CharacterPacketHelper.LoadPlayer(session, _charSlot);
					if (player == null)
						return ValueTask.CompletedTask; // handled in GameClient
					
					CharInfoTable.getInstance().addName(player);
					
					// Prevent instant disappear of invisible GMs on login.
					if (player.isGM() && Config.GM_STARTUP_INVISIBLE && AdminData.getInstance().hasAccess("admin_invisible", player.getAccessLevel()))
					{
						player.setInvisible(true);
					}
					
					player.setClient(session);
					session.Player = player;
					player.setOnlineStatus(true, true);
					
					if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_SELECT, Containers.Players()))
					{
						TerminateReturn terminate = EventDispatcher.getInstance().notifyEvent<TerminateReturn>(new OnPlayerSelect(player, player.getObjectId(), player.getName(), session), Containers.Players());
						if ((terminate != null) && terminate.terminate())
						{
							LeaveWorldPacket leaveWorldPacket = new();
							Disconnection.of(player).defaultSequence(ref leaveWorldPacket);
							return ValueTask.CompletedTask;
						}
					}

					session.State = GameSessionState.EnteringGame;

					CharacterSelectedPacket selectedPacket = new(player, session.PlayKey1);
					connection.Send(ref selectedPacket);
				}
			}
			finally
			{
				Monitor.Exit(session.PlayerLock);
			}
		}

        return ValueTask.CompletedTask;
    }
}

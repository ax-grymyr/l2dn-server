using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Punishment;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

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
	    if (session.Characters is null)
	    {
		    // Characters must be loaded in AuthLoginPacket
		    connection.Close();
		    return ValueTask.CompletedTask;
	    }

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
					if (_charSlot < 0 || _charSlot >= session.Characters.Count)
						return ValueTask.CompletedTask;

					CharacterInfo charInfo = session.Characters[_charSlot];

					// Disconnect offline trader.
					Player? offlineTrader = World.getInstance().getPlayer(charInfo.Id);
					if (offlineTrader != null)
					{
						Disconnection.of(offlineTrader).storeMe().deleteMe();
					}

					// Banned?
					if (PunishmentManager.getInstance().hasPunishment(charInfo.Id.ToString(), PunishmentAffect.CHARACTER, PunishmentType.BAN)
						|| PunishmentManager.getInstance().hasPunishment(session.AccountName, PunishmentAffect.ACCOUNT, PunishmentType.BAN)
						|| PunishmentManager.getInstance().hasPunishment(session.IpAddress.ToString(), PunishmentAffect.IP, PunishmentType.BAN))
					{
						ServerClosePacket serverClosePacket = new();
						connection.Send(ref serverClosePacket, SendPacketOptions.CloseAfterSending);
						return ValueTask.CompletedTask;
					}

					// Selected character is banned (compatibility with previous versions).
					if (charInfo.AccessLevel < 0)
					{
						ServerClosePacket serverClosePacket = new();
						connection.Send(ref serverClosePacket, SendPacketOptions.CloseAfterSending);
						return ValueTask.CompletedTask;
					}

					if (Config.DualboxCheck.DUALBOX_CHECK_MAX_PLAYERS_PER_IP > 0 && !AntiFeedManager.getInstance()
						    .tryAddClient(AntiFeedManager.GAME_ID, session, Config.DualboxCheck.DUALBOX_CHECK_MAX_PLAYERS_PER_IP))
					{
						HtmlContent htmlContent = HtmlContent.LoadFromFile("html/mods/IPRestriction.htm", null);

						htmlContent.Replace("%max%", AntiFeedManager.getInstance()
							.getLimit(session, Config.DualboxCheck.DUALBOX_CHECK_MAX_PLAYERS_PER_IP).ToString());

						NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(null, 0, htmlContent);
						connection.Send(ref msg);
						return ValueTask.CompletedTask;
					}

					if (Config.FactionSystem.FACTION_SYSTEM_ENABLED && Config.FactionSystem.FACTION_BALANCE_ONLINE_PLAYERS)
					{
						if (charInfo.IsGood && World.getInstance().getAllGoodPlayers().Count >=
						    World.getInstance().getAllEvilPlayers().Count +
						    Config.FactionSystem.FACTION_BALANCE_PLAYER_EXCEED_LIMIT)
						{
							HtmlContent htmlContent = HtmlContent.LoadFromFile("html/mods/Faction/ExceededOnlineLimit.htm", null);
							htmlContent.Replace("%more%", Config.FactionSystem.FACTION_GOOD_TEAM_NAME);
							htmlContent.Replace("%less%", Config.FactionSystem.FACTION_EVIL_TEAM_NAME);

							NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(null, 0, htmlContent);
							connection.Send(ref msg);
							return ValueTask.CompletedTask;
						}

						if (charInfo.IsEvil && World.getInstance().getAllEvilPlayers().Count >=
						    World.getInstance().getAllGoodPlayers().Count +
						    Config.FactionSystem.FACTION_BALANCE_PLAYER_EXCEED_LIMIT)
						{
							HtmlContent htmlContent = HtmlContent.LoadFromFile("html/mods/Faction/ExceededOnlineLimit.htm", null);

							htmlContent.Replace("%more%", Config.FactionSystem.FACTION_EVIL_TEAM_NAME);
							htmlContent.Replace("%less%", Config.FactionSystem.FACTION_GOOD_TEAM_NAME);

							NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(null, 0, htmlContent);
							connection.Send(ref msg);
							return ValueTask.CompletedTask;
						}
					}

					// load up character from disk
					Player? player = session.Characters.LoadPlayer(_charSlot);
					if (player == null)
						return ValueTask.CompletedTask; // handled in GameClient

					CharInfoTable.getInstance().addName(player);

					// Prevent instant disappear of invisible GMs on login.
					if (player.isGM() && Config.General.GM_STARTUP_INVISIBLE && AdminCommandData.Instance
						    .HasAccess("admin_invisible", player.getAccessLevel().Level))
					{
						player.setInvisible(true);
					}

					// Restore player location.
					PlayerVariables vars = player.getVariables();
					Location3D? restoreLocation = vars.Get<Location3D?>(PlayerVariables.RESTORE_LOCATION);
					if (restoreLocation != null)
					{
						player.setXYZ(restoreLocation.Value.X, restoreLocation.Value.Y, restoreLocation.Value.Z);
						vars.Remove(PlayerVariables.RESTORE_LOCATION);
					}

					player.setClient(session);
					session.Player = player;
					player.setOnlineStatus(true, true);
					session.Characters.UpdateLastAccessTime(_charSlot);

					if (GlobalEvents.Players.HasSubscribers<OnPlayerSelect>())
					{
						OnPlayerSelect onPlayerSelect = new(player, player.ObjectId, player.getName(), session);
						if (GlobalEvents.Players.Notify(onPlayerSelect) && onPlayerSelect.Terminate)
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
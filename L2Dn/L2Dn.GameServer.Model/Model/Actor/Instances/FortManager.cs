using System.Globalization;
using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Teleporters;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Actor.Instances;

/**
 * Fortress Foreman implementation used for: Area Teleports, Support Magic, Clan Warehouse, Exp Loss Reduction
 */
public sealed class FortManager: Merchant
{
	private const int COND_ALL_FALSE = 0;
    private const int COND_BUSY_BECAUSE_OF_SIEGE = 1;
    private const int COND_OWNER = 2;

	public const int ORC_FORTRESS_ID = 122;

    private static readonly Logger _logger = LogManager.GetLogger(nameof(FortManager));

	public FortManager(NpcTemplate template): base(template)
	{
		InstanceType = InstanceType.FortManager;
	}

	public override bool isWarehouse()
	{
		return true;
	}

	private void sendHtmlMessage(Player player, HtmlContent htmlContent)
	{
		htmlContent.Replace("%objectId%", ObjectId.ToString());
		htmlContent.Replace("%npcId%", getId().ToString());

		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(ObjectId, 0, htmlContent);
		player.sendPacket(html);
	}

	public override void onBypassFeedback(Player player, string command)
	{
		// BypassValidation Exploit plug.
		if (player.getLastFolkNPC().ObjectId != ObjectId)
			return;

		int condition = ValidateCondition(player);
		if (condition <= COND_ALL_FALSE)
			return;

        if (condition == COND_BUSY_BECAUSE_OF_SIEGE)
            return;

        if (condition != COND_OWNER)
            return;

        Fort fort = getFort() ?? throw new InvalidOperationException("Fort is null in FortManager.onBypassFeedback");
        StringTokenizer st = new StringTokenizer(command, " ");
        string actualCommand = st.nextToken(); // Get actual command
        string val = string.Empty;
        if (st.countTokens() >= 1)
        {
            val = st.nextToken();
        }

        if (actualCommand.equalsIgnoreCase("expel"))
        {
            if (player.hasClanPrivilege(ClanPrivilege.CS_DISMISS))
            {
                HtmlContent htmlContent = HtmlContent.LoadFromFile("html/fortress/foreman-expel.htm", player);
                htmlContent.Replace("%objectId%", ObjectId.ToString());
                NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(ObjectId, 0, htmlContent);
                player.sendPacket(html);
            }
            else
            {
                HtmlContent htmlContent = HtmlContent.LoadFromFile("html/fortress/foreman-noprivs.htm", player);
                htmlContent.Replace("%objectId%", ObjectId.ToString());
                NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(ObjectId, 0, htmlContent);
                player.sendPacket(html);
            }

            return;
        }

        if (actualCommand.equalsIgnoreCase("banish_foreigner"))
        {
            if (player.hasClanPrivilege(ClanPrivilege.CS_DISMISS))
            {
                fort.banishForeigners(); // Move non-clan members off fortress area
                HtmlContent htmlContent = HtmlContent.LoadFromFile("html/fortress/foreman-expeled.htm", player);
                htmlContent.Replace("%objectId%", ObjectId.ToString());
                NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(ObjectId, 0, htmlContent);
                player.sendPacket(html);
            }
            else
            {
                HtmlContent htmlContent = HtmlContent.LoadFromFile("html/fortress/foreman-noprivs.htm", player);
                htmlContent.Replace("%objectId%", ObjectId.ToString());
                NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(ObjectId, 0, htmlContent);
                player.sendPacket(html);
            }

            return;
        }

        if (actualCommand.equalsIgnoreCase("receive_report"))
        {
            if (fort.getFortState() < 2)
            {
                HtmlContent htmlContent = HtmlContent.LoadFromFile("html/fortress/foreman-report.htm", player);

                htmlContent.Replace("%objectId%", ObjectId.ToString());
                if (Config.FS_MAX_OWN_TIME > 0)
                {
                    TimeSpan time = fort.getTimeTillRebelArmy() ?? TimeSpan.Zero;
                    htmlContent.Replace("%hr%", time.Hours.ToString());
                    htmlContent.Replace("%min%", time.Minutes.ToString());
                }
                else
                {
                    TimeSpan time = fort.getOwnedTime() ?? TimeSpan.Zero;
                    htmlContent.Replace("%hr%", time.Hours.ToString());
                    htmlContent.Replace("%min%", time.Minutes.ToString());
                }

                NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(ObjectId, 0, htmlContent);
                player.sendPacket(html);
            }
            else
            {
                HtmlContent htmlContent =
                    HtmlContent.LoadFromFile("html/fortress/foreman-castlereport.htm", player);

                htmlContent.Replace("%objectId%", ObjectId.ToString());
                if (Config.FS_MAX_OWN_TIME > 0)
                {
                    TimeSpan time = fort.getTimeTillRebelArmy() ?? TimeSpan.Zero;
                    htmlContent.Replace("%hr%", time.Hours.ToString());
                    htmlContent.Replace("%min%", time.Minutes.ToString());
                }
                else
                {
                    TimeSpan time = fort.getOwnedTime() ?? TimeSpan.Zero;
                    htmlContent.Replace("%hr%", time.Hours.ToString());
                    htmlContent.Replace("%min%", time.Minutes.ToString());
                }

                TimeSpan time2 = fort.getTimeTillNextFortUpdate();
                htmlContent.Replace("%castle%", fort.getContractedCastle().getName());
                htmlContent.Replace("%hr2%", time2.Hours.ToString());
                htmlContent.Replace("%min2%", time2.Minutes.ToString());

                NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(ObjectId, 0, htmlContent);
                player.sendPacket(html);
            }

            return;
        }

        if (actualCommand.equalsIgnoreCase("operate_door")) // door
            // control
        {
            if (player.hasClanPrivilege(ClanPrivilege.CS_OPEN_DOOR))
            {
                if (!string.IsNullOrEmpty(val))
                {
                    bool open = int.Parse(val) == 1;
                    while (st.hasMoreTokens())
                    {
                        fort.openCloseDoor(player, int.Parse(st.nextToken()), open);
                    }

                    if (open)
                    {
                        if (fort.getResidenceId() == ORC_FORTRESS_ID)
                        {
                            return;
                        }

                        HtmlContent htmlContent =
                            HtmlContent.LoadFromFile("html/fortress/foreman-opened.htm", player);
                        htmlContent.Replace("%objectId%", ObjectId.ToString());
                        NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(ObjectId, 0, htmlContent);
                        player.sendPacket(html);
                    }
                    else
                    {
                        if (fort.getResidenceId() == ORC_FORTRESS_ID)
                        {
                            return;
                        }

                        HtmlContent htmlContent =
                            HtmlContent.LoadFromFile("html/fortress/foreman-closed.htm", player);
                        htmlContent.Replace("%objectId%", ObjectId.ToString());
                        NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(ObjectId, 0, htmlContent);
                        player.sendPacket(html);
                    }
                }
                else
                {
                    if (fort.getResidenceId() == ORC_FORTRESS_ID)
                    {
                        return;
                    }

                    HtmlContent htmlContent = HtmlContent.LoadFromFile(
                        "html/fortress/" + getTemplate().getId() + "-d.htm",
                        player);

                    htmlContent.Replace("%objectId%", ObjectId.ToString());
                    htmlContent.Replace("%npcname%", getName());
                    NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(ObjectId, 0, htmlContent);
                    player.sendPacket(html);
                }
            }
            else
            {
                HtmlContent htmlContent = HtmlContent.LoadFromFile("html/fortress/foreman-noprivs.htm", player);
                htmlContent.Replace("%objectId%", ObjectId.ToString());
                NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(ObjectId, 0, htmlContent);
                player.sendPacket(html);
            }

            return;
        }

        if (actualCommand.equalsIgnoreCase("manage_vault"))
        {
            if (player.hasClanPrivilege(ClanPrivilege.CL_VIEW_WAREHOUSE))
            {
                if (val.equalsIgnoreCase("deposit"))
                {
                    ShowVaultWindowDeposit(player);
                }
                else if (val.equalsIgnoreCase("withdraw"))
                {
                    ShowVaultWindowWithdraw(player);
                }
                else
                {
                    HtmlContent htmlContent = HtmlContent.LoadFromFile("html/fortress/foreman-vault.htm", player);
                    sendHtmlMessage(player, htmlContent);
                }
            }
            else
            {
                HtmlContent htmlContent = HtmlContent.LoadFromFile("html/fortress/foreman-noprivs.htm", player);
                sendHtmlMessage(player, htmlContent);
            }

            return;
        }

        if (actualCommand.equalsIgnoreCase("functions"))
        {
            if (val.equalsIgnoreCase("tele"))
            {
                Fort.FortFunction? funcTeleport = fort.getFortFunction(Fort.FUNC_TELEPORT);
                if (funcTeleport == null)
                {
                    HtmlContent htmlContent = HtmlContent.LoadFromFile("html/fortress/foreman-nac.htm", player);
                    sendHtmlMessage(player, htmlContent);
                }
                else
                {
                    HtmlContent htmlContent = HtmlContent.LoadFromFile(
                        "html/fortress/" + getId() + "-t" + funcTeleport.getLevel() + ".htm", player);

                    sendHtmlMessage(player, htmlContent);
                }
            }
            else if (val.equalsIgnoreCase("support"))
            {
                Fort.FortFunction? funcSupport = fort.getFortFunction(Fort.FUNC_SUPPORT);
                if (funcSupport == null)
                {
                    HtmlContent htmlContent = HtmlContent.LoadFromFile("html/fortress/foreman-nac.htm", player);
                    sendHtmlMessage(player, htmlContent);
                }
                else
                {
                    HtmlContent htmlContent = HtmlContent.LoadFromFile(
                        "html/fortress/support" + funcSupport.getLevel() + ".htm", player);

                    htmlContent.Replace("%mp%", ((int)getCurrentMp()).ToString());
                    sendHtmlMessage(player, htmlContent);
                }
            }
            else if (val.equalsIgnoreCase("back"))
            {
                showChatWindow(player);
            }
            else
            {
                HtmlContent htmlContent = HtmlContent.LoadFromFile("html/fortress/foreman-functions.htm", player);

                Fort.FortFunction? funcRestoreExp = fort.getFortFunction(Fort.FUNC_RESTORE_EXP);
                if (funcRestoreExp != null)
                {
                    htmlContent.Replace("%xp_regen%", funcRestoreExp.getLevel().ToString());
                }
                else
                {
                    htmlContent.Replace("%xp_regen%", "0");
                }

                Fort.FortFunction? funcRestoreHp = fort.getFortFunction(Fort.FUNC_RESTORE_HP);
                if (funcRestoreHp != null)
                {
                    htmlContent.Replace("%hp_regen%", funcRestoreHp.getLevel().ToString());
                }
                else
                {
                    htmlContent.Replace("%hp_regen%", "0");
                }

                Fort.FortFunction? funcRestoreMp = fort.getFortFunction(Fort.FUNC_RESTORE_MP);
                if (funcRestoreMp != null)
                {
                    htmlContent.Replace("%mp_regen%", funcRestoreMp.getLevel().ToString());
                }
                else
                {
                    htmlContent.Replace("%mp_regen%", "0");
                }

                sendHtmlMessage(player, htmlContent);
            }

            return;
        }

        if (actualCommand.equalsIgnoreCase("manage"))
        {
            if (player.hasClanPrivilege(ClanPrivilege.CS_SET_FUNCTIONS))
            {
                if (val.equalsIgnoreCase("recovery"))
                {
                    if (st.countTokens() >= 1)
                    {
                        if (fort.getOwnerClan() == null)
                        {
                            player.sendMessage("This fortress has no owner, you cannot change the configuration.");
                            return;
                        }

                        val = st.nextToken();
                        if (val.equalsIgnoreCase("hp_cancel"))
                        {
                            HtmlContent htmlContent =
                                HtmlContent.LoadFromFile("html/fortress/functions-cancel.htm", player);
                            htmlContent.Replace("%apply%", "recovery hp 0");
                            sendHtmlMessage(player, htmlContent);
                            return;
                        }

                        if (val.equalsIgnoreCase("mp_cancel"))
                        {
                            HtmlContent htmlContent =
                                HtmlContent.LoadFromFile("html/fortress/functions-cancel.htm", player);
                            htmlContent.Replace("%apply%", "recovery mp 0");
                            sendHtmlMessage(player, htmlContent);
                            return;
                        }

                        if (val.equalsIgnoreCase("exp_cancel"))
                        {
                            HtmlContent htmlContent =
                                HtmlContent.LoadFromFile("html/fortress/functions-cancel.htm", player);
                            htmlContent.Replace("%apply%", "recovery exp 0");
                            sendHtmlMessage(player, htmlContent);
                            return;
                        }

                        if (val.equalsIgnoreCase("edit_hp"))
                        {
                            val = st.nextToken();
                            HtmlContent htmlContent =
                                HtmlContent.LoadFromFile("html/fortress/functions-apply.htm", player);
                            htmlContent.Replace("%name%", "(HP Recovery Device)");
                            int percent = int.Parse(val);
                            int cost;
                            switch (percent)
                            {
                                case 300:
                                {
                                    cost = Config.FS_HPREG1_FEE;
                                    break;
                                }
                                default: // 400
                                {
                                    cost = Config.FS_HPREG2_FEE;
                                    break;
                                }
                            }

                            htmlContent.Replace("%cost%",
                                cost + "</font>Adena /" + Config.FS_HPREG_FEE_RATIO / 1000 / 60 / 60 / 24 +
                                " Day</font>)");
                            htmlContent.Replace("%use%",
                                "Provides additional HP recovery for clan members in the fortress.<font color=\"00FFFF\">" +
                                percent + "%</font>");
                            htmlContent.Replace("%apply%", "recovery hp " + percent);
                            sendHtmlMessage(player, htmlContent);
                            return;
                        }

                        if (val.equalsIgnoreCase("edit_mp"))
                        {
                            val = st.nextToken();
                            HtmlContent htmlContent =
                                HtmlContent.LoadFromFile("html/fortress/functions-apply.htm", player);

                            htmlContent.Replace("%name%", "(MP Recovery)");
                            int percent = int.Parse(val);
                            int cost;
                            switch (percent)
                            {
                                case 40:
                                {
                                    cost = Config.FS_MPREG1_FEE;
                                    break;
                                }
                                default: // 50
                                {
                                    cost = Config.FS_MPREG2_FEE;
                                    break;
                                }
                            }

                            htmlContent.Replace("%cost%",
                                cost + "</font>Adena /" + Config.FS_MPREG_FEE_RATIO / 1000 / 60 / 60 / 24 +
                                " Day</font>)");
                            htmlContent.Replace("%use%",
                                "Provides additional MP recovery for clan members in the fortress.<font color=\"00FFFF\">" +
                                percent + "%</font>");
                            htmlContent.Replace("%apply%", "recovery mp " + percent);
                            sendHtmlMessage(player, htmlContent);
                            return;
                        }

                        if (val.equalsIgnoreCase("edit_exp"))
                        {
                            val = st.nextToken();

                            HtmlContent htmlContent =
                                HtmlContent.LoadFromFile("html/fortress/functions-apply.htm", player);
                            htmlContent.Replace("%name%", "(EXP Recovery Device)");
                            int percent = int.Parse(val);
                            int cost;
                            switch (percent)
                            {
                                case 45:
                                {
                                    cost = Config.FS_EXPREG1_FEE;
                                    break;
                                }
                                default: // 50
                                {
                                    cost = Config.FS_EXPREG2_FEE;
                                    break;
                                }
                            }

                            htmlContent.Replace("%cost%",
                                cost + "</font>Adena /" + Config.FS_EXPREG_FEE_RATIO / 1000 / 60 / 60 / 24 +
                                " Day</font>)");
                            htmlContent.Replace("%use%",
                                "Restores the Exp of any clan member who is resurrected in the fortress.<font color=\"00FFFF\">" +
                                percent + "%</font>");
                            htmlContent.Replace("%apply%", "recovery exp " + percent);
                            sendHtmlMessage(player, htmlContent);
                            return;
                        }

                        if (val.equalsIgnoreCase("hp"))
                        {
                            if (st.countTokens() >= 1)
                            {
                                HtmlContent htmlContent;
                                int fee;
                                val = st.nextToken();

                                Fort.FortFunction? funcRestoreHp = fort.getFortFunction(Fort.FUNC_RESTORE_HP);
                                if (funcRestoreHp != null)
                                {
                                    if (funcRestoreHp.getLevel() == int.Parse(val))
                                    {
                                        htmlContent = HtmlContent.LoadFromFile("html/fortress/functions-used.htm",
                                            player);
                                        htmlContent.Replace("%val%", val + "%");
                                        sendHtmlMessage(player, htmlContent);
                                        return;
                                    }
                                }

                                htmlContent = HtmlContent.LoadFromFile(
                                    "html/fortress/functions-apply_confirmed.htm",
                                    player);

                                int percent = int.Parse(val);
                                switch (percent)
                                {
                                    case 0:
                                    {
                                        fee = 0;

                                        htmlContent = HtmlContent.LoadFromFile(
                                            "html/fortress/functions-cancel_confirmed.htm", player);

                                        break;
                                    }
                                    case 300:
                                    {
                                        fee = Config.FS_HPREG1_FEE;
                                        break;
                                    }
                                    default: // 400
                                    {
                                        fee = Config.FS_HPREG2_FEE;
                                        break;
                                    }
                                }

                                if (!fort.updateFunctions(player, Fort.FUNC_RESTORE_HP, percent, fee,
                                        Config.FS_HPREG_FEE_RATIO,
                                        fort.getFortFunction(Fort.FUNC_RESTORE_HP) == null))
                                {
                                    htmlContent = HtmlContent.LoadFromFile("html/fortress/low_adena.htm", player);
                                }

                                sendHtmlMessage(player, htmlContent);
                            }

                            return;
                        }

                        if (val.equalsIgnoreCase("mp"))
                        {
                            if (st.countTokens() >= 1)
                            {
                                int fee;
                                val = st.nextToken();
                                HtmlContent htmlContent =
                                    HtmlContent.LoadFromFile("html/fortress/functions-apply_confirmed.htm", player);

                                Fort.FortFunction? funcRestoreMp = fort.getFortFunction(Fort.FUNC_RESTORE_MP);
                                if (funcRestoreMp != null)
                                {
                                    if (funcRestoreMp.getLevel() == int.Parse(val))
                                    {
                                        htmlContent = HtmlContent.LoadFromFile("html/fortress/functions-used.htm",
                                            player);

                                        htmlContent.Replace("%val%", val + "%");
                                        sendHtmlMessage(player, htmlContent);
                                        return;
                                    }
                                }

                                int percent = int.Parse(val);
                                switch (percent)
                                {
                                    case 0:
                                    {
                                        fee = 0;
                                        htmlContent =
                                            HtmlContent.LoadFromFile("html/fortress/functions-cancel_confirmed.htm",
                                                player);
                                        break;
                                    }
                                    case 40:
                                    {
                                        fee = Config.FS_MPREG1_FEE;
                                        break;
                                    }
                                    default: // 50
                                    {
                                        fee = Config.FS_MPREG2_FEE;
                                        break;
                                    }
                                }

                                if (!fort.updateFunctions(player, Fort.FUNC_RESTORE_MP, percent, fee,
                                        Config.FS_MPREG_FEE_RATIO,
                                        fort.getFortFunction(Fort.FUNC_RESTORE_MP) == null))
                                {
                                    htmlContent = HtmlContent.LoadFromFile("html/fortress/low_adena.htm", player);
                                }

                                sendHtmlMessage(player, htmlContent);
                            }

                            return;
                        }

                        if (val.equalsIgnoreCase("exp"))
                        {
                            if (st.countTokens() >= 1)
                            {
                                int fee;
                                val = st.nextToken();
                                HtmlContent htmlContent =
                                    HtmlContent.LoadFromFile("html/fortress/functions-apply_confirmed.htm", player);

                                Fort.FortFunction? funcRestoreExp = fort.getFortFunction(Fort.FUNC_RESTORE_EXP);
                                if (funcRestoreExp != null)
                                {
                                    if (funcRestoreExp.getLevel() == int.Parse(val))
                                    {
                                        htmlContent = HtmlContent.LoadFromFile("html/fortress/functions-used.htm",
                                            player);

                                        htmlContent.Replace("%val%", val + "%");
                                        sendHtmlMessage(player, htmlContent);
                                        return;
                                    }
                                }

                                int percent = int.Parse(val);
                                switch (percent)
                                {
                                    case 0:
                                    {
                                        fee = 0;
                                        htmlContent =
                                            HtmlContent.LoadFromFile("html/fortress/functions-cancel_confirmed.htm",
                                                player);

                                        break;
                                    }
                                    case 45:
                                    {
                                        fee = Config.FS_EXPREG1_FEE;
                                        break;
                                    }
                                    default: // 50
                                    {
                                        fee = Config.FS_EXPREG2_FEE;
                                        break;
                                    }
                                }

                                if (!fort.updateFunctions(player, Fort.FUNC_RESTORE_EXP, percent, fee,
                                        Config.FS_EXPREG_FEE_RATIO,
                                        fort.getFortFunction(Fort.FUNC_RESTORE_EXP) == null))
                                {
                                    htmlContent = HtmlContent.LoadFromFile("html/fortress/low_adena.htm", player);
                                }

                                sendHtmlMessage(player, htmlContent);
                            }

                            return;
                        }
                    }

                    {
                        HtmlContent htmlContent =
                            HtmlContent.LoadFromFile("html/fortress/edit_recovery.htm", player);
                        string hp =
                            "[<a action=\"bypass -h npc_%objectId%_manage recovery edit_hp 300\">300%</a>][<a action=\"bypass -h npc_%objectId%_manage recovery edit_hp 400\">400%</a>]";
                        string exp =
                            "[<a action=\"bypass -h npc_%objectId%_manage recovery edit_exp 45\">45%</a>][<a action=\"bypass -h npc_%objectId%_manage recovery edit_exp 50\">50%</a>]";
                        string mp =
                            "[<a action=\"bypass -h npc_%objectId%_manage recovery edit_mp 40\">40%</a>][<a action=\"bypass -h npc_%objectId%_manage recovery edit_mp 50\">50%</a>]";

                        Fort.FortFunction? funcRestoreHp = fort.getFortFunction(Fort.FUNC_RESTORE_HP);
                        if (funcRestoreHp != null)
                        {
                            htmlContent.Replace("%hp_recovery%", funcRestoreHp.getLevel() +
                                "%</font> (<font color=\"FFAABB\">" + funcRestoreHp.getLease() + "</font>Adena /" +
                                Config.FS_HPREG_FEE_RATIO / 1000 / 60 / 60 / 24 + " Day)");

                            htmlContent.Replace("%hp_period%",
                                "Withdraw the fee for the next time at " + funcRestoreHp.getEndTime()
                                    ?.ToString("dd/MM/yyyy HH:mm"));

                            htmlContent.Replace("%change_hp%",
                                "[<a action=\"bypass -h npc_%objectId%_manage recovery hp_cancel\">Deactivate</a>]" +
                                hp);
                        }
                        else
                        {
                            htmlContent.Replace("%hp_recovery%", "none");
                            htmlContent.Replace("%hp_period%", "none");
                            htmlContent.Replace("%change_hp%", hp);
                        }

                        Fort.FortFunction? funcRestoreExp = fort.getFortFunction(Fort.FUNC_RESTORE_EXP);
                        if (funcRestoreExp != null)
                        {
                            htmlContent.Replace("%exp_recovery%", funcRestoreExp.getLevel() +
                                "%</font> (<font color=\"FFAABB\">" + funcRestoreExp.getLease() + "</font>Adena /" +
                                Config.FS_EXPREG_FEE_RATIO / 1000 / 60 / 60 / 24 + " Day)");
                            htmlContent.Replace("%exp_period%",
                                "Withdraw the fee for the next time at " +
                                funcRestoreExp.getEndTime()?.ToString("dd/MM/yyyy HH:mm"));
                            htmlContent.Replace("%change_exp%",
                                "[<a action=\"bypass -h npc_%objectId%_manage recovery exp_cancel\">Deactivate</a>]" +
                                exp);
                        }
                        else
                        {
                            htmlContent.Replace("%exp_recovery%", "none");
                            htmlContent.Replace("%exp_period%", "none");
                            htmlContent.Replace("%change_exp%", exp);
                        }

                        Fort.FortFunction? funcRestoreMp = fort.getFortFunction(Fort.FUNC_RESTORE_MP);
                        if (funcRestoreMp != null)
                        {
                            htmlContent.Replace("%mp_recovery%",
                                funcRestoreMp.getLevel() + "%</font> (<font color=\"FFAABB\">" +
                                funcRestoreMp.getLease() + "</font>Adena /" +
                                Config.FS_MPREG_FEE_RATIO / 1000 / 60 / 60 / 24 + " Day)");
                            htmlContent.Replace("%mp_period%",
                                "Withdraw the fee for the next time at " +
                                funcRestoreMp.getEndTime()?.ToString("dd/MM/yyyy HH:mm"));
                            htmlContent.Replace("%change_mp%",
                                "[<a action=\"bypass -h npc_%objectId%_manage recovery mp_cancel\">Deactivate</a>]" +
                                mp);
                        }
                        else
                        {
                            htmlContent.Replace("%mp_recovery%", "none");
                            htmlContent.Replace("%mp_period%", "none");
                            htmlContent.Replace("%change_mp%", mp);
                        }

                        sendHtmlMessage(player, htmlContent);
                    }
                }
                else if (val.equalsIgnoreCase("other"))
                {
                    if (st.countTokens() >= 1)
                    {
                        if (fort.getOwnerClan() == null)
                        {
                            player.sendMessage("This fortress has no owner, you cannot change the configuration.");
                            return;
                        }

                        val = st.nextToken();
                        if (val.equalsIgnoreCase("tele_cancel"))
                        {
                            HtmlContent htmlContent =
                                HtmlContent.LoadFromFile("html/fortress/functions-cancel.htm", player);
                            htmlContent.Replace("%apply%", "other tele 0");
                            sendHtmlMessage(player, htmlContent);
                            return;
                        }

                        if (val.equalsIgnoreCase("support_cancel"))
                        {
                            HtmlContent htmlContent =
                                HtmlContent.LoadFromFile("html/fortress/functions-cancel.htm", player);
                            htmlContent.Replace("%apply%", "other support 0");
                            sendHtmlMessage(player, htmlContent);
                            return;
                        }

                        if (val.equalsIgnoreCase("edit_support"))
                        {
                            val = st.nextToken();
                            HtmlContent htmlContent =
                                HtmlContent.LoadFromFile("html/fortress/functions-apply.htm", player);

                            htmlContent.Replace("%name%", "Insignia (Supplementary Magic)");
                            int stage = int.Parse(val);
                            int cost;
                            switch (stage)
                            {
                                case 1:
                                {
                                    cost = Config.FS_SUPPORT1_FEE;
                                    break;
                                }
                                default:
                                {
                                    cost = Config.FS_SUPPORT2_FEE;
                                    break;
                                }
                            }

                            htmlContent.Replace("%cost%",
                                cost + "</font>Adena /" + Config.FS_SUPPORT_FEE_RATIO / 1000 / 60 / 60 / 24 +
                                " Day</font>)");
                            htmlContent.Replace("%use%", "Enables the use of supplementary magic.");
                            htmlContent.Replace("%apply%", "other support " + stage);
                            sendHtmlMessage(player, htmlContent);
                            return;
                        }

                        if (val.equalsIgnoreCase("edit_tele"))
                        {
                            val = st.nextToken();
                            HtmlContent htmlContent =
                                HtmlContent.LoadFromFile("html/fortress/functions-apply.htm", player);
                            htmlContent.Replace("%name%", "Mirror (Teleportation Device)");
                            int stage = int.Parse(val);
                            int cost;
                            switch (stage)
                            {
                                case 1:
                                {
                                    cost = Config.FS_TELE1_FEE;
                                    break;
                                }
                                default:
                                {
                                    cost = Config.FS_TELE2_FEE;
                                    break;
                                }
                            }

                            htmlContent.Replace("%cost%",
                                cost + "</font>Adena /" + Config.FS_TELE_FEE_RATIO / 1000 / 60 / 60 / 24 +
                                " Day</font>)");
                            htmlContent.Replace("%use%",
                                "Teleports clan members in a fort to the target <font color=\"00FFFF\">Stage " +
                                stage + "</font> staging area");
                            htmlContent.Replace("%apply%", "other tele " + stage);
                            sendHtmlMessage(player, htmlContent);
                            return;
                        }

                        if (val.equalsIgnoreCase("tele"))
                        {
                            if (st.countTokens() >= 1)
                            {
                                int fee;
                                val = st.nextToken();
                                HtmlContent htmlContent =
                                    HtmlContent.LoadFromFile("html/fortress/functions-apply_confirmed.htm", player);

                                Fort.FortFunction? funcTeleport = fort.getFortFunction(Fort.FUNC_TELEPORT);
                                if (funcTeleport != null)
                                {
                                    if (funcTeleport.getLevel() == int.Parse(val))
                                    {
                                        htmlContent = HtmlContent.LoadFromFile("html/fortress/functions-used.htm",
                                            player);
                                        htmlContent.Replace("%val%", "Stage " + val);
                                        sendHtmlMessage(player, htmlContent);
                                        return;
                                    }
                                }

                                int level = int.Parse(val);
                                switch (level)
                                {
                                    case 0:
                                    {
                                        fee = 0;
                                        htmlContent =
                                            HtmlContent.LoadFromFile("html/fortress/functions-cancel_confirmed.htm",
                                                player);
                                        break;
                                    }
                                    case 1:
                                    {
                                        fee = Config.FS_TELE1_FEE;
                                        break;
                                    }
                                    default:
                                    {
                                        fee = Config.FS_TELE2_FEE;
                                        break;
                                    }
                                }

                                if (!fort.updateFunctions(player, Fort.FUNC_TELEPORT, level, fee,
                                        Config.FS_TELE_FEE_RATIO,
                                        fort.getFortFunction(Fort.FUNC_TELEPORT) == null))
                                {
                                    htmlContent = HtmlContent.LoadFromFile("html/fortress/low_adena.htm", player);
                                }

                                sendHtmlMessage(player, htmlContent);
                            }

                            return;
                        }

                        if (val.equalsIgnoreCase("support"))
                        {
                            if (st.countTokens() >= 1)
                            {
                                int fee;
                                val = st.nextToken();

                                HtmlContent htmlContent =
                                    HtmlContent.LoadFromFile("html/fortress/functions-apply_confirmed.htm", player);

                                Fort.FortFunction? funcSupport = fort.getFortFunction(Fort.FUNC_SUPPORT);
                                if (funcSupport != null)
                                {
                                    if (funcSupport.getLevel() == int.Parse(val))
                                    {
                                        htmlContent = HtmlContent.LoadFromFile("html/fortress/functions-used.htm",
                                            player);

                                        htmlContent.Replace("%val%", "Stage " + val);
                                        sendHtmlMessage(player, htmlContent);
                                        return;
                                    }
                                }

                                int level = int.Parse(val);
                                switch (level)
                                {
                                    case 0:
                                    {
                                        fee = 0;
                                        htmlContent =
                                            HtmlContent.LoadFromFile("html/fortress/functions-cancel_confirmed.htm",
                                                player);

                                        break;
                                    }
                                    case 1:
                                    {
                                        fee = Config.FS_SUPPORT1_FEE;
                                        break;
                                    }
                                    default:
                                    {
                                        fee = Config.FS_SUPPORT2_FEE;
                                        break;
                                    }
                                }

                                if (!fort.updateFunctions(player, Fort.FUNC_SUPPORT, level, fee,
                                        Config.FS_SUPPORT_FEE_RATIO,
                                        fort.getFortFunction(Fort.FUNC_SUPPORT) == null))
                                {
                                    htmlContent = HtmlContent.LoadFromFile("html/fortress/low_adena.htm", player);
                                }

                                sendHtmlMessage(player, htmlContent);
                            }

                            return;
                        }
                    }

                    {
                        HtmlContent htmlContent = HtmlContent.LoadFromFile("html/fortress/edit_other.htm", player);

                        string tele =
                            "[<a action=\"bypass -h npc_%objectId%_manage other edit_tele 1\">Level 1</a>][<a action=\"bypass -h npc_%objectId%_manage other edit_tele 2\">Level 2</a>]";
                        string support =
                            "[<a action=\"bypass -h npc_%objectId%_manage other edit_support 1\">Level 1</a>][<a action=\"bypass -h npc_%objectId%_manage other edit_support 2\">Level 2</a>]";

                        Fort.FortFunction? funcTeleport = fort.getFortFunction(Fort.FUNC_TELEPORT);
                        if (funcTeleport != null)
                        {
                            htmlContent.Replace("%tele%",
                                "Stage " + funcTeleport.getLevel() + "</font> (<font color=\"FFAABB\">" +
                                funcTeleport.getLease() + "</font>Adena /" +
                                Config.FS_TELE_FEE_RATIO / 1000 / 60 / 60 / 24 + " Day)");
                            htmlContent.Replace("%tele_period%",
                                "Withdraw the fee for the next time at " +
                                funcTeleport.getEndTime()?.ToString("dd/MM/yyyy HH:mm"));
                            htmlContent.Replace("%change_tele%",
                                "[<a action=\"bypass -h npc_%objectId%_manage other tele_cancel\">Deactivate</a>]" +
                                tele);
                        }
                        else
                        {
                            htmlContent.Replace("%tele%", "none");
                            htmlContent.Replace("%tele_period%", "none");
                            htmlContent.Replace("%change_tele%", tele);
                        }

                        Fort.FortFunction? funcSupport = fort.getFortFunction(Fort.FUNC_SUPPORT);
                        if (funcSupport != null)
                        {
                            htmlContent.Replace("%support%",
                                "Stage " + funcSupport.getLevel() + "</font> (<font color=\"FFAABB\">" +
                                funcSupport.getLease() + "</font>Adena /" +
                                Config.FS_SUPPORT_FEE_RATIO / 1000 / 60 / 60 / 24 + " Day)");
                            htmlContent.Replace("%support_period%",
                                "Withdraw the fee for the next time at " +
                                funcSupport.getEndTime()?.ToString("dd/MM/yyyy HH:mm"));
                            htmlContent.Replace("%change_support%",
                                "[<a action=\"bypass -h npc_%objectId%_manage other support_cancel\">Deactivate</a>]" +
                                support);
                        }
                        else
                        {
                            htmlContent.Replace("%support%", "none");
                            htmlContent.Replace("%support_period%", "none");
                            htmlContent.Replace("%change_support%", support);
                        }

                        sendHtmlMessage(player, htmlContent);
                    }
                }
                else if (val.equalsIgnoreCase("back"))
                {
                    showChatWindow(player);
                }
                else
                {
                    HtmlContent htmlContent = HtmlContent.LoadFromFile("html/fortress/manage.htm", player);
                    sendHtmlMessage(player, htmlContent);
                }
            }
            else
            {
                HtmlContent htmlContent = HtmlContent.LoadFromFile("html/fortress/foreman-noprivs.htm", player);
                sendHtmlMessage(player, htmlContent);
            }

            return;
        }

        if (actualCommand.equalsIgnoreCase("support"))
        {
            setTarget(player);
            if (string.IsNullOrEmpty(val) || !int.TryParse(val, CultureInfo.InvariantCulture, out int skillId))
                return;

            Fort.FortFunction? funcSupport = fort.getFortFunction(Fort.FUNC_SUPPORT);
            if (funcSupport == null || funcSupport.getLevel() == 0)
                return;

            HtmlContent htmlContent;
            int skillLevel = 0;
            if (st.countTokens() >= 1)
            {
                skillLevel = int.Parse(st.nextToken());
            }

            Skill? skill = SkillData.getInstance().getSkill(skillId, skillLevel);
            if (skill == null)
            {
                _logger.Warn($"Invalid skill id={skillId}, level={skillLevel}");
                player.sendMessage("Invalid skill, contact your admin!");
                return;
            }

            if (skill.hasEffectType(EffectType.SUMMON))
            {
                player.doCast(skill);
            }
            else if (skill.getMpConsume() + skill.getMpInitialConsume() <= getCurrentMp())
            {
                doCast(skill);
            }
            else
            {
                htmlContent = HtmlContent.LoadFromFile("html/fortress/support-no_mana.htm", player);
                htmlContent.Replace("%mp%", ((int)getCurrentMp()).ToString());
                sendHtmlMessage(player, htmlContent);
                return;
            }

            htmlContent = HtmlContent.LoadFromFile("html/fortress/support-done.htm", player);
            htmlContent.Replace("%mp%", ((int)getCurrentMp()).ToString());
            sendHtmlMessage(player, htmlContent);

            return;
        }

        if (actualCommand.equalsIgnoreCase("support_back"))
        {
            Fort.FortFunction? funcSupport = fort.getFortFunction(Fort.FUNC_SUPPORT);
            if (funcSupport == null || funcSupport.getLevel() == 0)
                return;

            HtmlContent htmlContent = HtmlContent.LoadFromFile(
                "html/fortress/support" + funcSupport.getLevel() + ".htm", player);

            htmlContent.Replace("%mp%", ((int)getStatus().getCurrentMp()).ToString());
            sendHtmlMessage(player, htmlContent);
            return;
        }

        if (actualCommand.equalsIgnoreCase("goto")) // goto listId locId
        {
            Fort.FortFunction? funcTeleport = fort.getFortFunction(Fort.FUNC_TELEPORT);
            if (funcTeleport == null || !st.hasMoreTokens())
                return;

            int funcLvl = val.Length >= 4 ? val[3..].TryParseOrDefault(-1) : -1;
            if (funcTeleport.getLevel() == funcLvl)
            {
                TeleportHolder? holder = TeleporterData.getInstance().getHolder(getId(), val);
                if (holder is not null)
                {
                    int locId = st.nextToken().Trim().TryParseOrDefault(-1);
                    holder.doTeleport(player, this, locId);
                }
            }

            return;
        }

        base.onBypassFeedback(player, command);
    }

	public override void showChatWindow(Player player)
	{
		player.sendPacket(ActionFailedPacket.STATIC_PACKET);
		string filename = "html/fortress/foreman-no.htm";

		int condition = ValidateCondition(player);
		if (condition > COND_ALL_FALSE)
		{
			if (condition == COND_BUSY_BECAUSE_OF_SIEGE)
			{
				filename = "html/fortress/foreman-busy.htm"; // Busy because of siege
			}
			else if (condition == COND_OWNER)
			{
				filename = "html/fortress/foreman.htm"; // Owner message window
			}
		}

		HtmlContent htmlContent = HtmlContent.LoadFromFile(filename, player);
		htmlContent.Replace("%objectId%", ObjectId.ToString());
		htmlContent.Replace("%npcname%", getName());
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(ObjectId, 0, htmlContent);
		player.sendPacket(html);
	}

	private int ValidateCondition(Player player)
    {
        Fort? fort = getFort();
		if (fort != null && fort.getResidenceId() > 0 && player.getClan() != null)
        {
            if (fort.getZone().isActive())
			{
				return COND_BUSY_BECAUSE_OF_SIEGE; // Busy because of siege
			}

            Clan? ownerClan = fort.getOwnerClan();
            if (ownerClan != null && ownerClan.getId() == player.getClanId())
            {
                return COND_OWNER; // Owner
            }
        }

		return COND_ALL_FALSE;
	}

	private static void ShowVaultWindowDeposit(Player player)
    {
		player.sendPacket(ActionFailedPacket.STATIC_PACKET);

        Clan? clan = player.getClan();
        if (clan == null)
            return;

		player.setActiveWarehouse(clan.getWarehouse());
		player.sendPacket(new WarehouseDepositListPacket(1, player, WarehouseDepositListPacket.CLAN));
	}

	private void ShowVaultWindowWithdraw(Player player)
	{
        Clan? clan = player.getClan();
		if (clan != null && (player.isClanLeader() || player.hasClanPrivilege(ClanPrivilege.CL_VIEW_WAREHOUSE)))
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			player.setActiveWarehouse(clan.getWarehouse());
			player.sendPacket(new WarehouseWithdrawalListPacket(1, player, WarehouseWithdrawalListPacket.CLAN));
		}
		else
		{
			HtmlContent htmlContent = HtmlContent.LoadFromFile("html/fortress/foreman-noprivs.htm", player);
			sendHtmlMessage(player, htmlContent);
		}
	}
}
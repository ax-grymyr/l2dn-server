using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Html.Styles;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author Sahar, Nik64
 */
public class AdminFence: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_addfence",
		"admin_setfencestate",
		"admin_removefence",
		"admin_listfence",
		"admin_gofence"
	};
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command, " ");
		string cmd = st.nextToken();
		switch (cmd)
		{
			case "admin_addfence":
			{
				try
				{
					int width = int.Parse(st.nextToken());
					int Length = int.Parse(st.nextToken());
					int height = int.Parse(st.nextToken());
					if ((width < 1) || (Length < 1))
					{
						BuilderUtil.sendSysMessage(activeChar, "Width and length values must be positive numbers.");
						return false;
					}
					if ((height < 1) || (height > 3))
					{
						BuilderUtil.sendSysMessage(activeChar, "The range for height can only be 1-3.");
						return false;
					}
					
					FenceData.getInstance().spawnFence(activeChar.Location.Location3D, width, Length, height, activeChar.getInstanceId(), FenceState.CLOSED);
					BuilderUtil.sendSysMessage(activeChar, "Fence added succesfully.");
				}
				catch (Exception e)
				{
					BuilderUtil.sendSysMessage(activeChar, "Format must be: //addfence <width> <length> <height>");
				}
				break;
			}
			case "admin_setfencestate":
			{
				try
				{
					var fenceValues = EnumUtil.GetValues<FenceState>();
					int objId = int.Parse(st.nextToken());
					int fenceTypeOrdinal = int.Parse(st.nextToken());
					if ((fenceTypeOrdinal < 0) || (fenceTypeOrdinal >= fenceValues.Length))
					{
						BuilderUtil.sendSysMessage(activeChar, "Specified FenceType is out of range. Only 0-" + (fenceValues.Length - 1) + " are permitted.");
					}
					else
					{
						WorldObject obj = World.getInstance().findObject(objId);
						if (obj is Fence)
						{
							Fence fence = (Fence) obj;
							FenceState state = fenceValues[fenceTypeOrdinal];
							fence.setState(state);
							BuilderUtil.sendSysMessage(activeChar, "Fence " + fence.getName() + "[" + fence.getId() + "]'s state has been changed to " + state.ToString());
						}
						else
						{
							BuilderUtil.sendSysMessage(activeChar, "Target is not a fence.");
						}
					}
				}
				catch (Exception e)
				{
					BuilderUtil.sendSysMessage(activeChar, "Format mustr be: //setfencestate <fenceObjectId> <fenceState>");
				}
				break;
			}
			case "admin_removefence":
			{
				try
				{
					int objId = int.Parse(st.nextToken());
					WorldObject obj = World.getInstance().findObject(objId);
					if (obj is Fence)
					{
						((Fence) obj).deleteMe();
						BuilderUtil.sendSysMessage(activeChar, "Fence removed succesfully.");
					}
					else
					{
						BuilderUtil.sendSysMessage(activeChar, "Target is not a fence.");
					}
				}
				catch (Exception e)
				{
					BuilderUtil.sendSysMessage(activeChar, "Invalid object ID or target was not found.");
				}
				sendHtml(activeChar, 0);
				break;
			}
			case "admin_listfence":
			{
				int page = 0;
				if (st.hasMoreTokens())
				{
					page = int.Parse(st.nextToken());
				}
				sendHtml(activeChar, page);
				break;
			}
			case "admin_gofence":
			{
				try
				{
					int objId = int.Parse(st.nextToken());
					WorldObject obj = World.getInstance().findObject(objId);
					if (obj != null)
					{
						activeChar.teleToLocation(obj.Location);
					}
				}
				catch (Exception e)
				{
					BuilderUtil.sendSysMessage(activeChar, "Invalid object ID or target was not found.");
				}
				break;
			}
		}
		
		return true;
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
	
	private void sendHtml(Player activeChar, int page)
	{
		PageResult result = PageBuilder.newBuilder(FenceData.getInstance().getFences().values().ToList(), 10, "bypass -h admin_listfence").currentPage(page).style(ButtonsStyle.INSTANCE).bodyHandler((pages, fence, sb) =>
		{
			sb.Append("<tr><td>");
			sb.Append(fence.getName() == null ? fence.getId() : fence.getName());
			sb.Append("</td><td>");
			sb.Append("<button value=\"Go\" action=\"bypass -h admin_gofence ");
			sb.Append(fence.getId());
			sb.Append("\" width=30 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\">");
			sb.Append("</td><td>");
			sb.Append("<button value=\"Hide\" action=\"bypass -h admin_setfencestate ");
			sb.Append(fence.getId());
			sb.Append(" 0");
			sb.Append("\" width=30 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\">");
			sb.Append("</td><td>");
			sb.Append("<button value=\"Off\" action=\"bypass -h admin_setfencestate ");
			sb.Append(fence.getId());
			sb.Append(" 1");
			sb.Append("\" width=30 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\">");
			sb.Append("</td><td>");
			sb.Append("<button value=\"On\" action=\"bypass -h admin_setfencestate ");
			sb.Append(fence.getId());
			sb.Append(" 2");
			sb.Append("\" width=30 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\">");
			sb.Append("</td><td>");
			sb.Append("<button value=\"X\" action=\"bypass -h admin_removefence ");
			sb.Append(fence.getId());
			sb.Append("\" width=30 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\">");
			sb.Append("</td></tr>");
		}).build();

		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/fences.htm", activeChar);
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);
		if (result.getPages() > 0)
		{
			htmlContent.Replace("%pages%", "<table width=280 cellspacing=0><tr>" + result.getPagerTemplate() + "</tr></table>");
		}
		else
		{
			htmlContent.Replace("%pages%", "");
		}
		
		htmlContent.Replace("%fences%", result.getBodyTemplate().ToString());
		activeChar.sendPacket(html);
	}
}
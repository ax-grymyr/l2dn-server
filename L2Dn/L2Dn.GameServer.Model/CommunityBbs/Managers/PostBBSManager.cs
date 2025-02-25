using System.Globalization;
using L2Dn.GameServer.CommunityBbs.BB;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.CommunityBbs.Managers;

public class PostBBSManager: BaseBBSManager
{
	private readonly Map<Topic, Post> _postByTopic = new();

	public Post getGPosttByTopic(Topic topic) => _postByTopic.GetOrAdd(topic, t => new Post(t));

    public void delPostByTopic(Topic t)
	{
		_postByTopic.remove(t);
	}

	public void addPostByTopic(Post p, Topic t)
	{
		if (_postByTopic.get(t) == null)
		{
			_postByTopic.put(t, p);
		}
	}

	public override void ParseCmd(string command, Player player)
	{
		if (command.startsWith("_bbsposts;read;"))
		{
			StringTokenizer st = new StringTokenizer(command, ";");
			st.nextToken();
			st.nextToken();
			int idf = int.Parse(st.nextToken());
			int idp = int.Parse(st.nextToken());
			string? index = st.hasMoreTokens() ? st.nextToken() : null;
			int ind = index == null ? 1 : int.Parse(index);
			showPost(TopicBBSManager.getInstance().getTopicByID(idp), ForumsBBSManager.getInstance().getForumByID(idf), player, ind);
		}
		else if (command.startsWith("_bbsposts;edit;"))
		{
			StringTokenizer st = new StringTokenizer(command, ";");
			st.nextToken();
			st.nextToken();
			int idf = int.Parse(st.nextToken());
			int idt = int.Parse(st.nextToken());
			int idp = int.Parse(st.nextToken());
			showEditPost(TopicBBSManager.getInstance().getTopicByID(idt), ForumsBBSManager.getInstance().getForumByID(idf), player, idp);
		}
		else
		{
			CommunityBoardHandler.separateAndSend("<html><body><br><br><center>the command: " + command + " is not implemented yet</center><br><br></body></html>", player);
		}
	}

	private void showEditPost(Topic? topic, Forum? forum, Player player, int idp)
	{
        Post? p = topic == null ? null : getGPosttByTopic(topic);
		if (forum == null || topic == null || p == null)
		{
			CommunityBoardHandler.separateAndSend("<html><body><br><br><center>Error, this forum, topic or post does not exist!</center><br><br></body></html>", player);
		}
		else
		{
			showHtmlEditPost(topic, player, forum, p);
		}
	}

	private void showPost(Topic? topic, Forum? forum, Player player, int ind)
	{
		if (forum == null || topic == null)
		{
			CommunityBoardHandler.separateAndSend("<html><body><br><br><center>Error: This forum is not implemented yet!</center></body></html>", player);
		}
		else if (forum.getType() == Forum.MEMO)
		{
			showMemoPost(topic, player, forum);
		}
		else
		{
			CommunityBoardHandler.separateAndSend("<html><body><br><br><center>The forum: " + forum.getName() + " is not implemented yet!</center></body></html>", player);
		}
	}

	private void showHtmlEditPost(Topic topic, Player player, Forum forum, Post p)
    {
        Post.CPost? cPost = p.getCPost(0);
		string html = "<html><body><br><br><table border=0 width=610><tr><td width=10></td><td width=600 align=left><a action=\"bypass _bbshome\">HOME</a>&nbsp;>&nbsp;<a action=\"bypass _bbsmemo\">" + forum.getName() + " Form</a></td></tr></table><img src=\"L2UI.squareblank\" width=\"1\" height=\"10\"><center><table border=0 cellspacing=0 cellpadding=0><tr><td width=610><img src=\"sek.cbui355\" width=\"610\" height=\"1\"><br1><img src=\"sek.cbui355\" width=\"610\" height=\"1\"></td></tr></table><table fixwidth=610 border=0 cellspacing=0 cellpadding=0><tr><td><img src=\"l2ui.mini_logo\" width=5 height=20></td></tr><tr><td><img src=\"l2ui.mini_logo\" width=5 height=1></td><td align=center FIXWIDTH=60 height=29>&$413;</td><td FIXWIDTH=540>" + topic.getName() + "</td><td><img src=\"l2ui.mini_logo\" width=5 height=1></td></tr></table><table fixwidth=610 border=0 cellspacing=0 cellpadding=0><tr><td><img src=\"l2ui.mini_logo\" width=5 height=10></td></tr><tr><td><img src=\"l2ui.mini_logo\" width=5 height=1></td><td align=center FIXWIDTH=60 height=29 valign=top>&$427;</td><td align=center FIXWIDTH=540><MultiEdit var =\"Content\" width=535 height=313></td><td><img src=\"l2ui.mini_logo\" width=5 height=1></td></tr><tr><td><img src=\"l2ui.mini_logo\" width=5 height=10></td></tr></table><table fixwidth=610 border=0 cellspacing=0 cellpadding=0><tr><td><img src=\"l2ui.mini_logo\" width=5 height=10></td></tr><tr><td><img src=\"l2ui.mini_logo\" width=5 height=1></td><td align=center FIXWIDTH=60 height=29>&nbsp;</td><td align=center FIXWIDTH=70><button value=\"&$140;\" action=\"Write Post " + forum.getID() + ";" + topic.getID() + ";0 _ Content Content Content\" back=\"l2ui_ch3.smallbutton2_down\" width=65 height=20 fore=\"l2ui_ch3.smallbutton2\" ></td><td align=center FIXWIDTH=70><button value = \"&$141;\" action=\"bypass _bbsmemo\" back=\"l2ui_ch3.smallbutton2_down\" width=65 height=20 fore=\"l2ui_ch3.smallbutton2\"> </td><td align=center FIXWIDTH=400>&nbsp;</td><td><img src=\"l2ui.mini_logo\" width=5 height=1></td></tr></table></center></body></html>";
		Send1001(html, player);
		Send1002(player, cPost?.getPostText() ?? string.Empty, topic.getName(), topic.getDate().ToString("D", CultureInfo.InvariantCulture));
	}

	private void showMemoPost(Topic topic, Player player, Forum forum)
	{
		Post p = getGPosttByTopic(topic);
        Post.CPost? cPost = p.getCPost(0);
		string mes = cPost?.getPostText().Replace(">", "&gt;") ?? string.Empty;
		mes = mes.Replace("<", "&lt;");

		string html =
			"<html><body><br><br><table border=0 width=610><tr><td width=10></td><td width=600 align=left><a action=\"bypass _bbshome\">HOME</a>&nbsp;>&nbsp;<a action=\"bypass _bbsmemo\">Memo Form</a></td></tr></table><img src=\"L2UI.squareblank\" width=\"1\" height=\"10\"><center><table border=0 cellspacing=0 cellpadding=0 bgcolor=333333><tr><td height=10></td></tr><tr><td fixWIDTH=55 align=right valign=top>&$413; : &nbsp;</td><td fixWIDTH=380 valign=top>" +
			topic.getName() +
			"</td><td fixwidth=5></td><td fixwidth=50></td><td fixWIDTH=120></td></tr><tr><td height=10></td></tr><tr><td align=right><font color=\"AAAAAA\" >&$417; : &nbsp;</font></td><td><font color=\"AAAAAA\">" +
			topic.getOwnerName() +
			"</font></td><td></td><td><font color=\"AAAAAA\">&$418; :</font></td><td><font color=\"AAAAAA\">" +
            (cPost?.getPostDate().ToString("D", CultureInfo.InvariantCulture) ?? string.Empty) +
			"</font></td></tr><tr><td height=10></td></tr></table><br><table border=0 cellspacing=0 cellpadding=0><tr><td fixwidth=5></td><td FIXWIDTH=600 align=left>" +
			mes +
			"</td><td fixqqwidth=5></td></tr></table><br><img src=\"L2UI.squareblank\" width=\"1\" height=\"5\"><img src=\"L2UI.squaregray\" width=\"610\" height=\"1\"><img src=\"L2UI.squareblank\" width=\"1\" height=\"5\"><table border=0 cellspacing=0 cellpadding=0 FIXWIDTH=610><tr><td width=50><button value=\"&$422;\" action=\"bypass _bbsmemo\" back=\"l2ui_ch3.smallbutton2_down\" width=65 height=20 fore=\"l2ui_ch3.smallbutton2\"></td><td width=560 align=right><table border=0 cellspacing=0><tr><td FIXWIDTH=300></td><td><button value = \"&$424;\" action=\"bypass _bbsposts;edit;" +
			forum.getID() + ";" + topic.getID() +
			";0\" back=\"l2ui_ch3.smallbutton2_down\" width=65 height=20 fore=\"l2ui_ch3.smallbutton2\" ></td>&nbsp;<td><button value = \"&$425;\" action=\"bypass _bbstopics;del;" +
			forum.getID() + ";" + topic.getID() +
			"\" back=\"l2ui_ch3.smallbutton2_down\" width=65 height=20 fore=\"l2ui_ch3.smallbutton2\" ></td>&nbsp;<td><button value = \"&$421;\" action=\"bypass _bbstopics;crea;" +
			forum.getID() +
			"\" back=\"l2ui_ch3.smallbutton2_down\" width=65 height=20 fore=\"l2ui_ch3.smallbutton2\" ></td>&nbsp;</tr></table></td></tr></table><br><br><br></center></body></html>";
		CommunityBoardHandler.separateAndSend(html, player);
	}

	public override void ParseWrite(string ar1, string ar2, string ar3, string ar4, string ar5, Player player)
	{
		StringTokenizer st = new StringTokenizer(ar1, ";");
		int idf = int.Parse(st.nextToken());
		int idt = int.Parse(st.nextToken());
		int idp = int.Parse(st.nextToken());
		Forum? f = ForumsBBSManager.getInstance().getForumByID(idf);
		if (f == null)
		{
			CommunityBoardHandler.separateAndSend("<html><body><br><br><center>the forum: " + idf + " does not exist !</center><br><br></body></html>", player);
		}
		else
		{
			Topic? t = f.getTopic(idt);
			if (t == null)
			{
				CommunityBoardHandler.separateAndSend("<html><body><br><br><center>the topic: " + idt + " does not exist !</center><br><br></body></html>", player);
			}
			else
			{
				Post p = getGPosttByTopic(t);
				if (p != null)
                {
                    Post.CPost? cPost = p.getCPost(idp);
					if (cPost == null)
					{
						CommunityBoardHandler.separateAndSend("<html><body><br><br><center>the post: " + idp + " does not exist !</center><br><br></body></html>", player);
					}
					else
					{
                        cPost.setPostText(ar4);
						p.updateText(idp);
						ParseCmd("_bbsposts;read;" + f.getID() + ";" + t.getID(), player);
					}
				}
			}
		}
	}

	public static PostBBSManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static PostBBSManager INSTANCE = new PostBBSManager();
	}
}
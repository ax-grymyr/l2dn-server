using L2Dn.GameServer.Model.Interfaces;

namespace L2Dn.GameServer.Model.Announcements;

public interface IAnnouncement : IStorable, IUpdatable, IDeletable
{
    int getId();
	
    AnnouncementType getType();
	
    void setType(AnnouncementType type);
	
    bool isValid();
	
    String getContent();
	
    void setContent(String content);
	
    String getAuthor();
	
    void setAuthor(String author);
}

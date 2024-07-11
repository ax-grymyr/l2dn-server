using L2Dn.GameServer.Model.Interfaces;

namespace L2Dn.GameServer.Model.Announcements;

public interface IAnnouncement : IStorable, IUpdatable, IDeletable
{
    int getId();
	
    AnnouncementType getType();
	
    void setType(AnnouncementType type);
	
    bool isValid();
	
    string getContent();
	
    void setContent(string content);
	
    string getAuthor();
	
    void setAuthor(string author);
}
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Scripts;

namespace L2Dn.GameServer.Model.Announcements;

public class EventAnnouncement: IAnnouncement
{
    private readonly int _id;
    private readonly DateRange _range;
    private string _content;

    public EventAnnouncement(DateRange range, string content)
    {
        _id = IdManager.getInstance().getNextId();
        _range = range;
        _content = content;
    }

    public int getId()
    {
        return _id;
    }

    public AnnouncementType getType()
    {
        return AnnouncementType.EVENT;
    }

    public void setType(AnnouncementType type)
    {
        throw new NotSupportedException();
    }

    public bool isValid()
    {
        return _range.isWithinRange(DateTime.Now);
    }

    public string getContent()
    {
        return _content;
    }

    public void setContent(string content)
    {
        _content = content;
    }

    public string getAuthor()
    {
        return "N/A";
    }

    public void setAuthor(string author)
    {
        throw new NotSupportedException();
    }

    public bool deleteMe()
    {
        IdManager.getInstance().releaseId(_id);
        return true;
    }

    public bool storeMe()
    {
        return true;
    }

    public bool updateMe()
    {
        throw new NotSupportedException();
    }
}
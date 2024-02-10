namespace L2Dn.GameServer.Model.Announcements;

public class EventAnnouncement: IAnnouncement
{
    private readonly int _id;
    private readonly DateRange _range;
    private String _content;

    public EventAnnouncement(DateRange range, String content)
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
        return _range.isWithinRange(new Date());
    }

    public String getContent()
    {
        return _content;
    }

    public void setContent(String content)
    {
        _content = content;
    }

    public String getAuthor()
    {
        return "N/A";
    }

    public void setAuthor(String author)
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
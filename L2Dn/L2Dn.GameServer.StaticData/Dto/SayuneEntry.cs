using System.Collections.Immutable;
using L2Dn.GameServer.StaticData.Xml.Sayune;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Dto;

public sealed record SayuneEntry(int Id, bool IsSelector, Location3D Location, ImmutableArray<SayuneEntry> InnerEntries)
{
    public SayuneEntry(XmlSayuneItem xmlSayuneItem): this(xmlSayuneItem.Id, false, default,
        CreateInnerEntries(xmlSayuneItem))
    {
    }

    private SayuneEntry(XmlSayuneSelector xmlSayuneSelector): this(xmlSayuneSelector.Id, false,
        new Location3D(xmlSayuneSelector.X, xmlSayuneSelector.Y, xmlSayuneSelector.Z),
        CreateInnerEntries(xmlSayuneSelector))
    {
    }

    private SayuneEntry(XmlSayuneSelectorChoice xmlSayuneSelectorChoice): this(xmlSayuneSelectorChoice.Id, false,
        new Location3D(xmlSayuneSelectorChoice.X, xmlSayuneSelectorChoice.Y, xmlSayuneSelectorChoice.Z),
        CreateInnerEntries(xmlSayuneSelectorChoice))
    {
    }

    private SayuneEntry(XmlSayuneLocation xmlSayuneLocation): this(xmlSayuneLocation.Id, false,
        new Location3D(xmlSayuneLocation.X, xmlSayuneLocation.Y, xmlSayuneLocation.Z),
        ImmutableArray<SayuneEntry>.Empty)
    {
    }

    private static ImmutableArray<SayuneEntry> CreateInnerEntries(XmlSayuneItem xmlSayuneItem)
    {
        List<SayuneEntry> list = xmlSayuneItem.Locations.Select(x => new SayuneEntry(x)).ToList();
        if (xmlSayuneItem.Selector is not null)
            list.Add(new SayuneEntry(xmlSayuneItem.Selector));

        return list.ToImmutableArray();
    }

    private static ImmutableArray<SayuneEntry> CreateInnerEntries(XmlSayuneSelectorChoice xmlSayuneSelectorChoice)
    {
        List<SayuneEntry> list = xmlSayuneSelectorChoice.Locations.Select(x => new SayuneEntry(x)).ToList();
        if (xmlSayuneSelectorChoice.Selector is not null)
            list.Add(new SayuneEntry(xmlSayuneSelectorChoice.Selector));

        return list.ToImmutableArray();
    }

    private static ImmutableArray<SayuneEntry> CreateInnerEntries(XmlSayuneSelector xmlSayuneSelector) =>
        xmlSayuneSelector.Choices.Select(x => new SayuneEntry(x)).ToImmutableArray();
}
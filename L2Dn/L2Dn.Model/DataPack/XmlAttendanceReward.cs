﻿using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlAttendanceReward
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("count")]
    public int Count { get; set; }
}
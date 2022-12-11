using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FirLib.Formats.Gpx;

public class GpxTrackOrRoute
{
    [XmlElement("name")]
    public string? Name { get; set; }

    [XmlElement("cmt")]
    public string? Comment { get; set; }

    [XmlElement("desc")]
    public string? Description { get; set; }

    [XmlElement("src")]
    public string? Source { get; set; }

    [XmlElement("link")]
    public List<string> Links { get; } = new();

    [XmlElement("number")]
    public int? GpsNumber { get; set; }

    [XmlIgnore]
    public bool GpsNumberSpecified => this.GpsNumber.HasValue;

    [XmlElement("type")]
    public string? Type { get; set; }

    [XmlElement("extensions")]
    public GpxExtensions? Extensions { get; set; }
}
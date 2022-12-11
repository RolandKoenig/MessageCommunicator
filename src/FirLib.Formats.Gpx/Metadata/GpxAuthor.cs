using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FirLib.Formats.Gpx.Metadata;

public class GpxAuthor
{
    [XmlElement("name")]
    public string? Name { get; set; }

    [XmlElement("email")]
    public string? EMail { get; set; }

    [XmlElement("link")]
    public GpxLink? Link { get; set; }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FirLib.Formats.Gpx.Metadata;

public class GpxBounds
{
    [XmlAttribute("minlat")]
    public double MinLatitude { get; set; }

    [XmlAttribute("minlon")]
    public double MinLongitude { get; set; }

    [XmlAttribute("maxlat")]
    public double MaxLatitude { get; set; }

    [XmlAttribute("maxlon")]
    public double MaxLongitude { get; set; }
}
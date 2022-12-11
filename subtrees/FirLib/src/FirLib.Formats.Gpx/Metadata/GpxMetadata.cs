using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FirLib.Formats.Gpx.Metadata;

public class GpxMetadata
{
    [XmlElement("name")]
    public string? Name { get; set; }

    [XmlElement("description")]
    public string? Description { get; set; }

    [XmlElement("author")]
    public GpxAuthor? Author { get; set; }

    [XmlElement("copyright")]
    public GpxCopyright? Copyright { get; set; }

    [XmlElement("link")]
    public GpxLink? Link { get; set; }

    [XmlElement("time")]
    public DateTime? Time { get; set; }

    [XmlIgnore]
    public bool TimeSpecified => this.Time.HasValue;

    [XmlElement("keywords")]
    public string? Keywords { get; set; }

    [XmlElement("bounds")]
    public GpxBounds? Bounds { get; set; }

    [XmlElement("extensions")]
    public GpxExtensions? Extensions { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Metadata: Name={this.Name}, Description={this.Description}";
    }
}
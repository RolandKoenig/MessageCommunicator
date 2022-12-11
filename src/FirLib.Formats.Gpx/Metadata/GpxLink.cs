using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FirLib.Formats.Gpx.Metadata;

public class GpxLink
{
    [XmlAttribute("href")]
    public string HRef { get; set; } = string.Empty;

    [XmlElement("text")]
    public string? Text { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Link: HRef={this.HRef}, Text={this.Text}";
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FirLib.Formats.Gpx.Metadata;

public class GpxEMail
{
    [XmlAttribute("id")]
    public string ID { get; set; } = string.Empty;

    [XmlAttribute("domain")]
    public string Domain { get; set; } = string.Empty;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"EMail: ID={this.ID}, Domain={this.Domain}";
    }
}
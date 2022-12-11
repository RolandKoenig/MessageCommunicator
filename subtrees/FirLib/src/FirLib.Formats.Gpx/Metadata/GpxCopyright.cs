using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FirLib.Formats.Gpx.Metadata;

public class GpxCopyright
{
    [XmlAttribute("author")]
    public string Author { get; set; } = string.Empty;

    [XmlElement("year")]
    public int? Year { get; set; }

    [XmlIgnore]
    public bool YearSpecified => this.Year.HasValue;

    [XmlElement("license")]
    public string? License { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Copyright: Author={this.Author}";
    }
}
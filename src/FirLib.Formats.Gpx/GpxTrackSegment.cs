using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FirLib.Formats.Gpx;

public class GpxTrackSegment
{
    [XmlElement("trkpt")]
    public List<GpxWaypoint> Points { get; } = new();

    [XmlElement("extensions")]
    public GpxExtensions? Extensions { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"TrackSegment: PointCount={this.Points.Count}";
    }
}
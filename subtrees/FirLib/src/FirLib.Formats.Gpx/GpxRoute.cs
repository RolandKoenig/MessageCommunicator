using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FirLib.Formats.Gpx
{
    public class GpxRoute : GpxTrackOrRoute
    {
        [XmlElement("rtept")]
        public List<GpxWaypoint> RoutePoints { get; } = new();

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Route: Name={this.Name}, PointCount={this.RoutePoints.Count}";
        }
    }
}

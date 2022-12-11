using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FirLib.Formats.Gpx;

public class GpxWaypoint
{
    [XmlAttribute("lat")]
    public double Latitude { get; set; }

    [XmlAttribute("lon")]
    public double Longitude { get; set; }

    [XmlElement("ele")]
    public double? Elevation { get; set; }

    [XmlIgnore]
    public bool ElevationSpecified => this.Elevation.HasValue;

    [XmlElement("time")]
    public DateTime? Time { get; set; }

    [XmlIgnore]
    public bool TimeSpecified => this.Time.HasValue;

    [XmlElement("magvar")]
    public double? MagneticVariationDec { get; set; }

    [XmlIgnore]
    public bool MagneticVariationDecSpecified => this.MagneticVariationDec.HasValue;

    [XmlElement("geoidheight")]
    public double? GeoidHeightMeters { get; set; }

    [XmlIgnore]
    public bool GeoidHeightMetersSpecified => this.GeoidHeightMeters.HasValue;

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

    [XmlElement("sym")]
    public string? GpsSymbol { get; set; }

    [XmlElement("type")]
    public string? Type { get; set; }

    [XmlElement("fix")]
    public string? GpsFixType { get; set; }

    [XmlElement("sat")]
    public int? NumberOfSatellites { get; set; }

    [XmlIgnore]
    public bool NumberOfSatellitesSpecified => this.NumberOfSatellites.HasValue;

    [XmlElement("hdop")]
    public double? HorizontalDilution { get; set; }

    [XmlIgnore]
    public bool HorizontalDilutionSpecified => this.HorizontalDilution.HasValue;

    [XmlElement("vdop")]
    public double? VerticalDilution { get; set; }

    [XmlIgnore]
    public bool VerticalDilutionSpecified => this.VerticalDilution.HasValue;

    [XmlElement("pdop")]
    public double? PositionDilution { get; set; }

    [XmlIgnore]
    public bool PositionDilutionSpecified => this.PositionDilution.HasValue;

    [XmlElement("ageofdgpsdata")]
    public double? SecondsSinceLastDgpsUpdate { get; set; }

    [XmlIgnore]
    public bool SecondsSinceLastDgpsUpdateSpecified => this.SecondsSinceLastDgpsUpdate.HasValue;

    [XmlElement("dgpsid")]
    public int? DgpsStationID { get; set; }

    [XmlIgnore]
    public bool DgpsStationIDSpecified => this.DgpsStationID.HasValue;

    [XmlElement("extensions")]
    public GpxExtensions? Extensions
    {
        get;
        set;
    }

    public GpxWaypoint()
    {

    }

    public GpxWaypoint(double latitude, double longitude, double elevation)
    {
        this.Latitude = latitude;
        this.Longitude = longitude;
        this.Elevation = elevation;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Waypoint: Name={this.Name}, Latitude={this.Latitude}, Longitude={this.Longitude}";
    }
}
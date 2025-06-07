using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace EcoSphere.Models;
public partial class VwObservation
{
    [Column("ID")]
    public int Id { get; set; }
    [Column("scientific_name")]
    public string? ScientificName { get; set; }
    [Column("username")]
    public string? Username { get; set; }
    [Column("province_name")]
    public string? ProvinceName { get; set; }
    [Column("district_name")]
    public string? DistrictName { get; set; }
    [Column("endemic_status")]
    public string? EndemicStatus { get; set; }
    [Column("project_name")]
    public string? ProjectName { get; set; }
    [Column("reference_name")]
    public string? ReferenceName { get; set; }
    [Column("location_type")]
    public string? LocationType { get; set; }
    [Column("gender_name")]
    public string? GenderName { get; set; }
    [Column("lat")]
    public string? Lat { get; set; }
    [Column("long")]
    public string? Long { get; set; }
    [Column("activity")]
    public string? Activity { get; set; }
    [Column("seen_time")]
    public DateTime? SeenTime { get; set; }
    [Column("creation_date")]
    public DateTime? CreationDate { get; set; }
}


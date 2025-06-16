using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace EcoSphere.Models;

public partial class VwMap
{
    [Column("ID")]
    public int Id { get; set; }
    [Column("lat")]
    public string? Lat { get; set; }

    [Column("long")]
    public string? Long { get; set; }
    [Column("scientific_name")]
    public string? ScientificName { get; set; }
    [Column("kingdom_name")]
    public string? KingdomName { get; set; }
    [Column("seen_time")]
    public DateTime? SeenTime { get; set; }
    [Column("District_ID")]
    public int DistrictId { get; set; }
    [Column("Province_ID")]
    public int ProvinceId { get; set; }
    [Column("province_name")]
    public string? ProvinceName { get; set; }
    [Column("district_name")]
    public string? DistrictName { get; set; }
    [Column("ImagePath")]
    public string? ImagePath { get; set; }
    [Column("Creature_ID")]
    public int CreatureId { get; set; }
    [Column("Endemic_status_ID")]
    public int EndemicstatID { get; set; }
    [Column("User_ID")]
    public int UserId { get; set; }
    [Column("username")]
    public string? Username { get; set; }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoSphere.Models;

public partial class TblMaintable
{
    //[Key]
    //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //[Column("ID")]  // veritabanındaki gerçek sütun adı
    public int Id { get; set; }

    public int? CreatureId { get; set; }
    
    public int? UserId { get; set; }

    public int? RegionId { get; set; }

    public int? CityId { get; set; }

    public int? DistrictId { get; set; }

    public int? LocalityId { get; set; }

    public int? NeighborhoodId { get; set; }

    public int? MigrationStatusId { get; set; }

    public int? EndemicStatusId { get; set; }

    public int? ProjectId { get; set; }

    public int? CitationId { get; set; }

    public int? ReferenceId { get; set; }

    public int? LocationTypeId { get; set; }

    public int? LocationRangeId { get; set; }

    public int? GenderId { get; set; }

    public string? Long { get; set; }

    public string? Lat { get; set; }

    public string? Activity { get; set; }

    public DateTime? SeenTime { get; set; }

    public DateTime? CreationDate { get; set; }

    public virtual TblCitation? Citation { get; set; }

    public virtual TblProvince? City { get; set; }

    public virtual TblCreature? Creature { get; set; }

    public virtual TblDistrict? District { get; set; }

    public virtual TblEndemicstatus? EndemicStatus { get; set; }

    public virtual TblGender? Gender { get; set; }

    public virtual TblLocationrange? LocationRange { get; set; }

    public virtual TblLocationtype? LocationType { get; set; }

    public virtual TblMigrationstatus? MigrationStatus { get; set; }

    public virtual TblNeighbourhood? Neighborhood { get; set; }

    public virtual TblProject? Project { get; set; }

    public virtual TblReference? Reference { get; set; }

    public virtual TblRegion? Region { get; set; }

    public virtual TblUser? User { get; set; }
}

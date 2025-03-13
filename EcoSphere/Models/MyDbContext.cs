using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EcoSphere.Models;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblCitation> TblCitations { get; set; }

    public virtual DbSet<TblClass> TblClasses { get; set; }

    public virtual DbSet<TblCreature> TblCreatures { get; set; }

    public virtual DbSet<TblDistrict> TblDistricts { get; set; }

    public virtual DbSet<TblEndemicstatus> TblEndemicstatuses { get; set; }

    public virtual DbSet<TblFamily> TblFamilies { get; set; }

    public virtual DbSet<TblGender> TblGenders { get; set; }

    public virtual DbSet<TblGenu> TblGenus { get; set; }

    public virtual DbSet<TblIucn> TblIucns { get; set; }

    public virtual DbSet<TblKingdom> TblKingdoms { get; set; }

    public virtual DbSet<TblLocality> TblLocalities { get; set; }

    public virtual DbSet<TblLocationrange> TblLocationranges { get; set; }

    public virtual DbSet<TblLocationtype> TblLocationtypes { get; set; }

    public virtual DbSet<TblMaintable> TblMaintables { get; set; }

    public virtual DbSet<TblMigrationstatus> TblMigrationstatuses { get; set; }

    public virtual DbSet<TblNeighbourhood> TblNeighbourhoods { get; set; }

    public virtual DbSet<TblOrder> TblOrders { get; set; }

    public virtual DbSet<TblPhylum> TblPhylums { get; set; }

    public virtual DbSet<TblProject> TblProjects { get; set; }

    public virtual DbSet<TblProvince> TblProvinces { get; set; }

    public virtual DbSet<TblReference> TblReferences { get; set; }

    public virtual DbSet<TblRegion> TblRegions { get; set; }

    public virtual DbSet<TblRole> TblRoles { get; set; }

    public virtual DbSet<TblSpeciesauthor> TblSpeciesauthors { get; set; }

    public virtual DbSet<TblSpecy> TblSpecies { get; set; }

    public virtual DbSet<TblSubspecy> TblSubspecies { get; set; }

    public virtual DbSet<TblUpperrealm> TblUpperrealms { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    public virtual DbSet<TblUserRole> TblUserRoles { get; set; }

    public virtual DbSet<TblUseraction> TblUseractions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=ConnectionStrings:MyDbContext");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblCitation>(entity =>
        {
            entity.HasKey(e => e.CitationId);

            entity.ToTable("tbl_citation");

            entity.Property(e => e.CitationId).HasColumnName("Citation_ID");
            entity.Property(e => e.CitationName)
                .HasMaxLength(50)
                .HasColumnName("citation_name");
        });

        modelBuilder.Entity<TblClass>(entity =>
        {
            entity.HasKey(e => e.ClassId);

            entity.ToTable("tbl_class");

            entity.Property(e => e.ClassId).HasColumnName("Class_ID");
            entity.Property(e => e.ClassName)
                .HasMaxLength(50)
                .HasColumnName("class_name");
            entity.Property(e => e.PhylumId).HasColumnName("Phylum_ID");
            entity.Property(e => e.ScientificName)
                .HasMaxLength(50)
                .HasColumnName("scientific_name");

            entity.HasOne(d => d.Phylum).WithMany(p => p.TblClasses)
                .HasForeignKey(d => d.PhylumId)
                .HasConstraintName("FK_tbl_class_tbl_phylum");
        });

        modelBuilder.Entity<TblCreature>(entity =>
        {
            entity.HasKey(e => e.CreatureId);

            entity.ToTable("tbl_creatures");

            entity.Property(e => e.CreatureId).HasColumnName("Creature_ID");
            entity.Property(e => e.AuthorId).HasColumnName("Author_ID");
            entity.Property(e => e.ClassId).HasColumnName("Class_ID");
            entity.Property(e => e.CommonName)
                .HasMaxLength(50)
                .HasColumnName("common_name");
            entity.Property(e => e.FamilyId).HasColumnName("Family_ID");
            entity.Property(e => e.GenusId).HasColumnName("Genus_ID");
            entity.Property(e => e.IucnId).HasColumnName("IUCN_ID");
            entity.Property(e => e.KingdomId).HasColumnName("Kingdom_ID");
            entity.Property(e => e.OrderId).HasColumnName("Order_ID");
            entity.Property(e => e.PhylumId).HasColumnName("Phylum_ID");
            entity.Property(e => e.ScientificName)
                .HasMaxLength(50)
                .HasColumnName("scientific_name");
            entity.Property(e => e.SpeciesId).HasColumnName("Species_ID");
            entity.Property(e => e.SubspeciesId).HasColumnName("subspecies_ID");
            entity.Property(e => e.UpperRealmId).HasColumnName("Upper_realm_ID");

            entity.HasOne(d => d.Author).WithMany(p => p.TblCreatures)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK_tbl_creatures_tbl_speciesauthor");

            entity.HasOne(d => d.Class).WithMany(p => p.TblCreatures)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK_tbl_creatures_tbl_class");

            entity.HasOne(d => d.Family).WithMany(p => p.TblCreatures)
                .HasForeignKey(d => d.FamilyId)
                .HasConstraintName("FK_tbl_creatures_tbl_family");

            entity.HasOne(d => d.Genus).WithMany(p => p.TblCreatures)
                .HasForeignKey(d => d.GenusId)
                .HasConstraintName("FK_tbl_creatures_tbl_genus");

            entity.HasOne(d => d.Iucn).WithMany(p => p.TblCreatures)
                .HasForeignKey(d => d.IucnId)
                .HasConstraintName("FK_tbl_creatures_tbl_IUCN");

            entity.HasOne(d => d.Kingdom).WithMany(p => p.TblCreatures)
                .HasForeignKey(d => d.KingdomId)
                .HasConstraintName("FK_tbl_creatures_tbl_kingdom");

            entity.HasOne(d => d.Order).WithMany(p => p.TblCreatures)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_tbl_creatures_tbl_order");

            entity.HasOne(d => d.Phylum).WithMany(p => p.TblCreatures)
                .HasForeignKey(d => d.PhylumId)
                .HasConstraintName("FK_tbl_creatures_tbl_phylum");

            entity.HasOne(d => d.Species).WithMany(p => p.TblCreatures)
                .HasForeignKey(d => d.SpeciesId)
                .HasConstraintName("FK_tbl_creatures_tbl_species");

            entity.HasOne(d => d.Subspecies).WithMany(p => p.TblCreatures)
                .HasForeignKey(d => d.SubspeciesId)
                .HasConstraintName("FK_tbl_creatures_tbl_subspecies");

            entity.HasOne(d => d.UpperRealm).WithMany(p => p.TblCreatures)
                .HasForeignKey(d => d.UpperRealmId)
                .HasConstraintName("FK_tbl_creatures_tbl_upperrealm");
        });

        modelBuilder.Entity<TblDistrict>(entity =>
        {
            entity.HasKey(e => e.DistrictId);

            entity.ToTable("tbl_district");

            entity.Property(e => e.DistrictId)
                .ValueGeneratedNever()
                .HasColumnName("District_ID");
            entity.Property(e => e.DistrictName)
                .HasMaxLength(50)
                .HasColumnName("district_name");
            entity.Property(e => e.ProvinceId).HasColumnName("Province_ID");

            entity.HasOne(d => d.Province).WithMany(p => p.TblDistricts)
                .HasForeignKey(d => d.ProvinceId)
                .HasConstraintName("FK_tbl_district_tbl_province");
        });

        modelBuilder.Entity<TblEndemicstatus>(entity =>
        {
            entity.HasKey(e => e.EndemicStatusId);

            entity.ToTable("tbl_endemicstatus");

            entity.Property(e => e.EndemicStatusId).HasColumnName("Endemic_status_ID");
            entity.Property(e => e.EndemicStatus)
                .HasMaxLength(50)
                .HasColumnName("endemic_status");
        });

        modelBuilder.Entity<TblFamily>(entity =>
        {
            entity.HasKey(e => e.FamilyId);

            entity.ToTable("tbl_family");

            entity.Property(e => e.FamilyId).HasColumnName("Family_ID");
            entity.Property(e => e.FamilyName)
                .HasMaxLength(50)
                .HasColumnName("family_name");
            entity.Property(e => e.OrderId).HasColumnName("Order_ID");
            entity.Property(e => e.ScientificName)
                .HasMaxLength(50)
                .HasColumnName("scientific_name");

            entity.HasOne(d => d.Order).WithMany(p => p.TblFamilies)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_tbl_family_tbl_order");
        });

        modelBuilder.Entity<TblGender>(entity =>
        {
            entity.HasKey(e => e.GenderId);

            entity.ToTable("tbl_gender");

            entity.Property(e => e.GenderId).HasColumnName("Gender_ID");
            entity.Property(e => e.GenderName)
                .HasMaxLength(50)
                .HasColumnName("gender_name");
        });

        modelBuilder.Entity<TblGenu>(entity =>
        {
            entity.HasKey(e => e.GenusId).HasName("PK_tbl_Genus");

            entity.ToTable("tbl_genus");

            entity.Property(e => e.GenusId).HasColumnName("Genus_ID");
            entity.Property(e => e.FamilyId).HasColumnName("Family_ID");
            entity.Property(e => e.GenusName)
                .HasMaxLength(50)
                .HasColumnName("genus_name");
            entity.Property(e => e.ScientificName)
                .HasMaxLength(50)
                .HasColumnName("scientific_name");

            entity.HasOne(d => d.Family).WithMany(p => p.TblGenus)
                .HasForeignKey(d => d.FamilyId)
                .HasConstraintName("FK_tbl_genus_tbl_family");
        });

        modelBuilder.Entity<TblIucn>(entity =>
        {
            entity.HasKey(e => e.IucnId);

            entity.ToTable("tbl_IUCN");

            entity.Property(e => e.IucnId).HasColumnName("IUCN_ID");
            entity.Property(e => e.IucnCode)
                .HasMaxLength(50)
                .HasColumnName("IUCN_code");
            entity.Property(e => e.IucnName)
                .HasMaxLength(50)
                .HasColumnName("IUCN_name");
        });

        modelBuilder.Entity<TblKingdom>(entity =>
        {
            entity.HasKey(e => e.KingdomId);

            entity.ToTable("tbl_kingdom");

            entity.Property(e => e.KingdomId).HasColumnName("Kingdom_ID");
            entity.Property(e => e.KingdomName)
                .HasMaxLength(50)
                .HasColumnName("kingdom_name");
            entity.Property(e => e.RealmId).HasColumnName("Realm_ID");
            entity.Property(e => e.ScientificName)
                .HasMaxLength(50)
                .HasColumnName("scientific_name");

            entity.HasOne(d => d.Realm).WithMany(p => p.TblKingdoms)
                .HasForeignKey(d => d.RealmId)
                .HasConstraintName("FK_tbl_kingdom_tbl_upperrealm");
        });

        modelBuilder.Entity<TblLocality>(entity =>
        {
            entity.HasKey(e => e.LocalityId);

            entity.ToTable("tbl_locality");

            entity.Property(e => e.LocalityId)
                .ValueGeneratedNever()
                .HasColumnName("Locality_ID");
            entity.Property(e => e.DistrictId).HasColumnName("District_ID");
            entity.Property(e => e.LocalityName)
                .HasMaxLength(50)
                .HasColumnName("locality_name");

            entity.HasOne(d => d.District).WithMany(p => p.TblLocalities)
                .HasForeignKey(d => d.DistrictId)
                .HasConstraintName("FK_tbl_locality_tbl_district");
        });

        modelBuilder.Entity<TblLocationrange>(entity =>
        {
            entity.HasKey(e => e.LocationRangeId);

            entity.ToTable("tbl_locationrange");

            entity.Property(e => e.LocationRangeId).HasColumnName("Location_Range_ID");
            entity.Property(e => e.LocationRangeValue)
                .HasMaxLength(50)
                .HasColumnName("location_range_value");
        });

        modelBuilder.Entity<TblLocationtype>(entity =>
        {
            entity.HasKey(e => e.LocationTypeId);

            entity.ToTable("tbl_locationtype");

            entity.Property(e => e.LocationTypeId).HasColumnName("Location_type_ID");
            entity.Property(e => e.LocationType)
                .HasMaxLength(50)
                .HasColumnName("location_type");
        });

        modelBuilder.Entity<TblMaintable>(entity =>
        {
            entity.ToTable("tbl_maintable");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Activity)
                .HasMaxLength(50)
                .HasColumnName("activity");
            entity.Property(e => e.CitationId).HasColumnName("Citation_ID");
            entity.Property(e => e.CityId).HasColumnName("City_ID");
            entity.Property(e => e.CreationDate)
                .HasColumnType("datetime")
                .HasColumnName("creation_date");
            entity.Property(e => e.CreatureId).HasColumnName("Creature_ID");
            entity.Property(e => e.DistrictId).HasColumnName("District_ID");
            entity.Property(e => e.EndemicStatusId).HasColumnName("Endemic_status_ID");
            entity.Property(e => e.GenderId).HasColumnName("Gender_ID");
            entity.Property(e => e.Lat)
                .HasMaxLength(50)
                .HasColumnName("lat");
            entity.Property(e => e.LocalityId).HasColumnName("Locality_ID");
            entity.Property(e => e.LocationRangeId).HasColumnName("Location_range_ID");
            entity.Property(e => e.LocationTypeId).HasColumnName("Location_type_ID");
            entity.Property(e => e.Long)
                .HasMaxLength(50)
                .HasColumnName("long");
            entity.Property(e => e.MigrationStatusId).HasColumnName("Migration_status_ID");
            entity.Property(e => e.NeighborhoodId).HasColumnName("Neighborhood_ID");
            entity.Property(e => e.ProjectId).HasColumnName("Project_ID");
            entity.Property(e => e.ReferenceId).HasColumnName("Reference_ID");
            entity.Property(e => e.RegionId).HasColumnName("Region_ID");
            entity.Property(e => e.SeenTime)
                .HasColumnType("datetime")
                .HasColumnName("seen_time");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.Citation).WithMany(p => p.TblMaintables)
                .HasForeignKey(d => d.CitationId)
                .HasConstraintName("FK_tbl_maintable_tbl_citation");

            entity.HasOne(d => d.City).WithMany(p => p.TblMaintables)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_tbl_maintable_tbl_province");

            entity.HasOne(d => d.Creature).WithMany(p => p.TblMaintables)
                .HasForeignKey(d => d.CreatureId)
                .HasConstraintName("FK_tbl_maintable_tbl_creatures");

            entity.HasOne(d => d.District).WithMany(p => p.TblMaintables)
                .HasForeignKey(d => d.DistrictId)
                .HasConstraintName("FK_tbl_maintable_tbl_district");

            entity.HasOne(d => d.EndemicStatus).WithMany(p => p.TblMaintables)
                .HasForeignKey(d => d.EndemicStatusId)
                .HasConstraintName("FK_tbl_maintable_tbl_endemicstatus");

            entity.HasOne(d => d.Gender).WithMany(p => p.TblMaintables)
                .HasForeignKey(d => d.GenderId)
                .HasConstraintName("FK_tbl_maintable_tbl_gender");

            entity.HasOne(d => d.LocationRange).WithMany(p => p.TblMaintables)
                .HasForeignKey(d => d.LocationRangeId)
                .HasConstraintName("FK_tbl_maintable_tbl_locationrange");

            entity.HasOne(d => d.LocationType).WithMany(p => p.TblMaintables)
                .HasForeignKey(d => d.LocationTypeId)
                .HasConstraintName("FK_tbl_maintable_tbl_locationtype");

            entity.HasOne(d => d.MigrationStatus).WithMany(p => p.TblMaintables)
                .HasForeignKey(d => d.MigrationStatusId)
                .HasConstraintName("FK_tbl_maintable_tbl_migrationstatus");

            entity.HasOne(d => d.Neighborhood).WithMany(p => p.TblMaintables)
                .HasForeignKey(d => d.NeighborhoodId)
                .HasConstraintName("FK_tbl_maintable_tbl_neighbourhood");

            entity.HasOne(d => d.Project).WithMany(p => p.TblMaintables)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK_tbl_maintable_tbl_project");

            entity.HasOne(d => d.Reference).WithMany(p => p.TblMaintables)
                .HasForeignKey(d => d.ReferenceId)
                .HasConstraintName("FK_tbl_maintable_tbl_reference");

            entity.HasOne(d => d.Region).WithMany(p => p.TblMaintables)
                .HasForeignKey(d => d.RegionId)
                .HasConstraintName("FK_tbl_maintable_tbl_region");

            entity.HasOne(d => d.User).WithMany(p => p.TblMaintables)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_tbl_maintable_tbl_user");
        });

        modelBuilder.Entity<TblMigrationstatus>(entity =>
        {
            entity.HasKey(e => e.MigrationStatusId);

            entity.ToTable("tbl_migrationstatus");

            entity.Property(e => e.MigrationStatusId).HasColumnName("Migration_status_ID");
            entity.Property(e => e.MigrationStatusName)
                .HasMaxLength(50)
                .HasColumnName("migration_status_name");
        });

        modelBuilder.Entity<TblNeighbourhood>(entity =>
        {
            entity.HasKey(e => e.NeighbourhoodId);

            entity.ToTable("tbl_neighbourhood");

            entity.Property(e => e.NeighbourhoodId)
                .ValueGeneratedNever()
                .HasColumnName("Neighbourhood_ID");
            entity.Property(e => e.HoodName)
                .HasMaxLength(50)
                .HasColumnName("hood_name");
            entity.Property(e => e.LocalityId).HasColumnName("Locality_ID");

            entity.HasOne(d => d.Locality).WithMany(p => p.TblNeighbourhoods)
                .HasForeignKey(d => d.LocalityId)
                .HasConstraintName("FK_tbl_neighbourhood_tbl_locality");
        });

        modelBuilder.Entity<TblOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId);

            entity.ToTable("tbl_order");

            entity.Property(e => e.OrderId).HasColumnName("Order_ID");
            entity.Property(e => e.ClassId).HasColumnName("Class_ID");
            entity.Property(e => e.OrderName)
                .HasMaxLength(50)
                .HasColumnName("order_name");
            entity.Property(e => e.ScientificName)
                .HasMaxLength(50)
                .HasColumnName("scientific_name");

            entity.HasOne(d => d.Class).WithMany(p => p.TblOrders)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK_tbl_order_tbl_class");
        });

        modelBuilder.Entity<TblPhylum>(entity =>
        {
            entity.HasKey(e => e.PhylumId);

            entity.ToTable("tbl_phylum");

            entity.Property(e => e.PhylumId).HasColumnName("Phylum_ID");
            entity.Property(e => e.KingdomId).HasColumnName("Kingdom_ID");
            entity.Property(e => e.PhylumName)
                .HasMaxLength(50)
                .HasColumnName("phylum_name");
            entity.Property(e => e.ScientificName)
                .HasMaxLength(50)
                .HasColumnName("scientific_name");

            entity.HasOne(d => d.Kingdom).WithMany(p => p.TblPhylums)
                .HasForeignKey(d => d.KingdomId)
                .HasConstraintName("FK_tbl_phylum_tbl_kingdom");
        });

        modelBuilder.Entity<TblProject>(entity =>
        {
            entity.HasKey(e => e.ProjectId);

            entity.ToTable("tbl_project");

            entity.Property(e => e.ProjectId).HasColumnName("Project_ID");
            entity.Property(e => e.ProjectName)
                .HasMaxLength(50)
                .HasColumnName("project_name");
        });

        modelBuilder.Entity<TblProvince>(entity =>
        {
            entity.HasKey(e => e.ProvinceId);

            entity.ToTable("tbl_province");

            entity.Property(e => e.ProvinceId)
                .ValueGeneratedNever()
                .HasColumnName("Province_ID");
            entity.Property(e => e.ProvinceName)
                .HasMaxLength(50)
                .HasColumnName("province_name");
            entity.Property(e => e.RegionId).HasColumnName("Region_ID");

            entity.HasOne(d => d.Region).WithMany(p => p.TblProvinces)
                .HasForeignKey(d => d.RegionId)
                .HasConstraintName("FK_tbl_province_tbl_region");
        });

        modelBuilder.Entity<TblReference>(entity =>
        {
            entity.HasKey(e => e.ReferenceId);

            entity.ToTable("tbl_reference");

            entity.Property(e => e.ReferenceId).HasColumnName("Reference_ID");
            entity.Property(e => e.ReferenceName)
                .HasMaxLength(50)
                .HasColumnName("reference_name");
        });

        modelBuilder.Entity<TblRegion>(entity =>
        {
            entity.HasKey(e => e.RegionId);

            entity.ToTable("tbl_region");

            entity.Property(e => e.RegionId)
                .ValueGeneratedNever()
                .HasColumnName("Region_ID");
            entity.Property(e => e.RegionName)
                .HasMaxLength(50)
                .HasColumnName("region_name");
        });

        modelBuilder.Entity<TblRole>(entity =>
        {
            entity.HasKey(e => e.RoleId);

            entity.ToTable("tbl_role");

            entity.Property(e => e.RoleId).HasColumnName("Role_ID");
            entity.Property(e => e.CreationDate)
                .HasColumnType("datetime")
                .HasColumnName("creation_date");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");
        });

        modelBuilder.Entity<TblSpeciesauthor>(entity =>
        {
            entity.HasKey(e => e.AuthorId);

            entity.ToTable("tbl_speciesauthor");

            entity.Property(e => e.AuthorId).HasColumnName("Author_ID");
            entity.Property(e => e.AuthorName)
                .HasMaxLength(50)
                .HasColumnName("author_name");
        });

        modelBuilder.Entity<TblSpecy>(entity =>
        {
            entity.HasKey(e => e.SpeciesId);

            entity.ToTable("tbl_species");

            entity.Property(e => e.SpeciesId).HasColumnName("Species_ID");
            entity.Property(e => e.GenusId).HasColumnName("Genus_ID");
            entity.Property(e => e.ScientificName)
                .HasMaxLength(50)
                .HasColumnName("scientific_name");
            entity.Property(e => e.SpeciesName)
                .HasMaxLength(50)
                .HasColumnName("species_name");

            entity.HasOne(d => d.Genus).WithMany(p => p.TblSpecies)
                .HasForeignKey(d => d.GenusId)
                .HasConstraintName("FK_tbl_species_tbl_genus");
        });

        modelBuilder.Entity<TblSubspecy>(entity =>
        {
            entity.HasKey(e => e.SubspeciesId);

            entity.ToTable("tbl_subspecies");

            entity.Property(e => e.SubspeciesId).HasColumnName("Subspecies_ID");
            entity.Property(e => e.ScientificName)
                .HasMaxLength(50)
                .HasColumnName("scientific_name");
            entity.Property(e => e.SubspeciesName)
                .HasMaxLength(50)
                .HasColumnName("subspecies_name");
        });

        modelBuilder.Entity<TblUpperrealm>(entity =>
        {
            entity.HasKey(e => e.RealmId);

            entity.ToTable("tbl_upperrealm");

            entity.Property(e => e.RealmId).HasColumnName("Realm_ID");
            entity.Property(e => e.RealmName)
                .HasMaxLength(50)
                .HasColumnName("realm_name");
            entity.Property(e => e.ScientificName)
                .HasMaxLength(50)
                .HasColumnName("scientific_name");
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("tbl_user");

            entity.Property(e => e.UserId).HasColumnName("User_ID");
            entity.Property(e => e.CreationDate)
                .HasColumnType("datetime")
                .HasColumnName("creation_date");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.MobileNumber).HasColumnName("mobile_number");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .HasColumnName("surname");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        modelBuilder.Entity<TblUserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId);

            entity.ToTable("tbl_user_roles");

            entity.Property(e => e.UserRoleId)
                .ValueGeneratedNever()
                .HasColumnName("User_Role_ID");
            entity.Property(e => e.CreationDate)
                .HasColumnType("datetime")
                .HasColumnName("creation_date");
            entity.Property(e => e.RoleId).HasColumnName("Role_ID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.Role).WithMany(p => p.TblUserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_tbl_user_roles_tbl_role");

            entity.HasOne(d => d.User).WithMany(p => p.TblUserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_tbl_user_roles_tbl_user");
        });

        modelBuilder.Entity<TblUseraction>(entity =>
        {
            entity.HasKey(e => e.UserActionId);

            entity.ToTable("tbl_useractions");

            entity.Property(e => e.UserActionId).HasColumnName("User_Action_ID");
            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .HasColumnName("action");
            entity.Property(e => e.ActionTime)
                .HasColumnType("datetime")
                .HasColumnName("action_time");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.User).WithMany(p => p.TblUseractions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_tbl_useractions_tbl_user");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

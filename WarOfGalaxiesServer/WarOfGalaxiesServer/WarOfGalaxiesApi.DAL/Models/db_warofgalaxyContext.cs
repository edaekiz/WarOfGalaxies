using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class db_warofgalaxyContext : DbContext
    {
        public db_warofgalaxyContext()
        {
        }

        public db_warofgalaxyContext(DbContextOptions<db_warofgalaxyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblBuildings> TblBuildings { get; set; }
        public virtual DbSet<TblParameters> TblParameters { get; set; }
        public virtual DbSet<TblUserPlanetBuildingUpgs> TblUserPlanetBuildingUpgs { get; set; }
        public virtual DbSet<TblUserPlanetBuildings> TblUserPlanetBuildings { get; set; }
        public virtual DbSet<TblUserPlanets> TblUserPlanets { get; set; }
        public virtual DbSet<TblUsers> TblUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=3.122.182.106;Database=db_warofgalaxy;Trusted_Connection=False;User Id=sa;Password=VB79n7nq.;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblBuildings>(entity =>
            {
                entity.HasKey(e => e.BuildingId);

                entity.ToTable("tbl_buildings");

                entity.Property(e => e.BuildingId)
                    .HasColumnName("BuildingID")
                    .ValueGeneratedNever();

                entity.Property(e => e.BuildingName)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<TblParameters>(entity =>
            {
                entity.HasKey(e => e.ParameterId);

                entity.ToTable("tbl_parameters");

                entity.Property(e => e.ParameterId)
                    .HasColumnName("ParameterID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ParameterDateTimeValue).HasColumnType("datetime");
            });

            modelBuilder.Entity<TblUserPlanetBuildingUpgs>(entity =>
            {
                entity.HasKey(e => e.UserPlanetBuildingUpgId);

                entity.ToTable("tbl_user_planet_building_upgs");

                entity.HasIndex(e => e.UserId);

                entity.HasIndex(e => e.UserPlanetId)
                    .HasName("IX_tbl_user_planet_building_upgs")
                    .IsUnique();

                entity.Property(e => e.UserPlanetBuildingUpgId).HasColumnName("UserPlanetBuildingUpgID");

                entity.Property(e => e.BeginDate).HasColumnType("datetime");

                entity.Property(e => e.BuildingId).HasColumnName("BuildingID");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.UserPlanetId).HasColumnName("UserPlanetID");
            });

            modelBuilder.Entity<TblUserPlanetBuildings>(entity =>
            {
                entity.HasKey(e => e.UserPlanetBuildingId);

                entity.ToTable("tbl_user_planet_buildings");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_tbl_user_planet_buildings");

                entity.HasIndex(e => new { e.UserPlanetId, e.BuildingId })
                    .HasName("IX_tbl_planet_buildings")
                    .IsUnique();

                entity.Property(e => e.UserPlanetBuildingId).HasColumnName("UserPlanetBuildingID");

                entity.Property(e => e.BuildingId).HasColumnName("BuildingID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.UserPlanetId).HasColumnName("UserPlanetID");
            });

            modelBuilder.Entity<TblUserPlanets>(entity =>
            {
                entity.HasKey(e => e.UserPlanetId);

                entity.ToTable("tbl_user_planets");

                entity.HasIndex(e => e.PlanetCordinate)
                    .HasName("IX_tbl_user_planets")
                    .IsUnique();

                entity.Property(e => e.UserPlanetId).HasColumnName("UserPlanetID");

                entity.Property(e => e.LastUpdateDate).HasColumnType("datetime");

                entity.Property(e => e.PlanetCordinate)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.PlanetName)
                    .IsRequired()
                    .HasMaxLength(18);

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<TblUsers>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("tbl_users");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.GoogleToken).HasMaxLength(50);

                entity.Property(e => e.IosToken).HasMaxLength(50);

                entity.Property(e => e.UserLanguage)
                    .IsRequired()
                    .HasMaxLength(2)
                    .HasDefaultValueSql("(N'en')");

                entity.Property(e => e.UserToken).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(16);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

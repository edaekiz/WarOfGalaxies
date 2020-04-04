using System;
using Microsoft.EntityFrameworkCore;

namespace WarOfGalaxiesApi.DAL.Models
{
    public partial class db_warofgalaxyContext : DbContext
    {
        public virtual DbSet<TblPlanets> TblPlanets { get; set; }
        public virtual DbSet<TblUserPlanets> TblUserPlanets { get; set; }
        public virtual DbSet<TblUsers> TblUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=3.122.182.106;Database=db_warofgalaxy;Trusted_Connection=False;User Id=sa;Password=VB79n7nq.;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblPlanets>(entity =>
            {
                entity.HasKey(e => e.PlanetId);

                entity.ToTable("tbl_planets");

                entity.Property(e => e.PlanetId)
                    .HasColumnName("PlanetID")
                    .ValueGeneratedNever();

                entity.Property(e => e.PlanetName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TblUserPlanets>(entity =>
            {
                entity.HasKey(e => e.UserPlanetId);

                entity.ToTable("tbl_user_planets");

                entity.Property(e => e.UserPlanetId).HasColumnName("UserPlanetID");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.PlanetId).HasColumnName("PlanetID");

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
        }
    }
}

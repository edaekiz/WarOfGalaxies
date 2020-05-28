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
        public virtual DbSet<TblCordinateTypes> TblCordinateTypes { get; set; }
        public virtual DbSet<TblCordinates> TblCordinates { get; set; }
        public virtual DbSet<TblDefenses> TblDefenses { get; set; }
        public virtual DbSet<TblFleetActionTypes> TblFleetActionTypes { get; set; }
        public virtual DbSet<TblFleets> TblFleets { get; set; }
        public virtual DbSet<TblParameters> TblParameters { get; set; }
        public virtual DbSet<TblResearches> TblResearches { get; set; }
        public virtual DbSet<TblShips> TblShips { get; set; }
        public virtual DbSet<TblUserMailCategories> TblUserMailCategories { get; set; }
        public virtual DbSet<TblUserMails> TblUserMails { get; set; }
        public virtual DbSet<TblUserPlanetBuildingUpgs> TblUserPlanetBuildingUpgs { get; set; }
        public virtual DbSet<TblUserPlanetBuildings> TblUserPlanetBuildings { get; set; }
        public virtual DbSet<TblUserPlanetDefenseProgs> TblUserPlanetDefenseProgs { get; set; }
        public virtual DbSet<TblUserPlanetDefenses> TblUserPlanetDefenses { get; set; }
        public virtual DbSet<TblUserPlanetShipProgs> TblUserPlanetShipProgs { get; set; }
        public virtual DbSet<TblUserPlanetShips> TblUserPlanetShips { get; set; }
        public virtual DbSet<TblUserPlanets> TblUserPlanets { get; set; }
        public virtual DbSet<TblUserResearchUpgs> TblUserResearchUpgs { get; set; }
        public virtual DbSet<TblUserResearches> TblUserResearches { get; set; }
        public virtual DbSet<TblUsers> TblUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=3.127.186.31;Database=db_warofgalaxy;Trusted_Connection=False;User Id=sa;Password=VB79n7nq.;");
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

                entity.Property(e => e.BaseValue).HasComment("Depolar için depo değeri üretim binaları için ise saatlik üretim değeri 1.seviye de.");

                entity.Property(e => e.BuildingName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.BuildingUpgradeCostRate).HasComment("Binanın seviye başına artık göstereceği maliyet oranı.");
            });

            modelBuilder.Entity<TblCordinateTypes>(entity =>
            {
                entity.HasKey(e => e.CordinateTypeId);

                entity.ToTable("tbl_cordinate_types");

                entity.Property(e => e.CordinateTypeId)
                    .HasColumnName("CordinateTypeID")
                    .ValueGeneratedNever();

                entity.Property(e => e.CordinateTypeName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TblCordinates>(entity =>
            {
                entity.HasKey(e => e.CordinateId);

                entity.ToTable("tbl_cordinates");

                entity.HasIndex(e => e.UserPlanetId)
                    .IsUnique();

                entity.HasIndex(e => new { e.GalaxyIndex, e.SolarIndex, e.OrderIndex })
                    .IsUnique();

                entity.Property(e => e.CordinateId).HasColumnName("CordinateID");

                entity.Property(e => e.CordinateTypeId).HasColumnName("CordinateTypeID");

                entity.Property(e => e.UserPlanetId).HasColumnName("UserPlanetID");

                entity.HasOne(d => d.CordinateType)
                    .WithMany(p => p.TblCordinates)
                    .HasForeignKey(d => d.CordinateTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_cordinates_tbl_cordinate_types");

                entity.HasOne(d => d.UserPlanet)
                    .WithOne(p => p.TblCordinates)
                    .HasForeignKey<TblCordinates>(d => d.UserPlanetId)
                    .HasConstraintName("FK_tbl_cordinates_tbl_user_planets");
            });

            modelBuilder.Entity<TblDefenses>(entity =>
            {
                entity.HasKey(e => e.DefenseId);

                entity.ToTable("tbl_defenses");

                entity.Property(e => e.DefenseId)
                    .HasColumnName("DefenseID")
                    .ValueGeneratedNever();

                entity.Property(e => e.DefenseName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TblFleetActionTypes>(entity =>
            {
                entity.HasKey(e => e.FleetActionTypeId);

                entity.ToTable("tbl_fleet_action_types");

                entity.Property(e => e.FleetActionTypeId)
                    .HasColumnName("FleetActionTypeID")
                    .ValueGeneratedNever();

                entity.Property(e => e.FleetActionTypeName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TblFleets>(entity =>
            {
                entity.HasKey(e => e.FleetId);

                entity.ToTable("tbl_fleets");

                entity.HasIndex(e => e.DestinationUserPlanetId);

                entity.HasIndex(e => e.SenderUserPlanetId);

                entity.Property(e => e.FleetId).HasColumnName("FleetID");

                entity.Property(e => e.BeginDate).HasColumnType("datetime");

                entity.Property(e => e.DestinationCordinate)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.DestinationUserPlanetId).HasColumnName("DestinationUserPlanetID");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.FleetActionTypeId).HasColumnName("FleetActionTypeID");

                entity.Property(e => e.FleetData).IsRequired();

                entity.Property(e => e.SenderCordinate)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.SenderUserPlanetId).HasColumnName("SenderUserPlanetID");

                entity.HasOne(d => d.DestinationUserPlanet)
                    .WithMany(p => p.TblFleetsDestinationUserPlanet)
                    .HasForeignKey(d => d.DestinationUserPlanetId)
                    .HasConstraintName("FK_tbl_fleets_tbl_user_planets_destination");

                entity.HasOne(d => d.FleetActionType)
                    .WithMany(p => p.TblFleets)
                    .HasForeignKey(d => d.FleetActionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_fleets_tbl_fleet_action_types");

                entity.HasOne(d => d.SenderUserPlanet)
                    .WithMany(p => p.TblFleetsSenderUserPlanet)
                    .HasForeignKey(d => d.SenderUserPlanetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_fleets_tbl_user_planets_sender");
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

            modelBuilder.Entity<TblResearches>(entity =>
            {
                entity.HasKey(e => e.ResearchId);

                entity.ToTable("tbl_researches");

                entity.Property(e => e.ResearchId)
                    .HasColumnName("ResearchID")
                    .ValueGeneratedNever();

                entity.Property(e => e.ResearchName)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<TblShips>(entity =>
            {
                entity.HasKey(e => e.ShipId);

                entity.ToTable("tbl_ships");

                entity.Property(e => e.ShipId)
                    .HasColumnName("ShipID")
                    .ValueGeneratedNever();

                entity.Property(e => e.ShipName)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<TblUserMailCategories>(entity =>
            {
                entity.HasKey(e => e.UserMailCategoryId);

                entity.ToTable("tbl_user_mail_categories");

                entity.Property(e => e.UserMailCategoryId)
                    .HasColumnName("UserMailCategoryID")
                    .ValueGeneratedNever();

                entity.Property(e => e.UserMailCategoryName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TblUserMails>(entity =>
            {
                entity.HasKey(e => e.UserMailId);

                entity.ToTable("tbl_user_mails");

                entity.HasIndex(e => e.UserId);

                entity.HasIndex(e => new { e.UserId, e.IsReaded });

                entity.Property(e => e.UserMailId).HasColumnName("UserMailID");

                entity.Property(e => e.MailCategoryId).HasColumnName("MailCategoryID");

                entity.Property(e => e.MailContent)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.MailDate).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.MailCategory)
                    .WithMany(p => p.TblUserMails)
                    .HasForeignKey(d => d.MailCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_user_mails_tbl_user_mail_categories");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblUserMails)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_user_mails_tbl_users");
            });

            modelBuilder.Entity<TblUserPlanetBuildingUpgs>(entity =>
            {
                entity.HasKey(e => e.UserPlanetBuildingUpgId);

                entity.ToTable("tbl_user_planet_building_upgs");

                entity.HasIndex(e => e.UserPlanetId);

                entity.HasIndex(e => new { e.UserPlanetId, e.BuildingId })
                    .IsUnique();

                entity.Property(e => e.UserPlanetBuildingUpgId).HasColumnName("UserPlanetBuildingUpgID");

                entity.Property(e => e.BeginDate).HasColumnType("datetime");

                entity.Property(e => e.BuildingId).HasColumnName("BuildingID");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.UserPlanetId).HasColumnName("UserPlanetID");

                entity.HasOne(d => d.Building)
                    .WithMany(p => p.TblUserPlanetBuildingUpgs)
                    .HasForeignKey(d => d.BuildingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_user_planet_building_upgs_tbl_buildings");

                entity.HasOne(d => d.UserPlanet)
                    .WithMany(p => p.TblUserPlanetBuildingUpgs)
                    .HasForeignKey(d => d.UserPlanetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_user_planet_building_upgs_tbl_user_planets");
            });

            modelBuilder.Entity<TblUserPlanetBuildings>(entity =>
            {
                entity.HasKey(e => e.UserPlanetBuildingId);

                entity.ToTable("tbl_user_planet_buildings");

                entity.HasIndex(e => e.UserPlanetId);

                entity.HasIndex(e => new { e.UserPlanetId, e.BuildingId })
                    .IsUnique();

                entity.Property(e => e.UserPlanetBuildingId).HasColumnName("UserPlanetBuildingID");

                entity.Property(e => e.BuildingId).HasColumnName("BuildingID");

                entity.Property(e => e.UserPlanetId).HasColumnName("UserPlanetID");

                entity.HasOne(d => d.Building)
                    .WithMany(p => p.TblUserPlanetBuildings)
                    .HasForeignKey(d => d.BuildingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_user_planet_buildings_tbl_buildings");

                entity.HasOne(d => d.UserPlanet)
                    .WithMany(p => p.TblUserPlanetBuildings)
                    .HasForeignKey(d => d.UserPlanetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_user_planet_buildings_tbl_user_planets");
            });

            modelBuilder.Entity<TblUserPlanetDefenseProgs>(entity =>
            {
                entity.HasKey(e => e.UserPlanetDefenseProgId);

                entity.ToTable("tbl_user_planet_defense_progs");

                entity.HasIndex(e => e.UserPlanetId);

                entity.Property(e => e.UserPlanetDefenseProgId).HasColumnName("UserPlanetDefenseProgID");

                entity.Property(e => e.DefenseId).HasColumnName("DefenseID");

                entity.Property(e => e.LastVerifyDate).HasColumnType("datetime");

                entity.Property(e => e.UserPlanetId).HasColumnName("UserPlanetID");

                entity.HasOne(d => d.Defense)
                    .WithMany(p => p.TblUserPlanetDefenseProgs)
                    .HasForeignKey(d => d.DefenseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_user_planet_defense_progs_tbl_defenses");

                entity.HasOne(d => d.UserPlanet)
                    .WithMany(p => p.TblUserPlanetDefenseProgs)
                    .HasForeignKey(d => d.UserPlanetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_user_planet_defense_progs_tbl_user_planets");
            });

            modelBuilder.Entity<TblUserPlanetDefenses>(entity =>
            {
                entity.HasKey(e => e.UserPlanetDefenseId);

                entity.ToTable("tbl_user_planet_defenses");

                entity.HasIndex(e => e.UserPlanetId);

                entity.HasIndex(e => new { e.UserPlanetId, e.DefenseId })
                    .IsUnique();

                entity.Property(e => e.UserPlanetDefenseId).HasColumnName("UserPlanetDefenseID");

                entity.Property(e => e.DefenseId).HasColumnName("DefenseID");

                entity.Property(e => e.UserPlanetId).HasColumnName("UserPlanetID");

                entity.HasOne(d => d.Defense)
                    .WithMany(p => p.TblUserPlanetDefenses)
                    .HasForeignKey(d => d.DefenseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_user_planet_defenses_tbl_defenses");

                entity.HasOne(d => d.UserPlanet)
                    .WithMany(p => p.TblUserPlanetDefenses)
                    .HasForeignKey(d => d.UserPlanetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_user_planet_defenses_tbl_user_planets");
            });

            modelBuilder.Entity<TblUserPlanetShipProgs>(entity =>
            {
                entity.HasKey(e => e.UserPlanetShipProgId);

                entity.ToTable("tbl_user_planet_ship_progs");

                entity.HasIndex(e => e.UserPlanetId);

                entity.Property(e => e.UserPlanetShipProgId).HasColumnName("UserPlanetShipProgID");

                entity.Property(e => e.LastVerifyDate).HasColumnType("datetime");

                entity.Property(e => e.ShipId).HasColumnName("ShipID");

                entity.Property(e => e.UserPlanetId).HasColumnName("UserPlanetID");

                entity.HasOne(d => d.Ship)
                    .WithMany(p => p.TblUserPlanetShipProgs)
                    .HasForeignKey(d => d.ShipId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_user_planet_ship_progs_tbl_ships");

                entity.HasOne(d => d.UserPlanet)
                    .WithMany(p => p.TblUserPlanetShipProgs)
                    .HasForeignKey(d => d.UserPlanetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_user_planet_ship_progs_tbl_user_planets");
            });

            modelBuilder.Entity<TblUserPlanetShips>(entity =>
            {
                entity.HasKey(e => e.UserPlanetShipId);

                entity.ToTable("tbl_user_planet_ships");

                entity.HasIndex(e => e.UserPlanetId);

                entity.HasIndex(e => new { e.UserPlanetId, e.ShipId })
                    .IsUnique();

                entity.Property(e => e.UserPlanetShipId).HasColumnName("UserPlanetShipID");

                entity.Property(e => e.ShipId).HasColumnName("ShipID");

                entity.Property(e => e.UserPlanetId).HasColumnName("UserPlanetID");

                entity.HasOne(d => d.Ship)
                    .WithMany(p => p.TblUserPlanetShips)
                    .HasForeignKey(d => d.ShipId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_user_planet_ships_tbl_ships");

                entity.HasOne(d => d.UserPlanet)
                    .WithMany(p => p.TblUserPlanetShips)
                    .HasForeignKey(d => d.UserPlanetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_user_planet_ships_tbl_user_planets");
            });

            modelBuilder.Entity<TblUserPlanets>(entity =>
            {
                entity.HasKey(e => e.UserPlanetId);

                entity.ToTable("tbl_user_planets");

                entity.Property(e => e.UserPlanetId).HasColumnName("UserPlanetID");

                entity.Property(e => e.LastUpdateDate).HasColumnType("datetime");

                entity.Property(e => e.PlanetName)
                    .IsRequired()
                    .HasMaxLength(18);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblUserPlanets)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_user_planets_tbl_users");
            });

            modelBuilder.Entity<TblUserResearchUpgs>(entity =>
            {
                entity.HasKey(e => e.UserResearchUpgId);

                entity.ToTable("tbl_user_research_upgs");

                entity.Property(e => e.UserResearchUpgId).HasColumnName("UserResearchUpgID");

                entity.Property(e => e.BeginDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.ResearchId).HasColumnName("ResearchID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.UserPlanetId).HasColumnName("UserPlanetID");

                entity.HasOne(d => d.Research)
                    .WithMany(p => p.TblUserResearchUpgs)
                    .HasForeignKey(d => d.ResearchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_user_research_upgs_tbl_researches");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblUserResearchUpgs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_user_research_upgs_tbl_users");

                entity.HasOne(d => d.UserPlanet)
                    .WithMany(p => p.TblUserResearchUpgs)
                    .HasForeignKey(d => d.UserPlanetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_user_research_upgs_tbl_user_planets");
            });

            modelBuilder.Entity<TblUserResearches>(entity =>
            {
                entity.HasKey(e => e.UserResearchId);

                entity.ToTable("tbl_user_researches");

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserResearchId).HasColumnName("UserResearchID");

                entity.Property(e => e.ResearchId).HasColumnName("ResearchID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Research)
                    .WithMany(p => p.TblUserResearches)
                    .HasForeignKey(d => d.ResearchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_user_researches_tbl_researches");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblUserResearches)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tbl_user_researches_tbl_users");
            });

            modelBuilder.Entity<TblUsers>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("tbl_users");

                entity.HasIndex(e => e.UserToken);

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

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Middleware.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<LandPrice> LandPrices { get; set; }

    public virtual DbSet<LandType> LandTypes { get; set; }

    public virtual DbSet<Period> Periods { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Street> Streets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pg_stat_statements");

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cities_pkey");

            entity.ToTable("cities");

            entity.HasIndex(e => e.Name, "cities_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("districts_pkey");

            entity.ToTable("districts");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.Name).HasColumnName("name");

            entity.HasOne(d => d.City).WithMany(p => p.Districts)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("districts_city_id_fkey");
        });

        modelBuilder.Entity<LandPrice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("land_prices_pkey");

            entity.ToTable("land_prices");

            entity.HasIndex(e => new { e.PeriodId, e.StreetId, e.LandTypeId, e.PositionId }, "land_prices_period_id_street_id_land_type_id_position_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LandTypeId).HasColumnName("land_type_id");
            entity.Property(e => e.PeriodId).HasColumnName("period_id");
            entity.Property(e => e.PositionId).HasColumnName("position_id");
            entity.Property(e => e.Price)
                .HasPrecision(18, 2)
                .HasColumnName("price");
            entity.Property(e => e.StreetId).HasColumnName("street_id");

            entity.HasOne(d => d.LandType).WithMany(p => p.LandPrices)
                .HasForeignKey(d => d.LandTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("land_prices_land_type_id_fkey");

            entity.HasOne(d => d.Period).WithMany(p => p.LandPrices)
                .HasForeignKey(d => d.PeriodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("land_prices_period_id_fkey");

            entity.HasOne(d => d.Position).WithMany(p => p.LandPrices)
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("land_prices_position_id_fkey");

            entity.HasOne(d => d.Street).WithMany(p => p.LandPrices)
                .HasForeignKey(d => d.StreetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("land_prices_street_id_fkey");
        });

        modelBuilder.Entity<LandType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("land_types_pkey");

            entity.ToTable("land_types");

            entity.HasIndex(e => e.Name, "land_types_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Period>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("periods_pkey");

            entity.ToTable("periods");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EndYear).HasColumnName("end_year");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.StartYear).HasColumnName("start_year");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("positions_pkey");

            entity.ToTable("positions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Level).HasColumnName("level");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Street>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("streets_pkey");

            entity.ToTable("streets");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DistrictId).HasColumnName("district_id");
            entity.Property(e => e.Name).HasColumnName("name");

            entity.HasOne(d => d.District).WithMany(p => p.Streets)
                .HasForeignKey(d => d.DistrictId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("streets_district_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LastLogin)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_login");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Hiku.Services.Models;

namespace Hiku.Services.Infrastructure.Repositories.Sql
{
    public partial class HikuDbContext : DbContext
    {
        public HikuDbContext()
        {
        }

        public HikuDbContext(DbContextOptions<HikuDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AudioRequest> AudioRequest { get; set; }
        public virtual DbSet<BarcodeRequest> BarcodeRequest { get; set; }
        public virtual DbSet<Battery> Battery { get; set; }
        public virtual DbSet<Device> Device { get; set; }
        public virtual DbSet<DeviceLog> DeviceLog { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<User> User { get; set; }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AudioRequest>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AppId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.AudioFilePath).HasMaxLength(1024);

                entity.Property(e => e.DeviceId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<BarcodeRequest>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AppId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Barcode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DeviceId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Battery>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AppId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DeviceId)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Device>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(50);

                entity.Property(e => e.AgentUrl)
                    .IsRequired()
                    .HasMaxLength(1024);

                entity.Property(e => e.AppId)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<DeviceLog>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AppId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Data).IsRequired();

                entity.Property(e => e.DeviceId)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Barcode)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AppId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DeviceId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}


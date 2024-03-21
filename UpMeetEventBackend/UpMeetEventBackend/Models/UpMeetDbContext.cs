using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UpMeetEventBackend.Models;

public partial class UpMeetDbContext : DbContext
{
    public UpMeetDbContext()
    {
    }

    public UpMeetDbContext(DbContextOptions<UpMeetDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.\\sqlexpress;Initial Catalog=UpMeetDB; Integrated Security=SSPI;Encrypt=false;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("event_eventid_pk");

            entity.ToTable("Event");

            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Image).WithMany(p => p.Events)
                .HasForeignKey(d => d.ImageId)
                .HasConstraintName("event_imageid_fk");

            entity.HasMany(d => d.Users).WithMany(p => p.Events)
                .UsingEntity<Dictionary<string, object>>(
                    "UserFavorite",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("userfavorites_userid_fk"),
                    l => l.HasOne<Event>().WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("userfavorites_eventid_fk"),
                    j =>
                    {
                        j.HasKey("EventId", "UserId").HasName("userfavorites_userfavoritesid_pk");
                        j.ToTable("UserFavorites");
                        j.IndexerProperty<int>("EventId").HasColumnName("EventID");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                    });
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("images_imageid_pk");

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.Path).HasMaxLength(1000);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_userid_pk");

            entity.ToTable("User");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.Bio).HasMaxLength(2000);
            entity.Property(e => e.FirstName).HasMaxLength(20);
            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.LastName).HasMaxLength(20);
            entity.Property(e => e.Password).HasMaxLength(35);
            entity.Property(e => e.UserName).HasMaxLength(30);

            entity.HasOne(d => d.Image).WithMany(p => p.Users)
                .HasForeignKey(d => d.ImageId)
                .HasConstraintName("user_imageid_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

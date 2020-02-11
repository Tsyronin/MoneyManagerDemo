using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MoneyManagerUI
{
    public partial class MoneyManagerDBContext : DbContext
    {
        public MoneyManagerDBContext()
        {
        }

        public MoneyManagerDBContext(DbContextOptions<MoneyManagerDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<Records> Records { get; set; }
        public virtual DbSet<RecordsTags> RecordsTags { get; set; }
        public virtual DbSet<Subcategories> Subcategories { get; set; }
        public virtual DbSet<Tads> Tads { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=DESKTOP-QME9REU\\SQLEXPRESS; Database=MoneyManagerDB; Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categories>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Records>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Sum).HasColumnType("money");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Records)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Records_Categories");

                entity.HasOne(d => d.Subcategory)
                    .WithMany(p => p.Records)
                    .HasForeignKey(d => d.SubcategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Records_Subcategories");
            });

            modelBuilder.Entity<RecordsTags>(entity =>
            {
                entity.HasOne(d => d.Record)
                    .WithMany(p => p.RecordsTags)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecordsTags_Records");

                entity.HasOne(d => d.Tad)
                    .WithMany(p => p.RecordsTags)
                    .HasForeignKey(d => d.TadId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecordsTags_Tads");
            });

            modelBuilder.Entity<Subcategories>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Catedory)
                    .WithMany(p => p.Subcategories)
                    .HasForeignKey(d => d.CatedoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Subcategories_Categories");
            });

            modelBuilder.Entity<Tads>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

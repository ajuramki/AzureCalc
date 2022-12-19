using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AzureCalc.Models;

public partial class AjWebAppDatabaseContext : DbContext
{
    public AjWebAppDatabaseContext()
    {
    }

    public AjWebAppDatabaseContext(DbContextOptions<AjWebAppDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CalcOutput> CalcOutputs { get; set; }

    public virtual DbSet<Calculator> Calculators { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:aj-web-app-server.database.windows.net,1433;Initial Catalog=aj-web-app-database;Persist Security Info=False;User ID=aj-web-app-server-admin;Password=Azure123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CalcOutput>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CalcOutput");

            entity.Property(e => e.FirstValue)
                .HasMaxLength(50)
                .HasColumnName("first_value");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Operator)
                .HasMaxLength(50)
                .HasColumnName("operator");
            entity.Property(e => e.SecondValue)
                .HasMaxLength(50)
                .HasColumnName("second_value");
            entity.Property(e => e.TotalVal)
                .HasMaxLength(50)
                .HasColumnName("total_val");
        });

        modelBuilder.Entity<Calculator>(entity =>
        {
            entity.ToTable("Calculator");

            entity.Property(e => e.FirstValue)
                .HasMaxLength(50)
                .HasColumnName("first_value");
            entity.Property(e => e.Operator)
                .HasMaxLength(50)
                .HasColumnName("operator");
            entity.Property(e => e.SecondValue)
                .HasMaxLength(50)
                .HasColumnName("second_value");
            entity.Property(e => e.Total)
                .HasMaxLength(50)
                .HasColumnName("total");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(50)
                .HasColumnName("email_address");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

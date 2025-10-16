using System;
using System.Collections.Generic;
using Luna.Pages.Models.Database.WorkspaceUser;
using Microsoft.EntityFrameworkCore;

namespace Luna.Pages.Repositories.Context;

public partial class LunaPagesContext : DbContext
{
    public LunaPagesContext()
    {
    }

    public LunaPagesContext(DbContextOptions<LunaPagesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<WorkspaceUserDatabase> WorkspaceUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkspaceUserDatabase>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("workspace_users_pkey");

            entity.ToTable("workspace_users");

            entity.HasIndex(e => e.UserId, "idx_workspace_users_user");

            entity.HasIndex(e => e.WorkspaceId, "idx_workspace_users_workspace");

            entity.HasIndex(e => new { e.UserId, e.WorkspaceId }, "workspace_users_user_id_workspace_id_key").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Permissions).HasColumnName("permissions");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.WorkspaceId).HasColumnName("workspace_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

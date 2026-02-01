using Luna.Tools.SharedModels.Models.Outbox;
using Luna.Workspaces.Models.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Luna.Workspaces.Repositories.Context;

public partial class LunaWorkspacesContext : DbContext
{
	public LunaWorkspacesContext()
	{
	}

	public LunaWorkspacesContext(DbContextOptions<LunaWorkspacesContext> options)
		: base(options)
	{
	}

	public virtual DbSet<WorkspaceDatabase> Workspaces { get; set; }

	public virtual DbSet<WorkspaceUserDatabase> WorkspaceUsers { get; set; }

	public virtual DbSet<OutboxMessageDatabase> OutboxMessages { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<WorkspaceDatabase>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("workspace_pkey");

			entity.ToTable("workspace");

			entity.Property(e => e.Id)
				.ValueGeneratedNever()
				.HasColumnName("id");
			entity.Property(e => e.CreatedAt)
				.HasDefaultValueSql("now()")
				.HasColumnName("created_at");
			entity.Property(e => e.DefaultPermission)
				.HasDefaultValueSql("'view'::text")
				.HasColumnName("default_permission");
			entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
			entity.Property(e => e.Description).HasColumnName("description");
			entity.Property(e => e.Icon).HasColumnName("icon");
			entity.Property(e => e.Name).HasColumnName("name");
			entity.Property(e => e.OwnerId).HasColumnName("owner_id");
			entity.Property(e => e.Settings)
				.HasDefaultValueSql("'{}'::jsonb")
				.HasColumnType("jsonb")
				.HasColumnName("settings");
			entity.Property(e => e.UpdatedAt)
				.HasDefaultValueSql("now()")
				.HasColumnName("updated_at");
		});

		modelBuilder.Entity<WorkspaceUserDatabase>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("workspace_users_pkey");

			entity.ToTable("workspace_users");

			entity.HasIndex(e => e.UserId, "idx_workspace_users_user");

			entity.HasIndex(e => e.WorkspaceId, "idx_workspace_users_workspace");

			entity.HasIndex(e => new {e.UserId, e.WorkspaceId}, "workspace_users_user_id_workspace_id_key").IsUnique();

			entity.Property(e => e.Id)
				.ValueGeneratedNever()
				.HasColumnName("id");
			entity.Property(e => e.AcceptedAt).HasColumnName("accepted_at");
			entity.Property(e => e.CreatedAt)
				.HasDefaultValueSql("now()")
				.HasColumnName("created_at");
			entity.Property(e => e.InvitedBy).HasColumnName("invited_by");
			entity.Property(e => e.Permissions)
				.HasDefaultValueSql("'{view}'::text[]")
				.HasColumnName("permissions");
			entity.Property(e => e.UpdatedAt)
				.HasDefaultValueSql("now()")
				.HasColumnName("updated_at");
			entity.Property(e => e.UserId).HasColumnName("user_id");
			entity.Property(e => e.WorkspaceId).HasColumnName("workspace_id");
			entity.HasOne<WorkspaceDatabase>()
				.WithMany(w => w.WorkspaceUsers)
				.HasForeignKey(u => u.WorkspaceId)
				.HasPrincipalKey(w => w.Id)
				.OnDelete(DeleteBehavior.Cascade);
		});

		modelBuilder.Entity<OutboxMessageDatabase>(entity =>
		{
			entity.ToTable("outbox_messages");

			entity.HasKey(e => e.Id).HasName("outbox_messages_pkey");

			entity.Property(e => e.Id)
				.ValueGeneratedNever()
				.HasColumnName("id");
			entity.Property(e => e.Type)
				.HasColumnName("type");
			entity.Property(e => e.Payload)
				.HasColumnType("jsonb")
				.HasColumnName("payload");
			entity.Property(e => e.Status)
				.HasColumnName("status");
			entity.Property(e => e.Attempts)
				.HasDefaultValueSql("0")
				.HasColumnName("attempts");
			entity.Property(e => e.CreatedAt)
				.HasDefaultValueSql("now()")
				.HasColumnName("created_at");
			entity.Property(e => e.ProcessedAt)
				.HasColumnName("processed_at");
			entity.Property(e => e.LastError)
				.HasColumnName("last_error");
			entity.Property(e => e.LockedUntil)
				.HasColumnName("locked_until");
		});

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
using Luna.Tools.Database.Npgsql.Options;
using Luna.Users.Models.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Luna.Users.Repositories.Context;

public partial class LunaUsersContext : DbContext
{
	private readonly string _connectionString;

	public LunaUsersContext(string connectionString)
	{
		_connectionString = connectionString;
	}

	public LunaUsersContext(DbContextOptions<LunaUsersContext> options, string connectionString)
		: base(options)
	{
		_connectionString = connectionString;
	}

	public LunaUsersContext(IDatabaseOptions options)
	{
		_connectionString = options.ConnectionString;
	}

	public virtual DbSet<BookmarkDatabase> Bookmarks { get; set; }

	public virtual DbSet<ReminderDatabase> Reminders { get; set; }

	public virtual DbSet<UserDatabase> Users { get; set; }

	public virtual DbSet<UserSettingsDatabase> UserSettings { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		=> optionsBuilder.UseNpgsql(_connectionString);

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<BookmarkDatabase>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("bookmarks_pkey");

			entity.ToTable("bookmarks");

			entity.HasIndex(e => new {e.UserId, e.WorkspaceId}, "idx_bookmarks_user");

			entity.Property(e => e.Id)
				.ValueGeneratedNever()
				.HasColumnName("id");
			entity.Property(e => e.CreatedAt)
				.HasDefaultValueSql("now()")
				.HasColumnName("created_at");
			entity.Property(e => e.EntityId).HasColumnName("entity_id");
			entity.Property(e => e.EntityType)
				.HasDefaultValue(1)
				.HasColumnName("entity_type");
			entity.Property(e => e.Index).HasColumnName("index");
			entity.Property(e => e.UpdatedAt)
				.HasDefaultValueSql("now()")
				.HasColumnName("updated_at");
			entity.Property(e => e.UserId).HasColumnName("user_id");
			entity.Property(e => e.WorkspaceId).HasColumnName("workspace_id");
		});

		modelBuilder.Entity<ReminderDatabase>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("reminders_pkey");

			entity.ToTable("reminders");

			entity.Property(e => e.Id)
				.ValueGeneratedNever()
				.HasColumnName("id");
			entity.Property(e => e.CreatedAt)
				.HasDefaultValueSql("now()")
				.HasColumnName("created_at");
			entity.Property(e => e.Description).HasColumnName("description");
			entity.Property(e => e.DueAt).HasColumnName("due_at");
			entity.Property(e => e.EntityId).HasColumnName("entity_id");
			entity.Property(e => e.EntityType)
				.HasDefaultValue(1)
				.HasColumnName("entity_type");
			entity.Property(e => e.NotificationSent)
				.HasDefaultValue(false)
				.HasColumnName("notification_sent");
			entity.Property(e => e.RepeatRule)
				.HasColumnType("jsonb")
				.HasColumnName("repeat_rule");
			entity.Property(e => e.Status)
				.HasDefaultValue(1)
				.HasColumnName("status");
			entity.Property(e => e.Title).HasColumnName("title");
			entity.Property(e => e.UpdatedAt)
				.HasDefaultValueSql("now()")
				.HasColumnName("updated_at");
			entity.Property(e => e.UserId).HasColumnName("user_id");
		});

		modelBuilder.Entity<UserDatabase>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("users_pkey");

			entity.ToTable("users");

			entity.HasIndex(e => e.Username, "idx_user_username");

			entity.Property(e => e.Id)
				.ValueGeneratedNever()
				.HasColumnName("id");
			entity.Property(e => e.Bio).HasColumnName("bio");
			entity.Property(e => e.CreatedAt)
				.HasDefaultValueSql("now()")
				.HasColumnName("created_at");
			entity.Property(e => e.DisplayName).HasColumnName("display_name");
			entity.Property(e => e.Image).HasColumnName("image");
			entity.Property(e => e.LastActive)
				.HasDefaultValueSql("now()")
				.HasColumnName("last_active");
			entity.Property(e => e.UpdatedAt)
				.HasDefaultValueSql("now()")
				.HasColumnName("updated_at");
			entity.Property(e => e.Username).HasColumnName("username");
		});

		modelBuilder.Entity<UserSettingsDatabase>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("user_settings_pkey");

			entity.ToTable("user_settings");

			entity.Property(e => e.Id)
				.ValueGeneratedNever()
				.HasColumnName("id");
			entity.Property(e => e.Language)
				.HasDefaultValueSql("'en'::text")
				.HasColumnName("language");
			entity.Property(e => e.Settings)
				.HasDefaultValueSql("'{}'::jsonb")
				.HasColumnType("jsonb")
				.HasColumnName("settings");
			entity.Property(e => e.Timezone).HasColumnName("timezone");
			entity.Property(e => e.UserId).HasColumnName("user_id");
		});

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
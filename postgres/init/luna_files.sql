\c luna_files;

-- Файлы и медиа
create table files
(
    id             uuid        not null primary key,
    user_id        uuid        not null,
    workspace_id   uuid        not null,
    content_type   text        not null,
    size           integer     not null,
    storage_path   text        not null,
    created_at     timestamptz not null default now(),
    updated_at     timestamptz not null default now(),
    name           text        not null,
    status         int        not null default 1, -- processing, ready, error
    thumbnail_path text,
    original_name  text,
    description    text,
    metadata       jsonb                default '{}',
    expires_at     timestamptz,
    public_access  bool        not null default false
);

CREATE INDEX idx_files_workspace ON files(workspace_id);
CREATE INDEX idx_files_user ON files(user_id);

-- Триггеры для автоматического обновления полей updated_at
CREATE OR REPLACE FUNCTION update_timestamp()
    RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';

CREATE TRIGGER update_files_timestamp BEFORE UPDATE
    ON files FOR EACH ROW EXECUTE PROCEDURE update_timestamp();
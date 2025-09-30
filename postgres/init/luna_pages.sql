\c luna_pages;

create table workspace_users
(
    id           uuid primary key not null,
    user_id      uuid             not null,
    workspace_id uuid             not null,
    permissions  TEXT[]           NOT NULL,
    created_at   timestamptz      not null default now(),
    updated_at   timestamptz      not null default now(),
    UNIQUE ("user_id", "workspace_id")
);

CREATE INDEX idx_workspace_users_workspace ON workspace_users(workspace_id);
CREATE INDEX idx_workspace_users_user ON workspace_users(user_id);

CREATE OR REPLACE FUNCTION update_timestamp()
    RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';

CREATE TRIGGER update_users_timestamp BEFORE UPDATE
    ON workspace_users FOR EACH ROW EXECUTE PROCEDURE update_timestamp();
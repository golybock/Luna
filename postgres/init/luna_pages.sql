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

create table outbox_messages
(
    id           uuid        not null primary key,
    type         text        not null,
    payload      jsonb       not null,
    status       int         not null,
    attempts     int         not null default 0,
    created_at   timestamptz not null default now(),
    processed_at timestamptz,
    last_error   text,
    locked_until timestamptz
);

COMMENT ON TABLE outbox_messages IS 'Outbox сообщения для Search индексации';

CREATE INDEX idx_workspace_users_workspace ON workspace_users(workspace_id);
CREATE INDEX idx_workspace_users_user ON workspace_users(user_id);
CREATE INDEX idx_outbox_status_locked ON outbox_messages (status, locked_until, created_at);

CREATE OR REPLACE FUNCTION update_timestamp()
    RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';

CREATE TRIGGER update_users_timestamp BEFORE UPDATE
    ON workspace_users FOR EACH ROW EXECUTE PROCEDURE update_timestamp();
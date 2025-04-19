\c luna_templates;

CREATE TABLE templates
(
    id             UUID primary key,
    name           TEXT        NOT NULL,
    description  TEXT,
    content      JSONB       NOT NULL,
    created_at   TIMESTAMPTZ NOT NULL DEFAULT now(),
    updated_at   TIMESTAMPTZ not null default now(),
    created_by   UUID        NOT NULL,
    updated_by   UUID,
    workspace_id UUID        NOT NULL,
    icon         TEXT,
    type         int         NOT NULL, -- page, database, block
    is_global    BOOLEAN     NOT NULL DEFAULT FALSE,
    category     TEXT
);

CREATE OR REPLACE FUNCTION update_timestamp()
    RETURNS TRIGGER AS
$$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';

CREATE TRIGGER update_users_timestamp
    BEFORE UPDATE
    ON templates
    FOR EACH ROW
EXECUTE PROCEDURE update_timestamp();
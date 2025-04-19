\c luna_activity;

-- Активность и история
CREATE TABLE activity_log
(
    id           UUID primary key,
    user_id      UUID        NOT NULL,
    action       int         NOT NULL, -- create, update, delete, etc.
    entity_id    UUID        NOT NULL,
    entity_type  TEXT        NOT NULL, -- page, comment, database, etc.
    created_at   TIMESTAMPTZ NOT NULL DEFAULT now(),
    details      JSONB                DEFAULT '{}',
    workspace_id UUID        NOT NULL
);

-- Уведомления
CREATE TABLE notification
(
    id                  UUID primary key,
    user_id             UUID        NOT NULL,
    type                int         NOT NULL default 1, -- 'mention', 'comment', 'share', 'permission', 'deadline', etc.
    content             JSONB,
    sent_by             UUID,
    priority            INTEGER     NOT NULL DEFAULT 1,
    image               TEXT,
    read_at             TIMESTAMPTZ,
    created_at          TIMESTAMPTZ NOT NULL DEFAULT now(),
    title               TEXT        NOT NULL,
    status              int         NOT NULL DEFAULT 1,
    updated_at          TIMESTAMPTZ,
    related_entity_id   UUID,                           -- ID связанной сущности (страницы, комментария и т.д.)
    related_entity_type int                             -- тип сущности ('page', 'comment', 'workspace', etc.)
);

CREATE INDEX idx_activity_log_workspace ON activity_log(workspace_id);
CREATE INDEX idx_activity_log_user ON activity_log(user_id);

CREATE INDEX idx_notification_user ON notification(user_id);

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
    ON notification
    FOR EACH ROW
EXECUTE PROCEDURE update_timestamp();
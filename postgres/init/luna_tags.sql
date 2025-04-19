\c luna_tags;

-- Теги и связи
CREATE TABLE tags
(
    id           UUID primary key,
    name         TEXT        NOT NULL,
    workspace_id UUID        NOT NULL,
    color        TEXT,
    created_at   TIMESTAMPTZ NOT NULL DEFAULT now(),
    created_by   UUID        NOT NULL,
    UNIQUE (name, workspace_id)
);

CREATE TABLE page_tags
(
    id         UUID primary key,
    page_id    UUID        NOT NULL,
    tag_id     UUID        NOT NULL references tags on delete cascade,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    created_by UUID        NOT NULL,
    UNIQUE (page_id, tag_id)
);

CREATE INDEX idx_tags_workspace ON tags(workspace_id);
CREATE INDEX idx_page_tags_page ON page_tags(page_id);
\c luna_graphs;

-- Связи между страницами (для backlinks и графа)
CREATE TABLE page_relations
(
    id             UUID primary key,
    source_page_id UUID        NOT NULL,
    target_page_id UUID        NOT NULL,
    type           int         NOT NULL DEFAULT 1, -- link, embed, relation
    created_at     TIMESTAMPTZ NOT NULL DEFAULT now(),
    created_by     UUID        NOT NULL,
    block_id       UUID                            -- блок, в котором находится связь
);

CREATE INDEX idx_page_relations_source ON page_relations (source_page_id);
CREATE INDEX idx_page_relations_target ON page_relations (target_page_id);
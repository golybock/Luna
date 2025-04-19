\с luna_database;

create table databases
(
    id           uuid primary key,
    workspace_id uuid        not null,
    name         text        not null,
    created_at   timestamptz not null default now(),
    created_by   uuid        not null,
    updated_at   timestamptz not null default now(),
    updated_by   uuid        not null,
    icon         text,
    description  text,
    page_id      uuid, -- связь с отображением в виде страницы
    properties   jsonb                default '{}',
    views        jsonb                default '{}'
);

CREATE TABLE "database_schema"
(
    "id"            UUID primary key,
    "database_id"   UUID        NOT NULL,
    "name"          TEXT        NOT NULL,
    "type"          TEXT        NOT NULL, -- text, number, date, select, multi_select, relation, formula, etc.
    "properties"    JSONB       NOT NULL DEFAULT '{}',
    "is_required"   BOOLEAN     NOT NULL DEFAULT FALSE,
    "is_unique"     BOOLEAN     NOT NULL DEFAULT FALSE,
    "default_value" JSONB,
    "created_at"    TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "created_by"    UUID        NOT NULL,
    "updated_at"    TIMESTAMPTZ,
    "updated_by"    UUID,
    "index"         INTEGER,              -- порядок колонок
    FOREIGN KEY ("database_id") REFERENCES "databases" ("id") ON DELETE CASCADE
);

CREATE TABLE "database_items"
(
    "id"          UUID primary key,
    "database_id" UUID        NOT NULL,
    "values"      JSONB       NOT NULL DEFAULT '{}',
    "created_at"  TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updated_at"  TIMESTAMPTZ,
    "created_by"  UUID        NOT NULL,
    "updated_by"  UUID,
    "page_id"     UUID, -- связь с возможной детальной страницей
    FOREIGN KEY ("database_id") REFERENCES "databases" ("id") ON DELETE CASCADE
);

CREATE INDEX idx_database_name_trgm ON databases USING GIN (name);

CREATE INDEX idx_database_items_database ON database_items (database_id);

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
    ON database_items
    FOR EACH ROW
EXECUTE PROCEDURE update_timestamp();

CREATE TRIGGER update_users_timestamp
    BEFORE UPDATE
    ON database_schema
    FOR EACH ROW
EXECUTE PROCEDURE update_timestamp();

CREATE TRIGGER update_users_timestamp
    BEFORE UPDATE
    ON databases
    FOR EACH ROW
EXECUTE PROCEDURE update_timestamp();
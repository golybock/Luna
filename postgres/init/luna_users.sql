\luna_users;

create table users
(
    id           uuid primary key,
    username     text        not null,
    display_name text,
    created_at   timestamptz not null default now(),
    updated_at   timestamptz not null default now(),
    image        text,
    bio          text,
    last_active  timestamptz not null default now()
);

create table user_settings
(
    id       uuid primary key not null,
    user_id  uuid             not null references users,
    settings jsonb default '{}',
    timezone text,
    language text  default 'en'
);

-- Закладки и Избранное
create table bookmarks
(
    id           uuid primary key not null,
    user_id      uuid             not null references users,
    entity_id    uuid             not null,          -- ID страницы или базы данных
    entity_type  int              not null default 1,-- 'page', 'database', etc.
    created_at   timestamptz      not null default now(),
    updated_at   timestamptz      not null default now(),
    workspace_id uuid             not null,
    index        int                                 -- для сортировки
);

-- Для автоматических задач и напоминаний
create table reminders
(
    id                uuid        not null primary key,
    user_id           uuid        not null references users,
    entity_id         uuid        not null,          -- ID страницы или блока
    entity_type       int         not null default 1,-- 'page', 'block', etc.
    due_at            timestamptz not null,
    title             text        not null,
    description       text,
    created_at        timestamptz not null default now(),
    updated_at        timestamptz not null default now(),
    status            int         NOT NULL DEFAULT 1,
    notification_sent bool        not null default false,
    repeat_rule       jsonb                          -- для повторяющихся напоминаний
);

CREATE INDEX idx_user_username ON users(username);
CREATE INDEX idx_bookmarks_user ON bookmarks(user_id, workspace_id);

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
    ON users
    FOR EACH ROW
EXECUTE PROCEDURE update_timestamp();

CREATE TRIGGER update_users_timestamp
    BEFORE UPDATE
    ON reminders
    FOR EACH ROW
EXECUTE PROCEDURE update_timestamp();

CREATE TRIGGER update_users_timestamp
    BEFORE UPDATE
    ON bookmarks
    FOR EACH ROW
EXECUTE PROCEDURE update_timestamp();
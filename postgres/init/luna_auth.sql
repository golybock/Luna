\c luna_auth;

CREATE table auth_users
(
    id                   UUID primary key not null,
    email                text             not null unique,
    status               int              not null default 1,
    created_at           timestamptz      not null default now(),
    updated_at           timestamptz      not null default now(),
    email_confirmed      boolean          not null default false,
    verification_token   text,
    reset_password_token text,
    reset_token_expires  timestamptz
);

COMMENT ON TABLE auth_users IS 'Данные для авторизации пользователя';
COMMENT ON TABLE auth_users IS 'verification_token - токен-ссылка для подтверждения почты';
COMMENT ON TABLE auth_users IS 'reset_password_token - токен-ссылка для сброса пароля';
COMMENT ON TABLE auth_users IS 'reset_token_expires - срок истечения токена-ссылка для сброса пароля';

create table session_archive
(
    id            uuid        not null primary key,
    user_id       uuid        not null references auth_users on delete cascade,
    token         text        not null,
    refresh_token text        not null,
    device        text,
    created_at    timestamptz not null default now(),
    expires_at    timestamptz not null,
    user_agent    text,
    ip_address    text,
    revoked_at    timestamptz
);

COMMENT ON TABLE "session_archive" IS 'Сессия, хранится в redis и в архив таблице в postgresql';

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

COMMENT ON TABLE outbox_messages IS 'Outbox сообщения для RabbitMQ';

CREATE INDEX idx_user_email ON auth_users (email);
CREATE INDEX idx_outbox_status_locked ON outbox_messages (status, locked_until, created_at);

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
    ON auth_users
    FOR EACH ROW
EXECUTE PROCEDURE update_timestamp();
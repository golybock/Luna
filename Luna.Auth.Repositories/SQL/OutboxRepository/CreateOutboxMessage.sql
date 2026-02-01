insert into outbox_messages (id, type, payload, status, created_at, attempts)
values ($1, $2, $3, $4, $5, $6);

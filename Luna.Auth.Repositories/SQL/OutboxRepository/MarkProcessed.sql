update outbox_messages
set status = $1,
    processed_at = $2,
    locked_until = null,
    last_error = null
where id = $3;

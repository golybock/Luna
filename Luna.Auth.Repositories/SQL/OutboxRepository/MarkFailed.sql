update outbox_messages
set status = $1,
    last_error = $2,
    locked_until = null
where id = $3;

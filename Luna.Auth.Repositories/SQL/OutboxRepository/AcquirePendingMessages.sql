update outbox_messages
set status = $1,
    locked_until = $2,
    attempts = attempts + 1
where id in (
    select id
    from outbox_messages
    where status = $3
      and (locked_until is null or locked_until < now())
    order by created_at
    limit $4
    for update skip locked
)
returning *;

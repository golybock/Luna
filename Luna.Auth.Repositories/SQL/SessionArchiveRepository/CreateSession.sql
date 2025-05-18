insert into session_archive (id, user_id, token, refresh_token, device, expires_at, user_agent, ip_address, revoked_at)
values ($1, $2, $3, $4, $5, $6, $7, $8, $9)
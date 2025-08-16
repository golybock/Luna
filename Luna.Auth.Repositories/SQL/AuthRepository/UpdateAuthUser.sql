update auth_users
set email                = $2,
    status               = $3,
    email_confirmed      = $4,
    verification_token   = $5,
    reset_password_token = $6,
    reset_token_expires  = $7
where id = $1
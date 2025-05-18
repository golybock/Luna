update auth_users
set password_hash        = $2,
    email                = $3,
    status               = $4,
    email_confirmed      = $5,
    verification_token   = $6,
    reset_password_token = $7,
    reset_token_expires  = $8
where id = $1
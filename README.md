# Luna

Notion‑like платформа для рабочих пространств: страницы, блоки, права, поиск и realtime‑редактирование.
Проект строится на микросервисах, CQRS, event‑driven коммуникации и нескольких хранилищах под разные задачи.

## Оглавление
- [Кратко о проекте](#кратко-о-проекте)
- [Ключевые возможности](#ключевые-возможности)
- [Архитектура и сервисы](#архитектура-и-сервисы)
- [Хранилища по назначению](#хранилища-по-назначению)
- [Основные потоки](#основные-потоки)
- [Технологии](#технологии)
- [Скриншоты](#скриншоты)
- [Схемы](#схемы)

## Кратко о проекте
Luna — учебно‑портфолио проект с продакшен‑подходом: распределенная архитектура, outbox, CQRS,
асинхронные шины, search и realtime.

## Ключевые возможности
- **Notion‑like страницы**: TipTap‑документ, версии, блоки.
- **CQRS в Pages**: разделение команд и запросов.
- **Event‑driven**: Kafka/RabbitMQ, outbox.
- **Gateway**: Ocelot, единая точка входа.
- **Auth**: JWT + refresh, Google OAuth2.
- **Realtime editing**: SignalR, cursors/presence, Redis.
- **Search**: полнотекстовый поиск через ElasticSearch.

## Архитектура и сервисы
- **Auth** — аутентификация, JWT/refresh, outbox для почтовых кодов.
- **Users** — создание пользователя через gRPC при sign‑in.
- **Workspaces** — права доступа, синхронизация в Pages через Kafka.
- **Pages** — CRUD страниц, версии, поиск, realtime.
- **Notification** — отправка писем по RabbitMQ.

## Хранилища по назначению
- **MongoDB** — страницы и TipTap‑документы (гибкая JSON‑структура).
- **PostgreSQL** — права доступа, связи и транзакционные данные.
- **Redis** — кэш прав, сессии и refresh‑токены.
- **ElasticSearch** — быстрый поиск по контенту страниц.

## Основные потоки
- **Обновление страницы**: WS → Gateway → Pages (CQRS) → Outbox → ES.
- **Поиск**: ES → Mongo (получение результатов).
- **Права**: Workspaces → Kafka → Pages → кэш/БД.
- **Отправка кода**: Auth → Outbox → RabbitMQ → Notification → Email.

## Технологии
- **Backend**: .NET 9, gRPC, EF Core, ADO.NET
- **Frontend**: Next.js, TypeScript
- **Infra**: Ocelot, Kafka, RabbitMQ
- **Storage**: MongoDB, PostgreSQL, Redis, ElasticSearch
- **Realtime**: SignalR

## Скриншоты

### Основной редактор с блоками

![editor blocks](assets/page_blocks.png)

### Авторизация

![sign in](assets/sign.png)

### Главная страница

![home](assets/home.png)

### Выбор рабочего пространства

![select workspace](assets/seatch_workspace.png)

### Создание рабочего пространства

![create workspace](assets/create_workspace.png)

### Домашняя страница рабочего пространства

![workspace home](assets/workspace_home.png)

### Домашняя страница рабочего пространства без страниц

![workspace first time home](assets/home_start.png)

### Редактор страниц

![page editor](assets/page_editor.png)

### Редактор страниц (широкий)

![page editor](assets/page_editor_wide.png)

### Контекстное меню редактора

![page editor context menu](assets/context_menu.png)

### Выбор эмодзи в редакторе страниц

![page editor emoji](assets/emoji_editor.png)

### Выбор обложки для страницы

![cover select](assets/page_cover_modal.png)

![cover select](assets/page_cover_set.png)

![cover select](assets/page_cover_setted.png)

### Окно приглашения в рабочее пространство

![invite modal](assets/invite_modal.png)

### Окно редактирования рабочего пространства

![workspace editor](assets/workspace_editor.png)

### Поиск в рабочем пространстве

![search workspace](assets/search_workspace.png)

![search workspace](assets/search_in_content.png)

## Схемы
Ключевые потоки и интеграции — от авторизации до индексации и realtime.

### Auth + Refresh Flow
JWT/refresh, Redis и Google OAuth2.

![Auth flow](schemes/auth.svg)

### Код авторизации и уведомления
Outbox → RabbitMQ → Notification → Email.

![Auth code email](schemes/auth-code-email.svg)

### Gateway валидирует токены
Ocelot ходит в Auth и прокидывает custom headers.

![Gateway validation](schemes/gateway.svg)

### Permissions sync
Workspaces → Kafka → Pages → кэш и БД.

![Permissions](schemes/permissions.svg)

### Realtime editing
SignalR cursors + presence + Redis.

![Realtime editing](schemes/realtime-edit.svg)

### CQRS в Pages
Разделение команд/запросов + поиск.

![CQRS](schemes/cqrs.svg)

### Индексация страниц
Outbox → ES: актуализация контента.

![Search index](schemes/search-index.svg)

### Поиск по контенту
ES → Mongo: получение результатов.

![Search result](schemes/search-result.svg)

### Обновление страницы по WS
Gateway → SignalR → CQRS → Outbox.

![Update page](schemes/update-page.svg)
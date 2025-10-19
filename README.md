# Архитектура микросервисов для Notion/Obsidian аналога

## Общая схема взаимодействия

```
           ┌─────────────┐
           │             │
  ┌───────▶│  Frontend   │◀─────────┐
  │        │             │          │
  │        └─────────────┘          │
  │               │                 │
  │               ▼                 │
  │        ┌─────────────┐          │
  │        │             │          │
  │        │   Gateway   │──────────┘
  │        │             │  токен в cookies
  │        └─────────────┘
  │               │  добавляет данные пользователя
  │               │  из токена к запросам
  │          ┌────┴────┐
  │          ▼         ▼
┌─┴────────────┐    ┌─────────────┐     ┌─────────────┐
│              │    │             │     │             │
│ Auth Service │◀───│  Другие     │────▶│   MongoDB   │
│              │    │  Сервисы    │     │   (Pages)   │
└──────────────┘    │             │     │             │
        │           └─────────────┘     └─────────────┘
        │                  │                   
        │                  ▼                   
┌───────┴──────┐     ┌─────────────┐     ┌─────────────┐
│              │     │             │     │             │
│  PostgreSQL  │     │  PostgreSQL │     │    MinIO    │
│   (Auth)     │     │  (Meta)     │     │   (Files)   │
│              │     │             │     │             │
└──────────────┘     └─────────────┘     └─────────────┘
```

## API Gateway

Gateway выполняет следующие функции:
- Маршрутизация запросов к нужным микросервисам
- Обработка аутентификации (проверка токена)
- Добавление пользовательских данных к запросам
- Логирование запросов
- Rate limiting

### Основные API точки Gateway:

```
/api/v1/auth/*       → перенаправление на Auth Service
/api/v1/users/*      → перенаправление на User Service
/api/v1/workspaces/* → перенаправление на Workspace Service
/api/v1/pages/*      → перенаправление на Page Service
/api/v1/files/*      → перенаправление на Files Service
/api/v1/databases/*  → перенаправление на Database Service
/api/v1/activities/* → перенаправление на Activity Service 
/api/v1/tags/*       → перенаправление на Tags Service
/api/v1/templates/*  → перенаправление на Template Service
/api/v1/graph/*      → перенаправление на Graph Service
/api/v1/search/*     → перенаправление на Search Service
```

## 1. Auth Service

Отвечает за аутентификацию, регистрацию и управление сессиями.

### API Auth Service:

```
POST   /api/v1/auth/register             - Регистрация нового пользователя
POST   /api/v1/auth/login                - Аутентификация пользователя и создание сессии (возврат токена в куки)
POST   /api/v1/auth/logout               - Выход (удаление сессии)
POST   /api/v1/auth/refresh              - Обновление токена
GET    /api/v1/auth/me                   - Получение данных текущего пользователя
POST   /api/v1/auth/validate             - Проверка токена (используется Gateway)
POST   /api/v1/auth/verify-email         - Подтверждение email
POST   /api/v1/auth/request-password-reset - Запрос сброса пароля
POST   /api/v1/auth/reset-password       - Сброс пароля
PUT    /api/v1/auth/change-password      - Изменение пароля
GET    /api/v1/auth/sessions             - Список активных сессий
DELETE /api/v1/auth/sessions/{id}        - Удаление конкретной сессии
```

### События Auth Service:
- `user.created`
- `user.verified`
- `user.password_changed`
- `user.login`
- `user.logout`

## 2. User Service

Управление пользовательскими профилями и настройками.

### API User Service:

```
GET    /api/v1/users                     - Получение списка пользователей (с фильтрацией)
GET    /api/v1/users/{id}                - Получение данных пользователя
PUT    /api/v1/users/{id}                - Обновление данных пользователя
PATCH  /api/v1/users/{id}/settings       - Обновление настроек пользователя
GET    /api/v1/users/{id}/bookmarks      - Получение закладок пользователя
POST   /api/v1/users/{id}/bookmarks      - Создание новой закладки
DELETE /api/v1/users/{id}/bookmarks/{bookmarkId} - Удаление закладки
PUT    /api/v1/users/{id}/bookmarks/{bookmarkId} - Обновление закладки
POST   /api/v1/users/{id}/reminders      - Создание напоминания
GET    /api/v1/users/{id}/reminders      - Получение напоминаний пользователя
PATCH  /api/v1/users/{id}/reminders/{reminderId} - Обновление напоминания
DELETE /api/v1/users/{id}/reminders/{reminderId} - Удаление напоминания
```

### События User Service:
- `user.profile_updated`
- `user.settings_changed`
- `bookmark.created`
- `bookmark.deleted`
- `reminder.created`
- `reminder.completed`

## 3. Workspace Service

Управление рабочими пространствами и правами доступа.

### API Workspace Service:

```
GET    /api/v1/workspaces                - Получение списка доступных рабочих пространств
POST   /api/v1/workspaces                - Создание рабочего пространства
GET    /api/v1/workspaces/{id}           - Получение данных рабочего пространства
PUT    /api/v1/workspaces/{id}           - Обновление рабочего пространства
DELETE /api/v1/workspaces/{id}           - Удаление/архивация рабочего пространства
GET    /api/v1/workspaces/{id}/users     - Получение пользователей рабочего пространства
POST   /api/v1/workspaces/{id}/users     - Добавление пользователя в рабочее пространство
DELETE /api/v1/workspaces/{id}/users/{userId} - Удаление пользователя из рабочего пространства
PUT    /api/v1/workspaces/{id}/users/{userId} - Обновление прав пользователя
POST   /api/v1/workspaces/{id}/invite    - Создание приглашения
GET    /api/v1/workspaces/invites/{token} - Получение данных по приглашению
POST   /api/v1/workspaces/invites/{token}/accept - Принятие приглашения
```

### События Workspace Service:
- `workspace.created`
- `workspace.updated`
- `workspace.deleted`
- `workspace.user_added`
- `workspace.user_removed`
- `workspace.user_role_changed`
- `workspace.invite_created`
- `workspace.invite_accepted`

## 4. Page Service

Управление страницами и их содержимым, интеграция с MongoDB.

### API Page Service:

```
GET    /api/v1/pages                     - Получение списка страниц (с фильтрацией)
POST   /api/v1/pages                     - Создание новой страницы
GET    /api/v1/pages/{id}                - Получение страницы по ID
PUT    /api/v1/pages/{id}                - Обновление страницы
DELETE /api/v1/pages/{id}                - Удаление страницы
GET    /api/v1/pages/{id}/blocks         - Получение блоков страницы
POST   /api/v1/pages/{id}/blocks         - Добавление блока на страницу
PUT    /api/v1/pages/{id}/blocks/{blockId} - Обновление блока
DELETE /api/v1/pages/{id}/blocks/{blockId} - Удаление блока
POST   /api/v1/pages/{id}/duplicate      - Дублирование страницы
GET    /api/v1/pages/{id}/versions       - Получение версий страницы
GET    /api/v1/pages/{id}/versions/{versionId} - Получение конкретной версии
POST   /api/v1/pages/{id}/versions/{versionId}/restore - Восстановление версии
GET    /api/v1/pages/{id}/comments       - Получение комментариев к странице
POST   /api/v1/pages/{id}/comments       - Добавление комментария
PUT    /api/v1/pages/{id}/comments/{commentId} - Обновление комментария
DELETE /api/v1/pages/{id}/comments/{commentId} - Удаление комментария
```

### События Page Service:
- `page.created`
- `page.updated`
- `page.deleted`
- `page.block_added`
- `page.block_updated`
- `page.block_deleted`
- `page.comment_added`
- `page.comment_updated`
- `page.version_created`

## 5. Files Service

Управление файлами и интеграция с MinIO.

### API Files Service:

```
POST   /api/v1/files/upload              - Загрузка файла
GET    /api/v1/files                     - Получение списка файлов
GET    /api/v1/files/{id}                - Получение метаданных файла
GET    /api/v1/files/{id}/download       - Скачивание файла
DELETE /api/v1/files/{id}                - Удаление файла
GET    /api/v1/files/{id}/thumbnail      - Получение миниатюры
PUT    /api/v1/files/{id}/metadata       - Обновление метаданных
POST   /api/v1/files/upload-url          - Получение URL для прямой загрузки
```

### События Files Service:
- `file.uploaded`
- `file.downloaded`
- `file.deleted`
- `file.metadata_updated`

## 6. Database Service

Управление базами данных пользователя (таблицы, канбан-доски и т.д.).

### API Database Service:

```
GET    /api/v1/databases                 - Получение списка баз данных
POST   /api/v1/databases                 - Создание новой базы данных
GET    /api/v1/databases/{id}            - Получение базы данных
PUT    /api/v1/databases/{id}            - Обновление базы данных
DELETE /api/v1/databases/{id}            - Удаление базы данных
GET    /api/v1/databases/{id}/schema     - Получение схемы базы данных
POST   /api/v1/databases/{id}/schema     - Добавление поля в схему
PUT    /api/v1/databases/{id}/schema/{fieldId} - Обновление поля
DELETE /api/v1/databases/{id}/schema/{fieldId} - Удаление поля
GET    /api/v1/databases/{id}/items      - Получение записей (с фильтрацией)
POST   /api/v1/databases/{id}/items      - Создание новой записи
PUT    /api/v1/databases/{id}/items/{itemId} - Обновление записи
DELETE /api/v1/databases/{id}/items/{itemId} - Удаление записи
GET    /api/v1/databases/{id}/views      - Получение представлений
POST   /api/v1/databases/{id}/views      - Создание нового представления
PUT    /api/v1/databases/{id}/views/{viewId} - Обновление представления
```

### События Database Service:
- `database.created`
- `database.updated`
- `database.deleted`
- `database.schema_updated`
- `database.item_created`
- `database.item_updated`
- `database.item_deleted`

## 7. Activity Service

Отслеживание активности и управление уведомлениями.

### API Activity Service:

```
GET    /api/v1/activities                - Получение активности (с фильтрацией)
GET    /api/v1/activities/{workspaceId}  - Получение активности для рабочего пространства
POST   /api/v1/activities/log            - Запись действия (внутренний API)
GET    /api/v1/notifications             - Получение уведомлений
GET    /api/v1/notifications/unread      - Получение непрочитанных уведомлений
PUT    /api/v1/notifications/{id}/read   - Отметка уведомления как прочитанного
PUT    /api/v1/notifications/read-all    - Отметка всех уведомлений как прочитанных
DELETE /api/v1/notifications/{id}        - Удаление уведомления
```

### События Activity Service:
- `notification.created`
- `notification.read`
- `activity.logged`

## 8. Tags Service

Управление тегами и их связями со страницами.

### API Tags Service:

```
GET    /api/v1/tags                      - Получение списка тегов
POST   /api/v1/tags                      - Создание нового тега
GET    /api/v1/tags/{id}                 - Получение тега
PUT    /api/v1/tags/{id}                 - Обновление тега
DELETE /api/v1/tags/{id}                 - Удаление тега
GET    /api/v1/tags/{id}/pages           - Получение страниц с этим тегом
POST   /api/v1/pages/{pageId}/tags       - Добавление тега к странице
DELETE /api/v1/pages/{pageId}/tags/{tagId} - Удаление тега со страницы
```

### События Tags Service:
- `tag.created`
- `tag.updated`
- `tag.deleted`
- `page.tag_added`
- `page.tag_removed`

## 9. Template Service

Управление шаблонами страниц и компонентов.

### API Template Service:

```
GET    /api/v1/templates                 - Получение списка шаблонов
POST   /api/v1/templates                 - Создание нового шаблона
GET    /api/v1/templates/{id}            - Получение шаблона
PUT    /api/v1/templates/{id}            - Обновление шаблона
DELETE /api/v1/templates/{id}            - Удаление шаблона
GET    /api/v1/templates/categories      - Получение категорий шаблонов
GET    /api/v1/templates/global          - Получение глобальных шаблонов
POST   /api/v1/pages/{pageId}/from-template/{templateId} - Создание страницы из шаблона
```

### События Template Service:
- `template.created`
- `template.updated`
- `template.deleted`
- `template.used`

## 10. Graph Service

Управление связями между страницами и визуализацией графа знаний.

### API Graph Service:

```
GET    /api/v1/graph/{workspaceId}       - Получение графа для рабочего пространства
GET    /api/v1/graph/page/{pageId}       - Получение связей для конкретной страницы
POST   /api/v1/graph/relation            - Создание связи между страницами
DELETE /api/v1/graph/relation/{id}       - Удаление связи
GET    /api/v1/graph/backlinks/{pageId}  - Получение обратных ссылок для страницы
GET    /api/v1/graph/analysis/{workspaceId} - Анализ графа (центральные узлы, кластеры)
```

### События Graph Service:
- `relation.created`
- `relation.deleted`
- `graph.updated`

## 11. Search Service

Полнотекстовый поиск по всему контенту.

### API Search Service:

```
GET    /api/v1/search                    - Глобальный поиск
GET    /api/v1/search/pages              - Поиск по страницам
GET    /api/v1/search/files              - Поиск по файлам
GET    /api/v1/search/database-items     - Поиск по записям в базах данных
GET    /api/v1/search/suggest            - Автозаполнение поисковых запросов
POST   /api/v1/search/reindex            - Переиндексация контента (админ)
```

### События, на которые реагирует Search Service:
- `page.created`
- `page.updated`
- `page.deleted`
- `file.uploaded`
- `file.deleted`
- `database.item_created`
- `database.item_updated`
- `database.item_deleted`

## Механизм аутентификации и работы с токенами

1. **Вход пользователя**:
    - Пользователь отправляет логин/пароль на `/api/v1/auth/login`
    - Auth Service проверяет учетные данные
    - При успешной аутентификации генерирует JWT токен
    - Токен возвращается в HTTP-only cookie с флагами Secure и SameSite

2. **Обработка запросов Gateway**:
    - Gateway извлекает JWT токен из cookie
    - Отправляет токен на проверку в Auth Service через `/api/v1/auth/validate`
    - Auth Service проверяет подпись, срок действия и не был ли токен отозван
    - Возвращает данные пользователя (id, roles, permissions)
    - Gateway добавляет данные пользователя в заголовки запроса к микросервисам:
      ```
      X-User-ID: [uuid пользователя]
      X-User-Roles: [роли пользователя]
      X-Workspace-ID: [текущее рабочее пространство]
      ```

3. **Внутренняя авторизация в микросервисах**:
    - Каждый микросервис проверяет заголовки, добавленные Gateway
    - Проверяет права доступа на основе полученных данных
    - Выполняет запрошенную операцию или возвращает ошибку доступа

4. **Обновление токена**:
    - Для обновления токена используется refresh token (также в cookie)
    - Когда основной токен истекает, клиент запрашивает новый через `/api/v1/auth/refresh`
    - Если refresh token валиден, выдается новая пара токенов

## Взаимодействие между сервисами

Сервисы могут взаимодействовать двумя способами:

1. **Синхронное взаимодействие (REST/gRPC)**:
    - Прямые вызовы API для получения данных в реальном времени
    - Используется для запросов, требующих немедленного ответа

2. **Асинхронное взаимодействие (Event-driven)**:
    - Сервисы публикуют события в шине событий (Kafka/RabbitMQ)
    - Другие сервисы подписываются на интересующие их события
    - Используется для поддержания консистентности данных между сервисами

### Примеры межсервисного взаимодействия:

1. **Создание страницы**:
    - Page Service создает страницу в MongoDB
    - Публикует событие `page.created`
    - Search Service подписан на это событие и индексирует страницу
    - Activity Service регистрирует действие и создает уведомления
    - Graph Service обновляет граф связей, если есть ссылки

2. **Управление доступом к странице**:
    - Page Service делает синхронный запрос к Workspace Service для проверки прав
    - Workspace Service возвращает информацию о правах пользователя
    - Page Service принимает решение на основе этих данных

3. **Удаление пользователя**:
    - Auth Service отмечает пользователя как удаленного
    - Публикует событие `user.deleted`
    - Все сервисы, хранящие данные пользователя, подписаны на это событие
    - Каждый сервис обрабатывает удаление по своей логике
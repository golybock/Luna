db = db.getSiblingDB('luna_pages');

// Удаление существующих коллекций, если они существуют
db.page.drop();
db.page_versions.drop();
db.page_blocks.drop();
db.page_comments.drop();

db.createCollection('page');

// Создание индексов для коллекции page
db.page.createIndex({ "title": 1 });
db.page.createIndex({ "workspace_id": 1 });
db.page.createIndex({ "parent_id": 1 });
db.page.createIndex({ "owner_id": 1 });
db.page.createIndex({ "deleted_at": 1 });
db.page.createIndex({ "path": 1 });
db.page.createIndex({ "custom_slug": 1 });

// Текстовый поиск
db.page.createIndex({ "title": "text", "description": "text" });

// Создание коллекции page_versions
db.createCollection('page_versions');

// Создание индексов для коллекции page_versions
db.page_versions.createIndex({ "page_id": 1 });
db.page_versions.createIndex({ "page_id": 1, "version": 1 }, { unique: true });

// Создание коллекции page_blocks
db.createCollection('page_blocks');

// Создание индексов для коллекции page_blocks
db.page_blocks.createIndex({ "page_id": 1 });
db.page_blocks.createIndex({ "parent_id": 1 });
db.page_blocks.createIndex({ "page_id": 1, "index": 1 });

// Создание коллекции page_comments
db.createCollection('page_comments');

// Создание индексов для коллекции page_comments
db.page_comments.createIndex({ "page_id": 1 });
db.page_comments.createIndex({ "user_id": 1 });
db.page_comments.createIndex({ "parent_id": 1 });
db.page_comments.createIndex({ "block_id": 1 });

// Добавление валидации схемы для коллекции page
db.runCommand({
	collMod: "page",
	validator: {
		$jsonSchema: {
			bsonType: "object",
			required: ["_id", "title", "workspace_id", "owner_id", "latest_version", "type"],
			properties: {
				_id: {
					bsonType: "string",
					description: "UUID для страницы"
				},
				title: {
					bsonType: "string",
					description: "Заголовок страницы"
				},
				description: {
					bsonType: "string"
				},
				created_at: {
					bsonType: "date",
					description: "Дата создания"
				},
				updated_at: {
					bsonType: "date",
					description: "Дата обновления"
				},
				workspace_id: {
					bsonType: "string",
					description: "UUID рабочего пространства"
				},
				deleted_at: {
					bsonType: ["date", "null"]
				},
				latest_version: {
					bsonType: "int",
					minimum: 1
				},
				owner_id: {
					bsonType: "string",
					description: "UUID владельца"
				},
				parent_id: {
					bsonType: ["string", "null"],
					description: "UUID родительской страницы"
				},
				icon: {
					bsonType: ["string", "null"]
				},
				cover: {
					bsonType: ["string", "null"]
				},
				emoji: {
					bsonType: ["string", "null"]
				},
				type: {
					bsonType: "string",
					description: "Тип страницы: document, database_view, calendar, kanban и т.д."
				},
				path: {
					bsonType: ["string", "null"],
					description: "URL путь к странице"
				},
				index: {
					bsonType: ["int", "null"],
					description: "Порядок сортировки"
				},
				is_template: {
					bsonType: "bool",
					description: "Является ли шаблоном"
				},
				archived_at: {
					bsonType: ["date", "null"]
				},
				pinned: {
					bsonType: "bool"
				},
				custom_slug: {
					bsonType: ["string", "null"]
				},
				properties: {
					bsonType: "object",
					description: "Метаданные страницы"
				}
			}
		}
	}
});

// Валидация для коллекции page_versions
db.runCommand({
	collMod: "page_versions",
	validator: {
		$jsonSchema: {
			bsonType: "object",
			required: ["_id", "page_id", "version", "created_by"],
			properties: {
				_id: {
					bsonType: "string",
					description: "UUID версии"
				},
				page_id: {
					bsonType: "string",
					description: "UUID страницы"
				},
				version: {
					bsonType: "int",
					description: "Номер версии"
				},
				content: {
					bsonType: "object"
				},
				created_at: {
					bsonType: "date"
				},
				created_by: {
					bsonType: "string",
					description: "UUID пользователя, создавшего версию"
				},
				change_description: {
					bsonType: ["string", "null"]
				}
			}
		}
	}
});

// Валидация для коллекции page_blocks
db.runCommand({
	collMod: "page_blocks",
	validator: {
		$jsonSchema: {
			bsonType: "object",
			required: ["_id", "page_id", "type", "created_by", "index"],
			properties: {
				_id: {
					bsonType: "string",
					description: "UUID блока"
				},
				page_id: {
					bsonType: "string",
					description: "UUID страницы"
				},
				type: {
					bsonType: "string",
					description: "Тип блока: paragraph, heading, list, code, image и т.д."
				},
				content: {
					bsonType: "object"
				},
				created_at: {
					bsonType: "date"
				},
				updated_at: {
					bsonType: "date"
				},
				created_by: {
					bsonType: "string",
					description: "UUID пользователя, создавшего блок"
				},
				updated_by: {
					bsonType: ["string", "null"],
					description: "UUID пользователя, обновившего блок"
				},
				parent_id: {
					bsonType: ["string", "null"],
					description: "UUID родительского блока"
				},
				index: {
					bsonType: "int",
					description: "Порядок блоков"
				},
				properties: {
					bsonType: "object",
					description: "Дополнительные свойства блока"
				}
			}
		}
	}
});

// Валидация для коллекции page_comments
db.runCommand({
	collMod: "page_comments",
	validator: {
		$jsonSchema: {
			bsonType: "object",
			required: ["_id", "page_id", "user_id", "content"],
			properties: {
				_id: {
					bsonType: "string",
					description: "UUID комментария"
				},
				page_id: {
					bsonType: "string",
					description: "UUID страницы"
				},
				user_id: {
					bsonType: "string",
					description: "UUID пользователя"
				},
				content: {
					bsonType: "string",
					description: "Содержание комментария"
				},
				deleted_at: {
					bsonType: ["date", "null"]
				},
				created_at: {
					bsonType: "date"
				},
				updated_at: {
					bsonType: ["date", "null"]
				},
				parent_id: {
					bsonType: ["string", "null"],
					description: "UUID родительского комментария"
				},
				block_id: {
					bsonType: ["string", "null"],
					description: "UUID связанного блока"
				},
				reactions: {
					bsonType: "object",
					description: "Реакции на комментарий"
				}
			}
		}
	}
});
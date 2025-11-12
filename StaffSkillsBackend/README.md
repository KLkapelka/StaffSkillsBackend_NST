# StaffSkillsBackend

Тестовое задание NST. Backend API для управления навыками сотрудников. Система для просмотра и редактирования навыков персонала в IT-компании. Реализована backend-часть с REST API. Проект покрыт модульными/Unit и интеграционными/Integration тестами

## Технологии

- ASP.NET Core 8.0
- Entity Framework Core 8.0.21
- SQLite
- Swagger/OpenAPI

## Структура БД

### Person (Сотрудник)
- `id`: long - уникальный идентификатор
- `name`: string - полное имя
- `displayName`: string - отображаемое имя
- `skills`: List<Skill> - список навыков

### Skill (Навык)
- `id`: long - уникальный идентификатор
- `name`: string - название навыка
- `level`: byte - уровень владения (1-10)
- `personId`: long - внешний ключ на Person

**Связь:** 1 Person → многие Skills (cascade delete)

## API Endpoints

| Метод | Endpoint | Описание |
|-------|----------|----------|
| GET | `/api/v1/persons` | Получить всех сотрудников |
| GET | `/api/v1/persons/{id}` | Получить сотрудника по ID |
| POST | `/api/v1/persons` | Создать нового сотрудника |
| PUT | `/api/v1/persons/{id}` | Обновить данные сотрудника |
| DELETE | `/api/v1/persons/{id}` | Удалить сотрудника |

## Запуск проекта

### Требования

- .NET SDK 8.0 или выше
- Git

### Установка
(Данный способ не гарантирует полное восстановление и корректную работоспособность проекта на вашем устройстве)

1. Клонируйте репозиторий
2. Перейди в папку проекта: `cd StaffSkillsBackend`
3. Восстановите пакеты: `dotnet restore`
4. Примените миграции БД: `dotnet ef database update`
5. Запустите приложение: `dotnet run`
6. Откройте Swagger UI: `http://localhost:5068/swagger`

## Статусы ответов

- `200 OK` — успешное выполнение запроса
- `201 Created` — ресурс успешно создан
- `204 No Content` — успешное выполнение без тела ответа
- `400 Bad Request` — неверный запрос
- `404 Not Found` — сущность не найдена
- `500 Internal Server Error` — внутренняя ошибка сервера

## Валидация

- Уровень навыка (level) должен быть от 1 до 10
- Все обязательные поля должны быть заполнены
- При обновлении сотрудника старые навыки удаляются и заменяются новыми

### Структура тестов

#### Модульные тесты/Unit Tests
- **PersonServiceTests** - тестирование бизнес-логики сервиса
    - GetAllAsync - получение всех сотрудников
    - GetByIdAsync - получение по ID (валидный/невалидный)
    - CreateAsync - создание сотрудника с навыками
    - UpdateAsync - обновление данных (валидный/невалидный ID)
    - DeleteAsync - удаление сотрудника (валидный/невалидный ID)

#### Интеграционные тесты/Integration Tests
- **PersonsControllerIntegrationTests** - тестирование API endpoints
    - GET /api/v1/persons - получение списка
    - GET /api/v1/persons/{id} - получение по ID (200/404)
    - POST /api/v1/persons - создание (201)
    - PUT /api/v1/persons/{id} - обновление (200/404)
    - DELETE /api/v1/persons/{id} - удаление (204/404)
    - Полный сценарий CRUD операций

### Технологии тестирования

- **xUnit** - фреймворк для тестирования
- **FluentAssertions** - удобные проверки
- **Moq** - создание mock-объектов
- **EF Core InMemory** - in-memory база данных
- **WebApplicationFactory** - тестирование API



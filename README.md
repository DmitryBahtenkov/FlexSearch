
# FlexSearch


FlexSearch — поисковый движок с json rest api, написанный на .NET C#.
Подобные движки используются при сложном поиске по базе документов.
На данный момент доступны два вида поиска: полнотекстовый и по точному совпадению.
GreatSearchEngine также является файловой бд. Документы хранятся в формате json.

## Install

Для запуска на сервере с помощью docker скачайте контейнер из dockerhub https://hub.docker.com/r/bahtenkov/searchapi и запустите контейнер.
По умолчанию он настроен на порт 5001.

## Endpoints

Пусть наш движок будет размещён на example.com.

### Основные операции

#### GET http://example.com/

Получить список всех баз данных

#### GET http://example.com/{databaseName}

Получить список всех индексов в базе данных databaseName

### Операции с индексами

#### POST: http://example.com/index/{databaseName}/{indexName}

##### application/json - любой объект.

Добавить объект в базу данных databaseName в индекс indexName

#### GET http://example.com/index/{databaseName}/{indexName}/

Получить все записи из индекса indexName, который находится в базе данных databaseName

#### GET http://example.com/index/{databaseName}/{indexName}/{id}

Получить запись с указанным id из индекса indexName, который находится в базе данных databaseName

#### PUT http://example.com/index/{databaseName}/{indexName}/rename

##### Query parameters: name=newName

Переименовать индекс на newName

#### PUT http://example.com/index/{dbname}/{index}/{id}/

##### application/json - любой объект.
Обновить документ с Id = id из базы данных dbname и индекса index

#### DELETE http://example.com/index/{databaseName}/

Удалить базу данных databaseName со всеми внутренними данными

#### DELETE http://example.com/index/{dbname}/{index}/{id}/

Удалить документ с Id = id из базы данных dbname и индекса index

#### DELETE http://example.com/index/{databaseName}/{indexName}/

Удалить индекс indexName из базы данных databaseName со всеми данными

### Поиск

#### GET http://example.com/search/{databaseName}/{indexName}/

##### application/json:

```
{

 Type:{type}
  Key:{key},
  Term:{text}

}
```

Поиск типа type по ключу key с искомым текстом text.
Доступные типы поиска:

* Fulltext - полнотекстовый по ключу
* Errors - с ошибками по ключу
* Match - полное совпадение по ключу
* Full - полное совпадение по всему документу
* Not - операция "НЕ"
* Or - операция "ИЛИ"

### Авторизация

#### POST on users/

application/json:

```
{
	"UserName":"string",
	"Password":"string",
	Database:"all"
}
```

Создать нового пользователя
Доступ только под root!

#### GET on users/all

Получить список всех пользователей без паролей (любой авторизованный юзер)

#### GET on users/pass

Получить список всех пользователей с паролями (только root)

#### GET on users/{username}

Получить данные пользователя `username` (только root)

#### PUT on users/{username}

application/json

```
{
	"UserName":"string",
	"Password":"string",
	Database:"all"
}
```

Изменить данные пользователя `username` на новые (только root)

#### DELETE on users/{username}

Удалить пользователя `username` (только root)

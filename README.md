# GreatSearchEngine

GreatSearchEngine — поисковый движок с json rest api, написанный на .NET C#.
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

#### POST: http://example.com/index/{databaseName}/{indexName}/add

##### application/json - любой объект.

Добавить объект в базу данных databaseName в индекс indexName

#### GET http://example.com/index/{databaseName}/{indexName}/all

Получить все записи из индекса indexName, который находится в базе данных databaseName

#### GET http://example.com/index/{databaseName}/{indexName}/{id}

Получить запись с указанным id из индекса indexName, который находится в базе данных databaseName

#### PUT
http://example.com/index/{databaseName}/{indexName}/rename

##### Query parameters: name=newName

Переименовать индекс на newName

#### DELETE
http://example.com/index/{databaseName}/delete
Удалить базу данных databaseName со всеми внутренними данными

#### DELETE
http://example.com/index/{databaseName}/{indexName}/delete
Удалить индекс indexName из базы данных databaseName со всеми данными


### Поиск

#### GET http://example.com/fulltext/{databaseName}/{indexName}/

##### application/json:

```
{

  Key:{key},
  Text:{text}

}
```

Полнотекстовый поиск по ключу key с искомым текстом text.

#### GET http://example.com/match/{databaseName}/{indexName}/

##### application/json:

```
{

  Key:{key},
  Text:{text}

}
```

Поиск на точное совпадение текста text по ключу key.

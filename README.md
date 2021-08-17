# BookLibrary
Реализованная библиотека позволяет взаимодействовать с базой данных MongoDB направленной на хранение информации о книгах. В качестве БД использовалась Mongo DB 5.0.2
WebApi находится в папке **BookLibrary**, настройки для базы данных находятся в файле **BookLibrary/appsettings.json**
## Настройки в файле appsetting.json
Все настройки для базы данных находятся в массиве BookLibrarySettings.

| Ключ | За что отвечает |
:---------:|:---------:|
| BooksCollectionName | Название коллекции с информацией о книгах |
| UsersCollectionName | Название коллекции с информацией о пользователях |
| ConnectionString | Строка подключения к БД |
| DBName | Название базы данных |

## Формат выходных данных  

Результаты запросов всегда приходят в формате JSON с двумя обязательными строкам: Error и Result. Пример:
```json
{
    "error": null,
    "result": [
        {
            "id": "61164a66bcebdc788222203a",
            "bookName": "The Catcher in ihe Rye",
            "author": "Jerome Salinger",
            "genres": [
                "Novel",
                "Realism"
            ]
        },
        {
            "id": "611b745cfa00eb2d041b076c",
            "bookName": "A Clockwork Orange",
            "author": "Anthony Burgess",
            "genres": [
                "Novel",
                "Satire"
            ]
        },
        {
            "id": "611b7554fe772e422cd559b7",
            "bookName": "A Clockwork Orange",
            "author": "Anthony Burgess",
            "genres": [
                "Novel",
                "Satire"
            ]
        }
    ]
}
```
____
## Реализованные методы 
### Методы авторизации
1. **POST** api/auth/login
При успешном запросе возвращает Bearer токен, который действует 15 минут, в строке Result.  
На данный момент в базе данных реализован 1 пользователь с логином User123, паролем qweasd123 и ролью User.

| Параметр | Тип данных | Обязательный |  Описание |
:---------:|:---------:|:---------:|:---------:|
| username | string | да | Логин пользователя |
| password | string | да | Пароль пользователя |

Пример успешного ответа:

```json

{
    "error": null,
    "result": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiVXNlcjEyMyIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJuYmYiOjE2MjkxOTQxMjgsImV4cCI6MTYyOTE5NTAyOCwiaXNzIjoiU2ltcGxlQXV0aGVuaWNhdGlvbiIsImF1ZCI6IkJvb2tMaWJyYXJ5In0.4jbLrpVVwvqrXyx-XdCugdP0QHSvlQPcBA6yJ2ZHNiw"
}

```
### Методы работы с библиотекой книг
*Прим.: перечисленные ниже методы работают только с авторизованными пользователями, и возвращаю результаты на основе Username указанного при авторизации.*  

1. **GET** api/books/getbooks
Возвращает список книг, хранящихся в библиотеке для данного пользователя.  

| Параметр | Тип данных | Обязательный | Описание |
:---------:|:---------:|:---------:|:---------:|
| genre | string | нет | Если указан, то фильтрует книги по указанному жанру |

Пример успешного ответа:
```json
{
    "error": null,
    "result": [
        {
            "id": "61164a66bcebdc788222203a",
            "bookName": "The Catcher in ihe Rye",
            "author": "Jerome Salinger",
            "genres": [
                "Novel",
                "Realism"
            ]
        },
        {
            "id": "611b745cfa00eb2d041b076c",
            "bookName": "A Clockwork Orange",
            "author": "Anthony Burgess",
            "genres": [
                "Novel",
                "Satire"
            ]
        },
        {
            "id": "611b7554fe772e422cd559b7",
            "bookName": "A Clockwork Orange",
            "author": "Anthony Burgess",
            "genres": [
                "Novel",
                "Satire"
            ]
        }
    ]
}
```
2. **GET** /api/books/getbookinfo  
Возвращает информацию по ID книги.

| Параметр | Тип данных | Обязательный | Описание |
:---------:|:---------:|:---------:|:---------:|
| id | string | да | Id книги, которую необходимо найти |

Пример успешного ответа:

```json
{
    "error": null,
    "result": {
        "id": "611b87a0d34f854bbd56347a",
        "owner": "User123",
        "bookName": "SomeRandomBook",
        "author": "SomeRandomAuthor",
        "genres": [
            "FirstGenre",
            "SecondGenre"
        ]
    }
}
```

3. **POST** /api/books/add  
Добавляет новую книгу и возвращает ее ID.

| Параметр | Тип данных | Обязательный | Описание |
:---------:|:---------:|:---------:|:---------:|
| author | string | да | Автор книги |
| bookname | string | да | Название книги |
| genres | string[] | да | Жанры книги |

Пример успешного ответа:

```json
{
    "error": null,
    "result": "611b90f3102f77e4794f9fa7"
}
```

4. **POST** /api/books/update  
Обновляет указанную книгу.

| Параметр | Тип данных | Обязательный | Описание |
:---------:|:---------:|:---------:|:---------:|
| id | string | да | ID книги, которую необходимо обновить |
| author | string | да | Автор книги |
| bookname | string | да | Название книги |
| genres | string[] | да | Жанры книги |

Пример успешного ответа:

```json
{
    "error": null,
    "result": "Книга успешно обновлена"
}
```

5. . **DELETE** /api/books/delete  
Удаляет указанную книгу.

| Параметр | Тип данных | Обязательный | Описание |
:---------:|:---------:|:---------:|:---------:|
| id | string | да | ID книги, которую необходимо удалить |

Пример успешного ответа:

```json
{
    "error": null,
    "result": "Книга успешно удалена"
}
```
____
К сожалению, у меня не удалось сделать нормальные Unit тесты для данной библиотеки

# User:
- POST api/login - вход
    Body: json
    - Email
    - Password
    Return: jwt токен

- POST api/register - регистрация
    Body: json
    - Email
    - Password
    - PasswordConfirmation

- GET api/verify?token=<token> - подтверждение почты
    Return: json
    - Email
    - EmailConfirmed: bool

# Items:
- GET api/items - запрос всех записей пользователя
    Header:
    - Authorization: Bearer <token>
    Return: json (массив [])
    - Id: int
    - BirthdayDate: YYYY-MM-DD
    - PersonName
    - Note
    - ImagePath

- GET api/items?birthdayId=<id> - запрос записи по id
    Header:
    - Authorization: Bearer <token>
    Return: json
    - Id: int
    - BirthdayDate: YYYY-MM-DD
    - PersonName
    - Note
    - ImagePath

- POST api/items - создание записи
    Header:
    - Authorization: Bearer <token>
    Body: json
    - BirthdayDate: YYYY-MM-DD
    - PersonName
    Return: json (добавленная запись)
    - Id: int
    - BirthdayDate: YYYY-MM-DD
    - PersonName
    - Note
    - ImagePath

- PUT api/items?birthdayId=<id> - обновление записи
    Header:
    - Authorization: Bearer <token>
    Body: json
    - BirthdayDate: YYYY-MM-DD
    - PersonName
    Return: json (измененная запись) 
    - Id: int
    - BirthdayDate: YYYY-MM-DD
    - PersonName
    - Note
    - ImagePath

- DELETE api/items?birthdayId=<id> - удаление записи (и всех записей уведомлений)
    Header:
    - Authorization: Bearer <token>
    Return: json (удаленная запись)
    - Id: int
    - BirthdayDate: YYYY-MM-DD
    - PersonName
    - Note
    - ImagePath

# Images:
- GET api/images?birthdayId=<id> - запрос изображения записи
    Header:
    - Authorization: Bearer <token>
    Return: image/jpeg

- POST api/images?birthdayId=<id> - загрузка/обновление изображения записи (удаляет предыдущее, если есть)
    Header:
    - Authorization: Bearer <token>
    Body: form-data
    - file: тип File, jpg || jpeg
    Return: json (запись)
    - Id: int
    - BirthdayDate: YYYY-MM-DD
    - PersonName
    - Note
    - ImagePath

- DELETE api/images?birthdayId<id> - удаление изображения записи
    Header:
    - Authorization: Bearer <token>
    Return: json (запись)
    - Id: int
    - BirthdayDate: YYYY-MM-DD
    - PersonName
    - Note
    - ImagePath

# Notifications:
- GET api/notifications?birthdayId=<id> - запрос всех записей об уведомлении для дня рождения
    Header:
    - Authorization: Bearer <token>
    Return: json (массив [])
    - Id: int
    - BirthdayId: int
    - DaysBefore: int

- GET api/notifications?birthdayId=<id>&notificationId=<id> - запрос записи об уведомлении дня рождения по id
    Header:
    - Authorization: Bearer <token>
    Return: json
    - Id: int
    - BirthdayId: int
    - DaysBefore: int

- POST api/notifications?birthdayId=<id> - загрузка записи уведомления
    Header:
    - Authorization: Bearer <token>
    Body: json
    - DaysBefore: int [0; 10]
    Return: json (добавленной)
    - Id: int
    - BirthdayId: int
    - DaysBefore: int

- PUT api/notifications?birthdayId=<id>&notificationId=<id> - обновления записи уведомления
    Header:
    - Authorization: Bearer <token>
    Body: json
    - DaysBefore: int [0; 10]
    Return: json (измененной)
    - Id: int
    - BirthdayId: int
    - DaysBefore: int

- DELETE api/notifications?birthdayId=<id>&notificationId=<id> - удаление записи уведомления
    Header:
    - Authorization: Bearer <token>
    Return: json (удаленной)
    - Id: int
    - BirthdayId: int
    - DaysBefore: int




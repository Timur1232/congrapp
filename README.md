# Congrapp

Congrapp - это веб-приложение для управления днями рождениями ваших друзей, родственников и коллег. Приложение позволяет создавать записи о днях рождениях, добавлять фотографии, устанавливать напоминания и отслеживать предстоящие события.

# Скриншоты
## Дни рождения сегодня
<img width="358" height="384" alt="1" src="https://github.com/user-attachments/assets/5dec2401-fc5b-4e52-96f6-562e2120eb11" />

## Предстоящие дни рождения
<img width="1236" height="344" alt="2" src="https://github.com/user-attachments/assets/df947a86-3d89-4194-ac2c-0b8b67fb534f" />

## Добавление дня рождения
<img width="633" height="416" alt="3" src="https://github.com/user-attachments/assets/1e349b68-9c7a-4770-b171-d7ed03bd8c98" />

## Изменение дня рождения и добавление уведомлений
<img width="837" height="713" alt="4" src="https://github.com/user-attachments/assets/cf311a40-8528-47de-be66-b5538937b8e5" />

Уведомления можно добавить только при подтвержедении почты

## Писмо с подтверждением почты
<img width="455" height="203" alt="5" src="https://github.com/user-attachments/assets/2184e053-f17f-4984-afdb-f7dfb199c154" />

## Уведомление о дне рождении
<img width="349" height="246" alt="6" src="https://github.com/user-attachments/assets/ea52ab18-4477-443a-a893-993a370741c5" />

# Технологии
Бэкенд:
- ASP.NET Core Web API - основной фреймворк для API
- Entity Framework Core - ORM для работы с базой данных
- JWT (JSON Web Tokens) - аутентификация пользователей
- SMTP - отправка электронных писем для подтверждения регистрации и напоминаний

Фронтенд:
- React - библиотека для построения пользовательского интерфейса
- TypeScript - язык программирования для строгой типизации
- React Router - маршрутизация в приложении
- Docker - контейнеризация фронтенда

Инфраструктура
- Docker Compose - оркестрация контейнеров (бэкенд, фронтенд, SMTP сервер)

# Инструкция по запуску
## Предварительные требования
- Установите [Docker и Docker Compose](https://www.docker.com/products/docker-desktop/)
- Установите .NET SDK (для применения миграций)

## Шаги запуска
Клонируйте репозиторий:
```bash
git clone https://github.com/Timur1232/congrapp
cd congrapp
```

Настройте файл конфигурации бэкенда:
```bash
vim Congrapp.Server/appsettings.json
```

Обновите следующие параметры:
- ConnectionStrings.DefaultConnection - строка подключения к вашей БД
- JwtSettings.Secret - секретный ключ для JWT
- SmtpSettings - настройки SMTP сервера

Примените миграции для создания базы данных:
```bash
cd Congrapp.Server
dotnet ef database update

```

Запустите приложение с помощью Docker Compose:
```bash
cd ..
docker-compose -f compose.yaml -f compose.override.yml up -d --build
```
Приложение будет доступно по адресу: http://localhost:3000

---
Доступ к компонентам системы
- Frontend: http://localhost:3000
- Backend API: http://localhost:8080
- Papercut SMTP: http://localhost:8082

# Функциональные возможности
- ✅ Регистрация и аутентификация пользователей
- ✅ Подтверждение email по ссылке
- ✅ Создание и управление записями о днях рождениях
- ✅ Загрузка фотографий
- ✅ Настройка напоминаний (1-10 дней до события)
- ✅ Просмотр предстоящих дней рождения
- ✅ Выделение сегодняшних дней рождения

Баймурадов Тимур

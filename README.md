# Микросервис статистики

Микросервис (StatMicroservice.sln), принимающий информацию о событии в формате json и выдающий список событий, отсортированный по указанному полю внутри json

#### API микросервиса:



------------------------

Два приложения общаются друг с другом через транспорт, реализуя расчет чисел Фибоначчи.  
  
Логика расчета одной последовательности такая:  
1.  Первое инициализирует расчет.  
2.  Первое отправляет второму N(i)  
3.  Второе вычисляет N(i-1) + N(i) и шлет обратно  
4.  Логика повторяется симметрично.  
5.  И так до остановки приложений.  
#### Особенности  
Первое приложение при старте получает параметр – целое число, сколько асинхронных расчетов начать. Все расчеты
идут параллельно.  
Передача данных от 1 к 2 идет через Rest WebApi  
Передача данных от 2 к 1 идет посредством MessageBus.  
Язык C#, среда MS .NET Framework версии от 4.0.  
#### Рекомендуемые технологии  
REST: ASP.NET WebApi + HttpClient  
MessageBus: RabbitMQ + EasyNetQ  
### Реализация
Приложение1 (Application1) - Консольное приложение .NET Core  
Приложение2 (Application2) - Приложение ASP.NET Core WebAPI  
Приложения были разработаны в Visual Studio 2019 на платформе .NET Core 3.1
#### Настройка
##### Приложение1 (Application1)
Файл appsettings.json содержит два ключа:  
`"web_api_url"` - адрес хоста web-службы  
`"rabbit_api_url"` - connection string шины rabbitMQ  
##### Приложение2 (Application2)
Файл appsettings.json содержит ключ:  
`"rabbit_connection_string"` - connection string шины rabbitMQ

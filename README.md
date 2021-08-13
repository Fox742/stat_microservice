# Микросервис статистики

Микросервис (StatMicroservice.sln), принимающий информацию о событии в формате json и выдающий список событий, отсортированный по указанному полю внутри json

#### API микросервиса:
`"post: add(string key, string eventJson,DateTime? clientDT = null)"`  
Добавить информацио о событии в базу данных 
`"key"` - Ключ. События с одним и тем же ключом могут быть запрошены в соответствующем вызове  
`"eventJson"` - JSON свободного формата, по полю внутри которого может быть произведена сортировка при запросе  
`"clientDT"` - Дата и время события, передаваемые клиентом  



`"get: get(string key, string field, DateTime? start = null, DateTime? finish = null, int pageSize = -1, int pageNumber = -1)"`  
Выбрать отсортированный по полю JSON-а список событий с ключом `"key"`  
`"key"` - Ключ. Возвращаются только события с указанным ключом  
`"field"` - Поле внутри JSON-а, по которому происходит сортировка событий  
`"start"` - Дата и время, с начиная с которых надо выбрать события  
`"finish"` - Дата и время, события до которого необходимо выбрать  
`"pageSize"` - Размер страницы (для пагинации)  
`"pageNumber"` - Номер страницы (для пагинации)  



`"get: getcount(string key, DateTime? start = null, DateTime? finish = null)"`  
Посчитать количество выбираемых событий с ключом `"key"` между датой и временем `"start"` и `"finish"` (нужно для организации пагинации)  
`"key"` - Ключ. Возвращаются только события с указанным ключом  
`"start"` - Дата и время, с начиная с которых надо выбрать события  
`"finish"` - Дата и время, события до которого необходимо выбрать  



`"post: clear()"`  
Очистить базу данных

#### Алгоритм сортировки:

1. Выбор из базы данных событий по ключу и дате, если она есть, получаем список:  
`"Событие0 JSON { "field": "00004" }"`  
`"Событие1 JSON { "field": "00003" }"`  
`"Событие2 JSON { "field": "00002" }"`  
`"Событие3 JSON { "field": "00001" }"`  
`"Событие4 JSON { "field": "00000" }"`  
2. Составляется список пар из значений поля JSON-а каждого из событий и его индекс в списке из пункта 1 (Сначала индексы отсортированы в порядке возрастания от нуля с шагом 1)  
`"Пара<"00004" (значение из события0), 0>"`  
`"Пара<"00003" (значение из события1), 1>"`  
`"Пара<"00002" (значение из события2), 2>"`  
`"Пара<"00001" (значение из события3), 3>"`  
`"Пара<"00000" (значение из события4), 4>"`  
3. Сортировка списка пар по значению из событий:  
`"Пара<"00000" (значение из события4), 4>"`  
`"Пара<"00001" (значение из события3), 3>"`  
`"Пара<"00002" (значение из события2), 2>"`  
`"Пара<"00003" (значение из события1), 1>"`  
`"Пара<"00004" (значение из события0), 0>"`  
4. Составление перемешивание старого списка из пункта 1 в соответствие с индексами пар, то есть если в первой паре после сортировки стоит индеекс 4, то берётся четвёрнтый элемент из первого списка
`"Событие4 JSON { "field": "00000" }"`  
`"Событие3 JSON { "field": "00001" }"`  
`"Событие2 JSON { "field": "00002" }"`  
`"Событие1 JSON { "field": "00003" }"`  
`"Событие0 JSON { "field": "00004" }"`  


#### Технологии:

Микросервис: ASP.NET Core 3.0 WebAPI  
Программа-тестер: Console .NET Core 3.0

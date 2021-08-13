using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Tester
{
    class Program
    {
 
        private static void printJTokens(IEnumerable<JToken> objects)
        {
            foreach(JToken token in objects)
            {
                Console.WriteLine(token.ToString());
            }
        }

        static void Main(string[] args)
        {
            using (MicroserviceClient _client = new MicroserviceClient())
            {
                InputKeeper input = new InputKeeper();
                Console.Write("Нахмите Enter, чтобы тестировать");
                Console.ReadLine();
                Console.WriteLine("Удаляем старую базу (если она есть)");
                if (!_client.CheckDatabaseDropped())
                {
                    Console.WriteLine("Не получилось удалить базу данных статистики");
                    return;
                }
                else
                {
                    Console.WriteLine("База удалена");
                }
                Console.WriteLine("Входной массив событий (он будет отправлен)");
                Console.WriteLine();
                Console.WriteLine();
                printJTokens(input.objects);

                _client.SendToServer(input.objects);
                Console.WriteLine("Массив отправлен!");
                
                var items = _client.GetSorted();
                Console.WriteLine("Отсортированный массив целиком:");
                printJTokens( items.Children() );

                items = _client.GetSorted(
                    begin: new DateTime (1989,12,31,23,59,59),
                    finish: new DateTime(2001, 1, 1, 0, 0, 0));
                Console.WriteLine("Отсортированный массив c датой между 1989 годом и двухтысячным:");
                printJTokens(items.Children());

                items = _client.GetSorted(
                    pageSize: 5,
                    pageNumber: 2
                    );
                Console.WriteLine("Отсортированный массив c размером страницы - 5 записей, 2 по счёту:");
                printJTokens(items.Children());

                Console.WriteLine("Количество записей (всех): {0}", _client.GetCount());

                Console.WriteLine(
                    "Количество записей  c датой между 1989 годом и двухтысячным: {0}",
                    _client.GetCount(
                        begin: new DateTime(1989, 12, 31, 23, 59, 59),
                        finish: new DateTime(2001, 1, 1, 0, 0, 0)));
            }
        }
    }
}

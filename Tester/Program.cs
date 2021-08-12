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
                Console.WriteLine("Нахмите Enter, чтобы тестировать");
                Console.ReadLine();
                Console.WriteLine("Удаляем старую базу (если она есть)");
                if (!_client.DropDatabase())
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
                printJTokens( items.Children() );
            }


            // Удалить базу данных (хехе)

            // Распечатать исходные данные

            // Скормить их микросервису

            // Вызвать запрос

            // Распечатать ответ от сервера
            /*
            using (var client = new HttpClient())
            {
                var builder = new ConfigurationBuilder()
               .AddJsonFile($"appsettings.json", true, true);

                var config = builder.Build();
                var connectionString = config["ConnectionStrings:CommonConnectionString"];

                var parameters = new Dictionary<string, string> { { "key", "concrete_key" }, { "eventJson", "concrete_json" } };
                var encodedContent = new FormUrlEncodedContent(parameters);

                client.BaseAddress = new Uri("http://localhost:5001/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                
                //var response = client.PostAsync("statistics/clear", encodedContent).Result;
                //Console.WriteLine(response.ToString());
                
                
                
                var response = client.PostAsync("statistics/add", encodedContent).Result;
                Console.WriteLine(response.ToString());

                Console.WriteLine("------------------------------");

                var parameters2 = new Dictionary<string, string> { { "key", "key" }, { "eventJson", "json" }, { "clientDT", new DateTime(1999, 5, 6, 14, 38, 12, 234).ToString() } };
                var encodedContent2 = new FormUrlEncodedContent(parameters2);

                var response2 = client.PostAsync("statistics/add", encodedContent2).Result;
                Console.WriteLine(response2.ToString());

                var parameters3 = new Dictionary<string, string> { { "key", "ключ" }, { "eventJson", "json2" } };
                var encodedContent3 = new FormUrlEncodedContent(parameters3);

                var response3 = client.PostAsync("statistics/add", encodedContent3).Result;
                Console.WriteLine(response3.ToString());

                var response4 = client.GetAsync("statistics/get?key=key").Result;
                Console.WriteLine(response4.ToString());

                var result = response4.Content.ReadAsStringAsync().Result;
                Console.WriteLine(result);

                var some = JToken.Parse(result);
                Console.WriteLine(some.ToString(Newtonsoft.Json.Formatting.Indented));
                

            }
            Console.WriteLine("Bye!");
            Console.ReadLine();
            */
        }
    }
}
